using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.AI;

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

        public Unit GenerateUnitFromTemplateWithoutChildren(int id, string name)
        {
            return new Unit(id, name, this);
        }

        public void AddRankCounts(Dictionary<SpecialtyRank, int> rankCounts)
        {
            foreach(SpecialtyRank rank in Members)
            {
                if(rankCounts.ContainsKey(rank))
                {
                    rankCounts[rank]++;
                }
                else
                {
                    rankCounts[rank] = 1;
                }
            }
            foreach(UnitTemplate child in ChildUnits)
            {
                child.AddRankCounts(rankCounts);
            }
        }
    }

    public class Unit
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public UnitTemplate UnitTemplate { get; private set; }
        public List<Soldier> Members;
        public List<int> AssignedVehicles;
        public List<Unit> ChildUnits;
        public Unit(int id, string name, UnitTemplate template)
        {
            Id = id;
            Name = name;
            UnitTemplate = template;
            Members = new List<Soldier>();
            AssignedVehicles = new List<int>();
            ChildUnits = new List<Unit>();
        }
        public IEnumerable<Soldier> GetAllMembers()
        {
            if(ChildUnits == null || ChildUnits.Count == 0)
            {
                return Members;
            }
            return Members.Union(ChildUnits.SelectMany(u => u.GetAllMembers()));
        }
    }
}