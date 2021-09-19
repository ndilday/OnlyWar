using System;
using System.Collections.Generic;
using System.Linq;

using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;

namespace OnlyWar.Models.Units
{
    public class Unit
    {
        private static int _nextId = 0;
        private readonly List<Squad> _squads;
        public int Id { get; private set; }
        public string Name { get; set; }
        public UnitTemplate UnitTemplate { get; private set; }
        public Squad HQSquad
        {
            get
            {
                return _squads.FirstOrDefault(s => (s.SquadTemplate.SquadType & SquadTypes.HQ) > 0);
            }
        }
        public IReadOnlyCollection<Squad> Squads { get => _squads; }
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<int> AssignedVehicles;
        public List<Unit> ChildUnits;
        public Unit ParentUnit;

        public Unit(int id, string name, UnitTemplate template, List<Squad> squads)
        {
            Id = id;
            if(id > _nextId)
            {
                _nextId = id + 1;
            }
            Name = name;
            UnitTemplate = template;
            _squads = squads;
        }
        public Unit(string name, UnitTemplate template)
        {
            Id = _nextId++;
            Name = name;
            UnitTemplate = template;
            AssignedVehicles = new List<int>();
            ChildUnits = new List<Unit>();
            
            int i = 1;
            
            _squads = new List<Squad>();
            if (template.HQSquad != null)
            {
                _squads.Add(new Squad(name + " HQ Squad", this, template.HQSquad));
                i++;
            }
            foreach (SquadTemplate squadTemplate in template.GetChildSquads())
            {
                _squads.Add(new Squad(squadTemplate.Name, this, squadTemplate));
                i++;
            }
        }
        public IEnumerable<ISoldier> GetAllMembers()
        {
            IEnumerable<ISoldier> soldiers = null;
            if(Squads != null)
            {
                soldiers = Squads.SelectMany(s => s.Members);
            }
            if(ChildUnits != null)
            {
                soldiers = soldiers.Union(ChildUnits.SelectMany(u => u.GetAllMembers()));
            }
            return soldiers;
        }

        public IEnumerable<Squad> GetAllSquads()
        {
            IEnumerable<Squad> squads = null;
            if (Squads != null)
            {
                squads = Squads;
            }
            if (ChildUnits != null)
            {
                squads = squads.Union(ChildUnits.SelectMany(u => u.GetAllSquads()));
            }
            return squads;
        }

        public void AddSquad(Squad squad)
        {
            _squads.Add(squad);
        }

        public void RemoveSquad(Squad squad)
        {
            if(squad.Members.Count != 0)
            {
                throw new InvalidOperationException("Deleted squad still has members!");
            }
            _squads.Remove(squad);
        }

        public override string ToString()
        {
            return Name + ParentUnit == null ? "" : ", " + ParentUnit.Name;
        }
    }
}