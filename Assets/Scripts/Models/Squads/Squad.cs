using OnlyWar.Models.Equippables;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Units;
using System.Collections.Generic;
using System.Linq;

namespace OnlyWar.Models.Squads
{
    public class Squad
    {
        private static int _nextId = 0;
        private readonly List<ISoldier> _members;
        public int Id { get; private set; }
        public string Name { get; set; }
        public Unit ParentUnit { get; set; }
        public SquadTemplate SquadTemplate { get; private set; }
        public bool IsInReserve { get; set; }
        public ISoldier SquadLeader { get => Members.FirstOrDefault(m => m.Template.IsSquadLeader); }
        public IReadOnlyCollection<ISoldier> Members { get => _members; }
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<WeaponSet> Loadout { get; set; }
        public Planet Location { get; set; }
        public Ship BoardedLocation { get; set; }
        //public List<int> AssignedVehicles;
        public Squad(string name, Unit parentUnit, SquadTemplate template)
        {
            Id = _nextId++;
            Name = name;
            ParentUnit = parentUnit;
            SquadTemplate = template;
            IsInReserve = true;
            _members = new List<ISoldier>();
            //AssignedVehicles = new List<int>();
            Loadout = new List<WeaponSet>();
        }

        public Squad(int id, string name, Unit parentUnit, SquadTemplate template, bool isInReserve)
        {
            Id = id;
            if(id > _nextId)
            {
                _nextId = id + 1;
            }
            Name = name;
            ParentUnit = parentUnit;
            SquadTemplate = template;
            IsInReserve = isInReserve;
            _members = new List<ISoldier>();
            //AssignedVehicles = new List<int>();
            Loadout = new List<WeaponSet>();
        }

        public void AddSquadMember(ISoldier soldier)
        {
            if (!_members.Contains(soldier))
            {
                _members.Add(soldier);
            }
        }

        public void RemoveSquadMember(ISoldier soldier)
        {
            if(_members.Contains(soldier))
            {
                _members.Remove(soldier);
            }
        }

        public override string ToString()
        {
            return Name + ", " + ParentUnit.Name;
        }
    }
}
