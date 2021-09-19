using UnityEngine;

namespace OnlyWar.Models.Soldiers
{
    public enum Attribute
    {
        Strength = 1,
        Dexterity = 2,
        Constitution = 3,
        Intelligence = 4,
        Presence = 5,
        Ego = 6
    }

    public enum SkillCategory
    {
        Ranged = 1,
        Gunnery = 2,
        Melee = 3,
        Vehicle = 4,
        Military = 5,
        Professional = 6,
        Tech = 7,
        Apothecary = 8
    }

    public class BaseSkill
    {
        public int Id;
        public SkillCategory Category;
        public string Name;
        public Attribute BaseAttribute;
        public float Difficulty;
        public BaseSkill(int id, SkillCategory category, string name, Attribute baseAttribute, float difficulty)
        {
            Id = id;
            Category = category;
            Name = name;
            BaseAttribute = baseAttribute;
            Difficulty = difficulty;
        }
    }

    public class Skill
    {
        public BaseSkill BaseSkill { get; private set; }
        public float PointsInvested { get; private set; }

        public float SkillBonus
        {
            get
            {
                return (PointsInvested == 0 ? -4 : Mathf.Log(PointsInvested, 2)) - BaseSkill.Difficulty;
            }
        }
        public Skill(BaseSkill baseSkill, float points = 0)
        {
            BaseSkill = baseSkill;
            if (points < 0) PointsInvested = 0;
            else PointsInvested = points;
        }
        public void AddPoints(float points)
        {
            if(points > 0)
            {
                PointsInvested += points;
            }
        }
    }
}