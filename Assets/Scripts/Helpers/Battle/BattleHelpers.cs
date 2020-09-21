using UnityEngine;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle
{
    public static class BattleHelpers
    {
        public static float GetRangeForModifier(float modifier)
        {
            return 2 * Mathf.Exp(-modifier / 2.4663f);
        }

        public static float CalculateRangeModifier(float range, float relativeTargetSpeed)
        {
            // 
            return 2.4663f * Mathf.Log(2 / (range + relativeTargetSpeed));
        }

        public static float CalculateSizeModifier(float size)
        {
            // this is just the opposite of the range modifier
            return -CalculateRangeModifier(size, 0);
        }

        public static float CalculateRateOfFireModifier(int rateOfFire)
        {
            if (rateOfFire == 1) return 0;
            return Mathf.Log(rateOfFire, 2);
        }

        public static float CalculateDamageAtRange(RangedWeapon weapon, float range)
        {
            return weapon.Template.DoesDamageDegradeWithRange ?
                                weapon.Template.BaseDamage * (1 - (range / weapon.Template.MaximumDistance)) :
                                weapon.Template.BaseDamage;
        }
    }
}
