using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iam.Scripts.Models
{

    public class UnitTemplate
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<SpecialtyRank> Members { get; private set; }
        public List<UnitTemplate> ChildUnits { get; private set; }

        public UnitTemplate(int id, string name)
        {
            Id = id;
            Name = name;
            ChildUnits = new List<UnitTemplate>();
            Members = new List<SpecialtyRank>();
        }
    }

    public class Unit
    {
        public int Id;
        public UnitTemplate Template;
        public string Name;
        public List<Soldier> Members;
        public List<int> AssignedVehicles;
    }
}