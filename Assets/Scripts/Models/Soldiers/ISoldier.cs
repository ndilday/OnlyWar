using OnlyWar.Models.Squads;
using System.Collections.Generic;

namespace OnlyWar.Models.Soldiers
{
    public interface ISoldier
    {
        int Id { get; }
        string Name { get; }
        SoldierTemplate Template { get; }

        float Strength { get; }
        float Dexterity { get; }
        float Constitution { get; }
        float Perception { get; }
        float Intelligence { get; }
        float Ego { get; }
        float Charisma { get; }
        float PsychicPower { get; }
        float AttackSpeed { get; }
        float Size { get; }
        float MoveSpeed { get; }
        Body Body { get; }
        Squad AssignedSquad { get; set; }

        IReadOnlyCollection<Skill> Skills { get; }

        int FunctioningHands { get; }
        void AddSkillPoints(BaseSkill skill, float points);
        void AddAttributePoints(Attribute attribute, float points);
        float GetTotalSkillValue(BaseSkill skill);
        Skill GetBestSkillInCategory(SkillCategory category);
    }
}
