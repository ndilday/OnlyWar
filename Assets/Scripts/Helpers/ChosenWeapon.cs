using UnityEngine;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers
{
    public class ChosenWeapon
    {
        public Weapon ActiveWeapon { get; private set; }
        public Soldier Soldier { get; private set; }
        public ChosenWeapon(Weapon weapon, Soldier soldier)
        {
            ActiveWeapon = weapon;
            Soldier = soldier;
        }

        public float GetStrengthAtRange(float range)
        {
            if (ActiveWeapon.Template.GetType() != typeof(RangedWeaponTemplate)) return 0;
            RangedWeaponTemplate template = (RangedWeaponTemplate)ActiveWeapon.Template;
            return template.BaseStrength * (1 - (range / template.MaximumDistance));
        }

        public float GetAccuracyAtRange(float range)
        {
            return ActiveWeapon.Template.Accuracy + GetStatForSkill() + Soldier.Skills[ActiveWeapon.Template.RelatedSkill.Id].SkillBonus + CalculateRangeModifier(range);
            
        }

        private float CalculateRangeModifier(float range)
        {
            return 2.4663f * Mathf.Log(2f / range);
        }

        private float GetStatForSkill()
        {
            switch (ActiveWeapon.Template.RelatedSkill.BaseAttribute)
            {
                case SkillAttribute.Dexterity:
                    return Soldier.Dexterity;
                case SkillAttribute.Intelligence:
                    return Soldier.Intelligence;
                case SkillAttribute.Ego:
                    return Soldier.Ego;
                case SkillAttribute.Presence:
                    return Soldier.Presence;
                default:
                    return Soldier.Dexterity;
            }
        }
    }
}
