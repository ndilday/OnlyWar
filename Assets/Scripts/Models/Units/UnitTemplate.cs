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
        public List<SpaceMarineRank> Members { get; private set; }
        public List<UnitWeaponOption> WeaponOptions { get; private set; }
        public WeaponSet DefaultWeapons { get; private set; }

        private UnitTemplate _parentUnit;
        private List<UnitTemplate> _childUnits;

        public UnitTemplate(int id, string name, WeaponSet defaultWeapons, List<UnitWeaponOption> weaponOptions)
        {
            Id = id;
            Name = name;
            _childUnits = new List<UnitTemplate>();
            Members = new List<SpaceMarineRank>();
            DefaultWeapons = defaultWeapons;
            WeaponOptions = weaponOptions;
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
            foreach (SpaceMarineRank rank in Members)
            {
                if (rankCounts.ContainsKey(rank))
                {
                    rankCounts[rank]++;
                }
                else
                {
                    rankCounts[rank] = 1;
                }
            }
            foreach (UnitTemplate child in GetChildUnits())
            {
                child.AddRankCounts(rankCounts);
            }
        }
    }

}
