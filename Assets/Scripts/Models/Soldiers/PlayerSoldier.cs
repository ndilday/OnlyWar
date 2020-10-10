using System;
using System.Collections.Generic;
using System.Linq;
using OnlyWar.Scripts.Models.Squads;

namespace OnlyWar.Scripts.Models.Soldiers
{
    // PlayerSoldier uses the decorator pattern to extend the Soldier class
    // with features we're only interested in for the player's troops
    public class PlayerSoldier : ISoldier
    {
        private readonly Soldier _soldier;
        private readonly List<string> _soldierHistory;
        private readonly Dictionary<int, ushort> _weaponCasualtyCountMap;
        private readonly Dictionary<int, ushort> _factionCasualtyCountMap;

        public Date ProgenoidImplantDate { get; set; }
        public IReadOnlyCollection<string> SoldierHistory { get => _soldierHistory; }
        public float MeleeRating { get; private set; }
        public float RangedRating { get; private set; }
        public float LeadershipRating { get; private set; }
        public float MedicalRating { get; private set; }
        public float TechRating { get; private set; }
        public float PietyRating { get; private set; }
        public float AncientRating { get; private set; }
        public Squad AssignedSquad { get; private set; }
        public IReadOnlyDictionary<int, ushort> WeaponCasualtyCountMap { get => _weaponCasualtyCountMap; }
        public IReadOnlyDictionary<int, ushort> FactionCasualtyCountMap { get => _factionCasualtyCountMap; }
        #region ISoldier passthrough
        public int Id => _soldier.Id;

        public string Name => _soldier.Name;

        public SoldierType Type { get => _soldier.Type; set => _soldier.Type = value; }

        public float Strength => _soldier.Strength;

        public float Dexterity => _soldier.Dexterity;

        public float Constitution => _soldier.Constitution;

        public float Perception => _soldier.Perception;

        public float Intelligence => _soldier.Intelligence;

        public float Ego => _soldier.Ego;

        public float Charisma => _soldier.Charisma;

        public float PsychicPower => _soldier.PsychicPower;

        public float AttackSpeed => _soldier.AttackSpeed;

        public float Size => _soldier.Size;

        public float MoveSpeed => _soldier.MoveSpeed;

        public Body Body => _soldier.Body;

        public int FunctioningHands => _soldier.FunctioningHands;

        public IReadOnlyCollection<Skill> Skills => _soldier.Skills;
        public bool IsWounded
        {
            get
            {
                return _soldier.Body.HitLocations.Any(hl => hl.Wounds.WoundTotal > 0);
            }
        }

        public bool IsDeployable
        {
            get
            {
                return !_soldier.Body.HitLocations.Any(hl => hl.Template.IsMotive && hl.IsCrippled)
                    && !_soldier.Body.HitLocations.Any(hl => hl.Template.IsVital && hl.IsCrippled);
            }
        }

        public void AddSkillPoints(BaseSkill skill, float points)
        {
            _soldier.AddSkillPoints(skill, points);
        }

        public void AddAttributePoints(Attribute attribute, float points)
        {
            _soldier.AddAttributePoints(attribute, points);
        }

        public float GetTotalSkillValue(BaseSkill skill)
        {
            return _soldier.GetTotalSkillValue(skill);
        }

        public Skill GetBestSkillInCategory(SkillCategory category)
        {
            return _soldier.GetBestSkillInCategory(category);
        }
        #endregion

        public PlayerSoldier(Soldier soldier, string name)
        {
            _soldier = soldier;
            _soldier.Name = name;
            _soldierHistory = new List<string>();
            _weaponCasualtyCountMap = new Dictionary<int, ushort>();
            _factionCasualtyCountMap = new Dictionary<int, ushort>();
        }

        public PlayerSoldier(Soldier soldier, float melee, float ranged,
                             float leadership, float medical, float tech,
                             float piety, float ancient, Date implantDate,
                             List<string> history,
                             Dictionary<int, ushort> weaponCasualties,
                             Dictionary<int, ushort> factionCasualties)
        {
            _soldier = soldier;
            _soldierHistory = history;
            MeleeRating = melee;
            RangedRating = ranged;
            LeadershipRating = leadership;
            MedicalRating = medical;
            TechRating = tech;
            PietyRating = piety;
            AncientRating = ancient;
            ProgenoidImplantDate = implantDate;
            _weaponCasualtyCountMap = weaponCasualties;
            _factionCasualtyCountMap = factionCasualties;
        }

        public void AssignToSquad(Squad squad)
        {
            if(AssignedSquad != null)
            {
                RemoveFromSquad();
            }
            AssignedSquad = squad;
            squad.AddSquadMember(this);
        }

        public void RemoveFromSquad()
        {
            AssignedSquad.RemoveSquadMember(this);
            AssignedSquad = null;
        }

        public void AddEntryToHistory(string entry)
        {
            _soldierHistory.Add(entry);
        }

        public void AddKill(int factionId, int weaponTemplateId)
        {
            if (_weaponCasualtyCountMap.ContainsKey(weaponTemplateId))
            {
                _weaponCasualtyCountMap[weaponTemplateId]++;
            }
            else
            {
                _weaponCasualtyCountMap[weaponTemplateId] = 1;
            }

            if (_factionCasualtyCountMap.ContainsKey(factionId))
            {
                _factionCasualtyCountMap[factionId]++;
            }
            else
            {
                _factionCasualtyCountMap[factionId] = 1;
            }
        }

        public void UpdateRatings()
        {
            // Melee score = (Speed * STR * Melee)
            // Expected score = 16 * 16 * 15.5/8 = 1000
            // low-end = 15 * 15 * 14/8 = 850
            // high-end = 17 * 17 * 16/8 = 578
            MeleeRating = _soldier.AttackSpeed * _soldier.Strength
                * GetTotalSkillValue(TempBaseSkillList.Instance.Sword) /
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            Skill bestRanged = _soldier.GetBestSkillInCategory(SkillCategory.Ranged);
            RangedRating = Perception * (Dexterity + bestRanged.SkillBonus) / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Leadership Score = EGO * Leadership * Tactics
            LeadershipRating = _soldier.Ego
                * GetTotalSkillValue(TempBaseSkillList.Instance.Leadership)
                * GetTotalSkillValue(TempBaseSkillList.Instance.Tactics)
                / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Ancient Score = EGO * BOD
            AncientRating = _soldier.Ego * _soldier.Constitution 
                / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Medical Score = INT * Medicine
            MedicalRating = GetTotalSkillValue(TempBaseSkillList.Instance.Diagnosis)
                * GetTotalSkillValue(TempBaseSkillList.Instance.FirstAid)
                / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Tech Score =  INT * TechRapair
            TechRating = GetTotalSkillValue(TempBaseSkillList.Instance.ArmorySmallArms)
                * GetTotalSkillValue(TempBaseSkillList.Instance.ArmoryVehicle)
                / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Piety Score = Piety * Ritual * Persuade
            PietyRating = GetTotalSkillValue(TempBaseSkillList.Instance.Piety)
                / UnityEngine.Random.Range(0.09f, 0.11f);
        }
    }
}
