using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Fleets;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using UnityEngine;

namespace Iam.Scripts.Models.Factions
{
    public sealed class TempFactions
    {
        private static TempFactions _instance;
        public static TempFactions Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempFactions();
                }
                return _instance;
            }
        }

        public FactionTemplate SpaceMarineFaction { get; }
        public FactionTemplate TyranidFaction { get; }

        private TempFactions() 
        {
            SpaceMarineFaction = new FactionTemplate(1, "Space Marines", Color.blue,
                                                     TempSoldierTypes.Instance.SpaceMarineSoldierTypes,
                                                     TempSpaceMarineEquippables.Instance.RangedWeaponTemplates,
                                                     TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates,
                                                     TempSpaceMarineEquippables.Instance.ArmorTemplates,
                                                     TempSpaceMarineWeaponSets.Instance.WeaponSets,
                                                     TempSpaceMarineSoldierTemplate.Instance.SoldierTemplates,
                                                     TempSpaceMarineSquadTemplates.Instance.SquadTemplates,
                                                     TempSpaceMarineUnitTemplates.Instance.UnitTemplates,
                                                     TempSpaceMarineShipTemplates.Instance.BoatTemplates,
                                                     TempSpaceMarineShipTemplates.Instance.ShipTemplates,
                                                     TempSpaceMarineFleetTemplates.Instance.FleetTemplates);
            TyranidFaction = new FactionTemplate(2, "Tyranids", Color.magenta,
                                                 TempSoldierTypes.Instance.TyranidSoldierTypes,
                                                 TempTyranidEquippables.Instance.RangedWeaponTemplates,
                                                 TempTyranidEquippables.Instance.MeleeWeaponTemplates,
                                                 TempTyranidEquippables.Instance.ArmorTemplates,
                                                 TempTyranidWeaponSets.Instance.WeaponSets,
                                                 TempTyranidSoldierTemplates.Instance.SoldierTemplates,
                                                 TempTyranidSquadTemplates.Instance.SquadTemplates,
                                                 TempTyranidUnitTemplates.Instance.UnitTemplates,
                                                 null,
                                                 null,
                                                 null);
        }
    }
}
