using System.Collections.Generic;

namespace OnlyWar.Models.Soldiers
{
    public class SkillTemplate : NormalizedValueTemplate
    {
        public BaseSkill BaseSkill;
    }

    public class Species
    {
        public int Id { get; }
        public string Name { get; }

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
        public ushort Width { get; }
        public ushort Depth { get; }
        public BodyTemplate BodyTemplate { get; }

        public Species(int id, string name, NormalizedValueTemplate strength,
                       NormalizedValueTemplate dex, NormalizedValueTemplate con, 
                       NormalizedValueTemplate intl, NormalizedValueTemplate per, 
                       NormalizedValueTemplate ego, NormalizedValueTemplate cha,
                       NormalizedValueTemplate psy, NormalizedValueTemplate atk, 
                       NormalizedValueTemplate mov, NormalizedValueTemplate siz,
                       ushort width, ushort depth, BodyTemplate bodyTemplate)
        {
            Id = id;
            Name = name;
            Strength = strength;
            Dexterity = dex;
            Perception = per;
            Intelligence = intl;
            Ego = ego;
            Charisma = cha;
            Constitution = con;
            PsychicPower = psy;
            AttackSpeed = atk;
            MoveSpeed = mov;
            Size = siz;
            Width = width;
            Depth = depth;
            BodyTemplate = bodyTemplate;
        }
    }
}