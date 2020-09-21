using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Equippables
{
    public class TempSpaceMarineWeaponSets
    {
        private static TempSpaceMarineWeaponSets _instance;

        public static TempSpaceMarineWeaponSets Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempSpaceMarineWeaponSets();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, WeaponSet> WeaponSets { get; }

        private TempSpaceMarineWeaponSets()
        {
            WeaponSets = new List<WeaponSet>
            {
                new WeaponSet(0, "Boltgun", 
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[0]),
                new WeaponSet(1, "Bolt Pistol + Chainsword",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[1],
                              null,
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[101]),
                new WeaponSet(2, "Flamer",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[2]),
                new WeaponSet(3, "Plasma Gun",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[3]),
                new WeaponSet(4, "Meltagun",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[4]),
                new WeaponSet(5, "Heavy Bolter",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[5]),
                new WeaponSet(6, "Lascannon",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[6]),
                new WeaponSet(7, "Missile Launcher",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[7]),
                new WeaponSet(8, "Multi-melta",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[8]),
                new WeaponSet(9, "Plasma Cannon",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[9]),
                new WeaponSet(10, "Plasma Pistol + Chainsword",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[10],
                              null,
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[101]),
                new WeaponSet(11, "Sniper Rifle",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[11]),
                new WeaponSet(12, "Shotgun",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[12]),
                new WeaponSet(13, "Eviscerator",
                              null,
                              null,
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[103]),
                new WeaponSet(14, "Bolter + Bolt Pistol",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[0],
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[1]),
            }.ToDictionary(ws => ws.Id);
        }
    }
}
