using System;
using System.Collections.Generic;

namespace Iam.Scripts.Models
{
    // TODO: replace this with a 
    public class Specialty
    {
        public int Id;
        public string Name;
        List<SpecialtyRank> SpecialtyRanks;
    }

    public class SpecialtyRank
    {
        public int Id { get; private set; }
        public int Level { get; private set; }
        public bool IsOfficer { get; private set; }
        public string Name { get; private set; }
        public List<Equippable> AllowedEquipment;
        public SpecialtyRank(int id, int level, bool isOfficer, string name)
        {
            Id = id;
            Level = level;
            IsOfficer = isOfficer;
            Name = name;
            AllowedEquipment = new List<Equippable>();
        }
    }

    public class Soldier
    {
        public int Id;
        public SpecialtyRank Rank;
        public string FirstName;
        public string LastName;
        public Unit AssignedUnit;
        public List<Equippable> Equipment;
        public List<Weapon> Weapons;
        public Armor Armor;
        //public Dictionary<int, float> SkillMap;
        // skills
        public float Melee;
        public float Ranged;
        public float Strength;
        public float Dexterity;
        public float Perception;
        public float Intelligence;
        public float Ego;
        public float Presence;
        public float Constitution;
        
        public float Speed;
        
        public float Piety;
        public float PsychicAbility;
        public float Piloting;
        public float TechRepair;
        public float Medicine;
        public List<string> SoldierHistory;

        // TODO: break out weapon specializations
        public float MeleeScore;
        public float RangedScore;
        public float LeadershipScore;
        public float MedicalScore;
        public float TechScore;
        public float PietyScore;
        public float AncientScore;

        public Body Body;
        public bool IsUnconscious;
        public Soldier()
        {
            Equipment = new List<Equippable>();
            Weapons = new List<Weapon>();
            SoldierHistory = new List<string>();
            Body = new Body();
            IsUnconscious = false;
        }
    }
}