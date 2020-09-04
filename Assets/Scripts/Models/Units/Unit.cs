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
        public Squad HQSquad { get; private set; }
        public List<Squad> Squads { get; private set; }
        // if Loadout count < Member count, assume the rest are using the default loadout in the template
        public List<int> AssignedVehicles;
        public List<Unit> ChildUnits;
        public Unit ParentUnit;
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
                HQSquad = new Squad(id * 100 + i, name + " HQ Squad", template.HQSquad);
                i++;
            }
            
            Squads = new List<Squad>();
            foreach(SquadTemplate squadTemplate in template.GetChildSquads())
            {
                Squads.Add(new Squad(id * 100 + i, squadTemplate.Name, squadTemplate));
                i++;
            }
        }
        public IEnumerable<Soldier> GetAllMembers()
        {
            IEnumerable<Soldier> soldiers = null;
            if(Squads != null)
            {
                soldiers = Squads.SelectMany(s => s.GetAllMembers());
            }
            if(HQSquad != null)
            {
                soldiers = HQSquad.GetAllMembers().Union(soldiers);
            }
            if(ChildUnits != null)
            {
                soldiers = soldiers.Union(ChildUnits.SelectMany(u => u.GetAllMembers()));
            }
            return soldiers;
        }
        
        public IEnumerable<Squad> GetAllSquads()
        {
            return Squads.Union(new[] { HQSquad }).Union(ChildUnits.SelectMany(u => u.GetAllSquads()));
        }

        public void AddHQSquad(Squad hq)
        {
            HQSquad = hq;
        }

        public void AddSquad(Squad squad)
        {
            Squads.Add(squad);
        }

        public override string ToString()
        {
            return Name + ParentUnit == null ? "" : ", " + ParentUnit.Name;
        }
    }
}