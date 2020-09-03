using System.Collections.Generic;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public class WeaponSet
    {
        public string Name { get; set; }
        public WeaponTemplate MainWeapon { get; set; }
        public WeaponTemplate SecondaryWeapon { get; set; }
    }

    public class UnitWeaponOption
    {
        public string Name { get; private set; }
        public int MaxNumber { get; private set; }
        public int MinNumber { get; private set; }
        public List<WeaponSet> Options { get; private set; }

        public UnitWeaponOption(string name, int min, int max, List<WeaponSet> options)
        {
            Name = name;
            MinNumber = min;
            MaxNumber = max;
            Options = options;
        }
    }

    public class UnitTemplate
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public SquadTemplate HQSquad { get; private set; }

        private UnitTemplate _parentUnit;
        private List<UnitTemplate> _childUnits;
        private List<SquadTemplate> _childSquads;

        public UnitTemplate(int id, string name)
        {
            Id = id;
            Name = name;
            _childUnits = new List<UnitTemplate>();
            _childSquads = new List<SquadTemplate>();
        }

        public IEnumerable<UnitTemplate> GetChildUnits()
        {
            return _childUnits;
        }

        public IEnumerable<SquadTemplate> GetChildSquads()
        {
            return _childSquads;
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

        public void AddHQSquad(SquadTemplate hq)
        {
            HQSquad = hq;
        }

        public void AddSquad(SquadTemplate squad)
        {
            squad.SetUnit(this);
            _childSquads.Add(squad);
        }

        public Unit GenerateUnitFromTemplateWithoutChildren(int id, string name)
        {
            return new Unit(id, name, this);
        }
    }

}
