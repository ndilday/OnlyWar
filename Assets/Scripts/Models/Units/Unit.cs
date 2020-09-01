using Iam.Scripts.Models.Soldiers;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Units
{
    public class Unit
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public UnitTemplate UnitTemplate { get; private set; }
        public bool IsInReserve { get; set; }
        public List<Soldier> Members;
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<WeaponSet> Loadout { get; set; }
        public List<int> AssignedVehicles;
        public List<Unit> ChildUnits;
        public Unit ParentUnit;
        public Unit(int id, string name, UnitTemplate template)
        {
            Id = id;
            Name = name;
            UnitTemplate = template;
            IsInReserve = true;
            Members = new List<Soldier>();
            AssignedVehicles = new List<int>();
            ChildUnits = new List<Unit>();
            Loadout = new List<WeaponSet>();
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