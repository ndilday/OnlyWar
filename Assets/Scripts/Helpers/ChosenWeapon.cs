using UnityEngine;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers
{
    public class ChosenRangedWeapon
    {
        public RangedWeapon ActiveWeapon { get; private set; }
        public ISoldier Soldier { get; private set; }
        public ChosenRangedWeapon(RangedWeapon weapon, ISoldier soldier)
        {
            ActiveWeapon = weapon;
            Soldier = soldier;
        }

        public float GetStrengthAtRange(float range)
        {
            return ActiveWeapon.Template.DamageMultiplier * (1 - (range / ActiveWeapon.Template.MaximumRange));
        }

        public float GetAccuracyAtRange(float range)
        {
            return ActiveWeapon.Template.Accuracy 
                + Soldier.GetTotalSkillValue(ActiveWeapon.Template.RelatedSkill) 
                + CalculateRangeModifier(range);
            
        }

        private float CalculateRangeModifier(float range)
        {
            return 2.4663f * Mathf.Log(2f / range);
        }
    }
}
