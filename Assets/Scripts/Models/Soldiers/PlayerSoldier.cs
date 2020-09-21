using System;
using System.Collections.Generic;

using Iam.Scripts.Models.Squads;

namespace Iam.Scripts.Models.Soldiers
{
    public class SkillRanking
    {

    }

    // PlayerSoldier uses the decorator pattern to extend the Soldier class
    // with features we're only interested in for the player's troops
    public class PlayerSoldier : ISoldier
    {
        private readonly Soldier _soldier;

        private float _meleeRating;
        private float _rangedRating;
        private float _leadershipRating;
        private float _medicalRating;
        private float _techRating;
        private float _pietyRating;
        private float _ancientRating;

        private readonly List<string> _soldierHistory;
        private readonly Dictionary<int, ushort> _weaponCasualtyCountMap;
        private readonly Dictionary<int, ushort> _factionCasualtyCountMap;

        public Squad AssignedSquad { get; private set; }
        public Date ProgenoidImplantDate { get; set; }
        public IReadOnlyCollection<string> SoldierHistory { get => _soldierHistory; }
        public float MeleeRating { get => _meleeRating; }
        public float RangedRating { get => _rangedRating; }
        public float LeadershipRating { get => _leadershipRating; }
        public float MedicalRating { get => _medicalRating; }
        public float TechRating { get => _techRating; }
        public float PietyRating { get => _pietyRating; }
        public float AncientRating { get => _ancientRating; }
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

        public float Presence => _soldier.Presence;

        public float PsychicPower => _soldier.PsychicPower;

        public float AttackSpeed => _soldier.AttackSpeed;

        public float Size => _soldier.Size;

        public float MoveSpeed => _soldier.MoveSpeed;

        public Body Body => _soldier.Body;

        public int FunctioningHands => _soldier.FunctioningHands;

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

        public void AssignToSquad(Squad squad)
        {
            if(AssignedSquad != null)
            {
                RemoveFromSquad();
            }
            AssignedSquad = squad;
        }

        public void RemoveFromSquad()
        {
            AssignedSquad.Members.Remove(_soldier);
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
            _meleeRating = _soldier.AttackSpeed * _soldier.Strength
                * GetTotalSkillValue(TempBaseSkillList.Instance.Sword) /
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            Skill bestRanged = _soldier.GetBestSkillInCategory(SkillCategory.Ranged);
            _rangedRating = Perception * (Dexterity + bestRanged.SkillBonus) / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Leadership Score = EGO * Leadership * Tactics
            _leadershipRating = _soldier.Ego
                * GetTotalSkillValue(TempBaseSkillList.Instance.Leadership)
                * GetTotalSkillValue(TempBaseSkillList.Instance.Tactics)
                / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Ancient Score = EGO * BOD
            _ancientRating = _soldier.Ego * _soldier.Constitution 
                / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Medical Score = INT * Medicine
            _medicalRating = GetTotalSkillValue(TempBaseSkillList.Instance.Diagnosis)
                * GetTotalSkillValue(TempBaseSkillList.Instance.FirstAid)
                / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Tech Score =  INT * TechRapair
            _techRating = GetTotalSkillValue(TempBaseSkillList.Instance.ArmorySmallArms)
                * GetTotalSkillValue(TempBaseSkillList.Instance.ArmoryVehicle)
                / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Piety Score = Piety * Ritual * Persuade
            _pietyRating = GetTotalSkillValue(TempBaseSkillList.Instance.Piety)
                / UnityEngine.Random.Range(0.09f, 0.11f);
        }
    }
}
