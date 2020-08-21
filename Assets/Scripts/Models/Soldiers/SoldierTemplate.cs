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
        public string Name;
        public int Id;
    }
    
    public abstract class SoldierTemplate
    {
        public int Id;

        // skills
        public AttributeTemplate Melee;
        public AttributeTemplate Ranged;
        public AttributeTemplate Strength;
        public AttributeTemplate Dexterity;
        public AttributeTemplate Perception;
        public AttributeTemplate Intelligence;
        public AttributeTemplate Ego;
        public AttributeTemplate Presence;
        public AttributeTemplate Constitution;
        
        public AttributeTemplate AttackSpeed;
        public AttributeTemplate MoveSpeed;
        public AttributeTemplate Size;

        public List<SkillTemplate> SkillTemplates;

        // TODO: Need to factor psychic ability back in

        public BodyTemplate BodyTemplate;
    }
}