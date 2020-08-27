using Iam.Scripts.Models.Soldiers;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Units
{

    public class UnitTemplate
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<SpaceMarineRank> Members { get; private set; }
        private UnitTemplate _parentUnit;

        private List<UnitTemplate> _childUnits;

        public UnitTemplate(int id, string name)
        {
            Id = id;
            Name = name;
            _childUnits = new List<UnitTemplate>();
            Members = new List<SpaceMarineRank>();
        }

        public IEnumerable<UnitTemplate> GetChildUnits()
        {
            return _childUnits;
        }

        public void SetParentUnit(UnitTemplate parent)
        {
            _parentUnit = parent;
        }

        public void AddChildUnit(UnitTemplate child)
        {
            child.SetParentUnit(this);
            _childUnits.Add(child);
        }

        public Unit GenerateUnitFromTemplateWithoutChildren(int id, string name)
        {
            return new Unit(id, name, this);
        }

        public void AddRankCounts(Dictionary<SpaceMarineRank, int> rankCounts)
        {
            foreach(SpaceMarineRank rank in Members)
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
            foreach(UnitTemplate child in GetChildUnits())
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
        public Unit ParentUnit;
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

        public override string ToString()
        {
            return Name + ParentUnit == null ? "" : ", " + ParentUnit.Name;
        }
    }
}