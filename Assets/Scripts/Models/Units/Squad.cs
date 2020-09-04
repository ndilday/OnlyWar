using System.Collections.Generic;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public class Squad
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public SquadTemplate SquadTemplate { get; private set; }
        public bool IsInReserve { get; set; }
        public Soldier SquadLeader { get; set; }
        public List<Soldier> Members;
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<WeaponSet> Loadout { get; set; }
        public List<int> AssignedVehicles;
        public Squad(int id, string name, SquadTemplate template)
        {
            Id = id;
            Name = name;
            SquadTemplate = template;
            IsInReserve = true;
            Members = new List<Soldier>();
            AssignedVehicles = new List<int>();
            Loadout = new List<WeaponSet>();
        }
        public Soldier[] GetAllMembers()
        {
            int count = Members.Count + (SquadLeader == null ? 0 : 1);
            Soldier[] array = new Soldier[count];
            int i = 0;
            if(SquadLeader != null)
            {
                array[i] = SquadLeader;
                i++;
            }
            foreach(Soldier soldier in Members)
            {
                array[i] = soldier;
                i++;
            }
            return array;
        }
    }
}
