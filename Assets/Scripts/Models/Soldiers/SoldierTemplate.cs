using System.Collections.Generic;

namespace OnlyWar.Scripts.Models.Soldiers
{
    public class SkillTemplate : NormalizedValueTemplate
    {
        public BaseSkill BaseSkill;
    }

    public class SoldierTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public SoldierType Type { get; }

        // attributes
        public NormalizedValueTemplate Strength { get; }
        public NormalizedValueTemplate Dexterity { get; }
        public NormalizedValueTemplate Perception { get; }
        public NormalizedValueTemplate Intelligence { get; }
        public NormalizedValueTemplate Ego { get; }
        public NormalizedValueTemplate Charisma { get; }
        public NormalizedValueTemplate Constitution { get; }
        public NormalizedValueTemplate PsychicPower { get; }
        
        public NormalizedValueTemplate AttackSpeed { get; }
        public NormalizedValueTemplate MoveSpeed { get; }
        public NormalizedValueTemplate Size { get; }
        public IReadOnlyCollection<SkillTemplate> SkillTemplates { get; }
        public BodyTemplate BodyTemplate { get; }

        public SoldierTemplate(int id, string name, SoldierType type, NormalizedValueTemplate strength,
                               NormalizedValueTemplate dex, NormalizedValueTemplate con, NormalizedValueTemplate intl,
                               NormalizedValueTemplate per, NormalizedValueTemplate ego, NormalizedValueTemplate pre,
                               NormalizedValueTemplate psy, NormalizedValueTemplate atk, NormalizedValueTemplate mov,
                               NormalizedValueTemplate siz, IReadOnlyCollection<SkillTemplate> skillTemplates,
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
            Charisma = pre;
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