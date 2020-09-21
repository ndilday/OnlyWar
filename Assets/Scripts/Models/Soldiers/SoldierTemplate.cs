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

    public class SoldierTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public SoldierType Type { get; }

        // attributes
        public AttributeTemplate Strength { get; }
        public AttributeTemplate Dexterity { get; }
        public AttributeTemplate Perception { get; }
        public AttributeTemplate Intelligence { get; }
        public AttributeTemplate Ego { get; }
        public AttributeTemplate Presence { get; }
        public AttributeTemplate Constitution { get; }
        public AttributeTemplate PsychicPower { get; }
        
        public AttributeTemplate AttackSpeed { get; }
        public AttributeTemplate MoveSpeed { get; }
        public AttributeTemplate Size { get; }
        public IReadOnlyCollection<SkillTemplate> SkillTemplates { get; }
        public BodyTemplate BodyTemplate { get; }

        public SoldierTemplate(int id, string name, SoldierType type, AttributeTemplate strength,
                               AttributeTemplate dex, AttributeTemplate per, AttributeTemplate intl,
                               AttributeTemplate ego, AttributeTemplate pre, AttributeTemplate con,
                               AttributeTemplate psy, AttributeTemplate atk, AttributeTemplate mov,
                               AttributeTemplate siz, IReadOnlyCollection<SkillTemplate> skillTemplates,
                               BodyTemplate bodyTemplate)
        {
            Id = id;
            Name = name;
            Type = type;
            Strength = strength;
            Dexterity = dex;
            Perception = per;
            Intelligence = intl;
            Ego = ego;
            Presence = pre;
            Constitution = con;
            PsychicPower = psy;
            AttackSpeed = atk;
            MoveSpeed = mov;
            Size = siz;
            SkillTemplates = skillTemplates;
            BodyTemplate = bodyTemplate;
        }
    }
}