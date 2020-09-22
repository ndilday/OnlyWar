using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Models.Squads
{
    public class Squad
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public Unit ParentUnit { get; }
        public SquadTemplate SquadTemplate { get; private set; }
        public bool IsInReserve { get; set; }
        public ISoldier SquadLeader { get => Members.FirstOrDefault(m => m.Type.IsSquadLeader); }
        public List<ISoldier> Members { get; }
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<WeaponSet> Loadout { get; set; }
        //public List<int> AssignedVehicles;
        public Squad(int id, string name, Unit parentUnit, SquadTemplate template)
        {
            Id = id;
            Name = name;
            ParentUnit = parentUnit;
            SquadTemplate = template;
            IsInReserve = true;
            Members = new List<ISoldier>();
            //AssignedVehicles = new List<int>();
            Loadout = new List<WeaponSet>();
        }
    }
}
