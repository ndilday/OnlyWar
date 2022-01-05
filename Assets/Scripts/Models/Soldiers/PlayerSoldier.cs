using System;
using System.Collections.Generic;
using System.Linq;
using OnlyWar.Models.Squads;

namespace OnlyWar.Models.Soldiers
{
    // PlayerSoldier uses the decorator pattern to extend the Soldier class
    // with features we're only interested in for the player's troops
    public class PlayerSoldier : ISoldier
    {
        private readonly Soldier _soldier;
        private readonly List<string> _soldierHistory;
        private readonly Dictionary<int, ushort> _rangedWeaponCasualtyCountMap;
        private readonly Dictionary<int, ushort> _meleeWeaponCasualtyCountMap;
        private readonly Dictionary<int, ushort> _factionCasualtyCountMap;
        private Squad _assignedSquad;

        public Date ProgenoidImplantDate { get; set; }
        public IReadOnlyCollection<string> SoldierHistory { get => _soldierHistory; }
        public float MeleeRating { get; set; }
        public float RangedRating { get; set; }
        public float LeadershipRating { get; set; }
        public float MedicalRating { get; set; }
        public float TechRating { get; set; }
        public float PietyRating { get; set; }
        public float AncientRating { get; set; }
        public IReadOnlyDictionary<int, ushort> RangedWeaponCasualtyCountMap { get => _rangedWeaponCasualtyCountMap; }
        public IReadOnlyDictionary<int, ushort> MeleeWeaponCasualtyCountMap { get => _meleeWeaponCasualtyCountMap; }
        public IReadOnlyDictionary<int, ushort> FactionCasualtyCountMap { get => _factionCasualtyCountMap; }
        #region ISoldier passthrough
        public int Id => _soldier.Id;

        public string Name => _soldier.Name;

        public SoldierTemplate Template { get => _soldier.Template; set => _soldier.Template = value; }

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

        public Squad AssignedSquad
        {
            get { return _assignedSquad; }
            set { _assignedSquad = value; }
        }

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
            _rangedWeaponCasualtyCountMap = new Dictionary<int, ushort>();
            _meleeWeaponCasualtyCountMap = new Dictionary<int, ushort>();
            _factionCasualtyCountMap = new Dictionary<int, ushort>();
            if (soldier.AssignedSquad != null)
            {
                _assignedSquad = soldier.AssignedSquad;
                soldier.AssignedSquad = null;
                AssignedSquad.RemoveSquadMember(soldier);
                AssignedSquad.AddSquadMember(this);
            }
        }

        public PlayerSoldier(Soldier soldier, float melee, float ranged,
                             float leadership, float medical, float tech,
                             float piety, float ancient, Date implantDate,
                             List<string> history,
                             Dictionary<int, ushort> rangedWeaponCasualties,
                             Dictionary<int, ushort> meleeWeaponCasualties,
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
            _rangedWeaponCasualtyCountMap = rangedWeaponCasualties;
            _meleeWeaponCasualtyCountMap = meleeWeaponCasualties;
            _factionCasualtyCountMap = factionCasualties;
            if(soldier.AssignedSquad != null)
            {
                _assignedSquad = soldier.AssignedSquad;
                soldier.AssignedSquad = null;
                AssignedSquad.RemoveSquadMember(soldier);
                AssignedSquad.AddSquadMember(this);
            }
        }

        public void AddEntryToHistory(string entry)
        {
            _soldierHistory.Add(entry);
        }

        public void AddRangedKill(int factionId, int weaponTemplateId)
        {
            if (_rangedWeaponCasualtyCountMap.ContainsKey(weaponTemplateId))
            {
                _rangedWeaponCasualtyCountMap[weaponTemplateId]++;
            }
            else
            {
                _rangedWeaponCasualtyCountMap[weaponTemplateId] = 1;
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

        public void AddMeleeKill(int factionId, int weaponTemplateId)
        {
            if (_meleeWeaponCasualtyCountMap.ContainsKey(weaponTemplateId))
            {
                _meleeWeaponCasualtyCountMap[weaponTemplateId]++;
            }
            else
            {
                _meleeWeaponCasualtyCountMap[weaponTemplateId] = 1;
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

        public override string ToString()
        {
            return _soldier.ToString();
        }
    }
}
