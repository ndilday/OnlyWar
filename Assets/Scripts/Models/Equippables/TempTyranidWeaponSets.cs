using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Equippables
{
    public class TempTyranidWeaponSets
    {
        private static TempTyranidWeaponSets _instance;

        public static TempTyranidWeaponSets Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempTyranidWeaponSets();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, WeaponSet> WeaponSets { get; }

        private TempTyranidWeaponSets()
        {
            WeaponSets = new List<WeaponSet>
            {
                new WeaponSet(1, "Deathspitter",
                              TempTyranidEquippables.Instance.RangedWeaponTemplates[1]),
                new WeaponSet(2, "Devourer",
                              TempSpaceMarineEquippables.Instance.RangedWeaponTemplates[2]),
                new WeaponSet(101, "Scything Talons",
                              null,
                              null,
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[101],
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[101]),
                new WeaponSet(102, "Rending Claws",
                              null,
                              null,
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[102],
                              TempSpaceMarineEquippables.Instance.MeleeWeaponTemplates[102])
            }.ToDictionary(ws => ws.Id);
        }
    }
}
