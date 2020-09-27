using System.Collections.Generic;
using UnityEngine;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Models.Fleets;

namespace Iam.Scripts.Models.Factions
{
    public class FactionTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public Color Color { get; }
        public IReadOnlyDictionary<int, SoldierType> SoldierTypes { get; }
        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; }
        public IReadOnlyDictionary<int, ArmorTemplate> ArmorTemplates { get; }
        public IReadOnlyDictionary<int, WeaponSet> WeaponSets { get; }
        public IReadOnlyDictionary<int, SoldierTemplate> SoldierTemplates { get; }
        public IReadOnlyDictionary<int, SquadTemplate> SquadTemplates { get; }
        public IReadOnlyDictionary<int, UnitTemplate> UnitTemplates { get; }
        public IReadOnlyDictionary<int, ShipTemplate> ShipTemplates { get; }
        public IReadOnlyDictionary<int, BoatTemplate> BoatTemplates { get; }
        public IReadOnlyDictionary<int, FleetTemplate> FleetTemplates { get; }
        
        public FactionTemplate(int id, string name, Color color,
                       IReadOnlyDictionary<int, SoldierType> soldierTypes,
                       IReadOnlyDictionary<int, RangedWeaponTemplate> rangedWeaponTemplates,
                       IReadOnlyDictionary<int, MeleeWeaponTemplate> meleeWeaponTemplates,
                       IReadOnlyDictionary<int, ArmorTemplate> armorTemplates,
                       IReadOnlyDictionary<int, WeaponSet> weaponSets,
                       IReadOnlyDictionary<int, SoldierTemplate> soldierTemplates,
                       IReadOnlyDictionary<int, SquadTemplate> squadTemplates,
                       IReadOnlyDictionary<int, UnitTemplate> unitTemplates,
                       IReadOnlyDictionary<int, BoatTemplate> boatTemplates,
                       IReadOnlyDictionary<int, ShipTemplate> shipTemplates,
                       IReadOnlyDictionary<int, FleetTemplate> fleetTemplates)
        {
            Id = id;
            Name = name;
            Color = color;
            SoldierTypes = soldierTypes;
            RangedWeaponTemplates = rangedWeaponTemplates;
            MeleeWeaponTemplates = meleeWeaponTemplates;
            ArmorTemplates = armorTemplates;
            WeaponSets = weaponSets;
            SoldierTemplates = soldierTemplates;
            SquadTemplates = squadTemplates;
            UnitTemplates = unitTemplates;
            BoatTemplates = boatTemplates;
            ShipTemplates = shipTemplates;
            FleetTemplates = fleetTemplates;
        }
    }
}
