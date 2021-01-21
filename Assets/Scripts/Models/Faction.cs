using System.Collections.Generic;
using UnityEngine;

using OnlyWar.Scripts.Models.Equippables;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;
using OnlyWar.Scripts.Models.Fleets;

namespace OnlyWar.Scripts.Models
{
    public enum GrowthType
    {
        None = 0,
        Logistic = 1,
        Conversion = 2
    }

    public class Faction
    {
        public int Id { get; }
        public string Name { get; }
        public Color Color { get; }
        public bool IsPlayerFaction { get; }
        public bool IsDefaultFaction { get; }
        public bool CanInfiltrate { get; }
        public GrowthType GrowthType { get; }
        public IReadOnlyDictionary<int, SoldierType> SoldierTypes { get; }
        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; }
        public IReadOnlyDictionary<int, ArmorTemplate> ArmorTemplates { get; }
        public IReadOnlyDictionary<int, SoldierTemplate> SoldierTemplates { get; }
        public IReadOnlyDictionary<int, SquadTemplate> SquadTemplates { get; }
        public IReadOnlyDictionary<int, UnitTemplate> UnitTemplates { get; }
        public IReadOnlyDictionary<int, ShipTemplate> ShipTemplates { get; }
        public IReadOnlyDictionary<int, BoatTemplate> BoatTemplates { get; }
        public IReadOnlyDictionary<int, FleetTemplate> FleetTemplates { get; }

        public List<Unit> Units { get; set; }
        
        public Faction(int id, string name, Color color, bool isPlayerFaction, 
                       bool isDefaultFaction, bool canInfiltrate, GrowthType growthType,
                       IReadOnlyDictionary<int, SoldierType> soldierTypes,
                       IReadOnlyDictionary<int, RangedWeaponTemplate> rangedWeaponTemplates,
                       IReadOnlyDictionary<int, MeleeWeaponTemplate> meleeWeaponTemplates,
                       IReadOnlyDictionary<int, ArmorTemplate> armorTemplates,
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
            IsPlayerFaction = isPlayerFaction;
            IsDefaultFaction = isDefaultFaction;
            CanInfiltrate = canInfiltrate;
            GrowthType = growthType;
            SoldierTypes = soldierTypes;
            RangedWeaponTemplates = rangedWeaponTemplates;
            MeleeWeaponTemplates = meleeWeaponTemplates;
            ArmorTemplates = armorTemplates;
            SoldierTemplates = soldierTemplates;
            SquadTemplates = squadTemplates;
            UnitTemplates = unitTemplates;
            BoatTemplates = boatTemplates ?? new Dictionary<int, BoatTemplate>();
            ShipTemplates = shipTemplates ?? new Dictionary<int, ShipTemplate>();
            FleetTemplates = fleetTemplates ?? new Dictionary<int, FleetTemplate>();
            foreach(UnitTemplate template in UnitTemplates.Values)
            {
                template.Faction = this;
            }
            Units = new List<Unit>();
        }
    }
}
