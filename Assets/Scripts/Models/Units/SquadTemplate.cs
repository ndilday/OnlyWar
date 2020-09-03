using System.Collections.Generic;

using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public class SquadTemplate
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public UnitTemplate Unit { get; private set; }
        public SoldierTemplate SquadLeader { get; private set; }
        public List<SoldierTemplate> Members { get; private set; }
        public List<UnitWeaponOption> WeaponOptions { get; private set; }
        public WeaponSet DefaultWeapons { get; private set; }

        public SquadTemplate(int id, string name, WeaponSet defaultWeapons, List<UnitWeaponOption> weaponOptions)
        {
            Id = id;
            Name = name;
            Members = new List<SoldierTemplate>();
            DefaultWeapons = defaultWeapons;
            WeaponOptions = weaponOptions;
        }

        public void AddSquadLeader(SoldierTemplate soldierTemplate)
        {
            SquadLeader = soldierTemplate;
        }

        public void SetUnit(UnitTemplate unit)
        {
            Unit = unit;
        }
    }
}
