using System;
using System.Collections.Generic;
using System.Linq;

using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;

namespace OnlyWar.Scripts.Models.Units
{
    public class Unit
    {
        private readonly List<Squad> _squads;
        public int Id { get; private set; }
        public string Name { get; set; }
        public UnitTemplate UnitTemplate { get; private set; }
        public Squad HQSquad { get; private set; }
        public IReadOnlyCollection<Squad> Squads { get => _squads; }
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<int> AssignedVehicles;
        public List<Unit> ChildUnits;
        public Unit ParentUnit;

        public Unit(int id, string name, UnitTemplate template, Squad hq, List<Squad> squads)
        {
            Id = id;
            Name = name;
            UnitTemplate = template;
            HQSquad = hq;
            _squads = squads;
        }
        public Unit(int id, string name, UnitTemplate template)
        {
            Id = id;
            Name = name;
            UnitTemplate = template;
            AssignedVehicles = new List<int>();
            ChildUnits = new List<Unit>();
            
            int i = 1;

            if (template.HQSquad != null)
            {
                HQSquad = new Squad(id * 100 + i, name + " HQ Squad", this, template.HQSquad);
                i++;
            }
            
            _squads = new List<Squad>();
            foreach(SquadTemplate squadTemplate in template.GetChildSquads())
            {
                _squads.Add(new Squad(id * 100 + i, squadTemplate.Name, this, squadTemplate));
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
            if(HQSquad != null)
            {
                soldiers = HQSquad.Members.Union(soldiers);
            }
            if(ChildUnits != null)
            {
                soldiers = soldiers.Union(ChildUnits.SelectMany(u => u.GetAllMembers()));
            }
            return soldiers;
        }
        
        public IEnumerable<Squad> GetAllSquads()
        {
            if (HQSquad != null)
            {
                return Squads.Union(new[] { HQSquad }).Union(ChildUnits.SelectMany(u => u.GetAllSquads()));
            }
            else
            {
                return Squads.Union(ChildUnits.SelectMany(u => u.GetAllSquads()));
            }
        }

        public void AddHQSquad(Squad hq)
        {
            HQSquad = hq;
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