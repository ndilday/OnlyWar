using System.Collections.Generic;

namespace Iam.Scripts.Models.Soldiers
{
    public class AttributeTemplate
    {
        public float BaseValue;
        public float StandardDeviation;
    }

    public class SkillTemplate : AttributeTemplate
    {
        public BaseSkill BaseSkill;
    }

    public abstract class SoldierTemplate
    {
        public int Id;

        // attributes
        public AttributeTemplate Strength;
        public AttributeTemplate Dexterity;
        public AttributeTemplate Perception;
        public AttributeTemplate Intelligence;
        public AttributeTemplate Ego;
        public AttributeTemplate Presence;
        public AttributeTemplate Constitution;
        public AttributeTemplate PsychicPower;
        
        public AttributeTemplate AttackSpeed;
        public AttributeTemplate MoveSpeed;
        public AttributeTemplate Size;

        public List<SkillTemplate> SkillTemplates;

        // TODO: Need to factor psychic ability back in

        public BodyTemplate BodyTemplate;
    }
}