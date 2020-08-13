using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public string Name;
        public Unit AssignedUnit;
        public List<Equippable> Equipment;
        public Dictionary<int, float> SkillMap;
    }
}