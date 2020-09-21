using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Squads
{
    public class Squad
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public SquadTemplate SquadTemplate { get; private set; }
        public bool IsInReserve { get; set; }
        public ISoldier SquadLeader { get => Members.FirstOrDefault(m => m.Type.IsSquadLeader); }
        public List<ISoldier> Members { get; }
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<WeaponSet> Loadout { get; set; }
        //public List<int> AssignedVehicles;
        public Squad(int id, string name, SquadTemplate template)
        {
            Id = id;
            Name = name;
            SquadTemplate = template;
            IsInReserve = true;
            Members = new List<ISoldier>();
            //AssignedVehicles = new List<int>();
            Loadout = new List<WeaponSet>();
        }
    }
}
