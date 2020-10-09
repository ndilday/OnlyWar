using OnlyWar.Scripts.Models.Squads;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace OnlyWar.Scripts.Models.Soldiers
{
    public class Soldier : ISoldier
    {
        protected readonly Dictionary<int, Skill> _skills;

        public Soldier(BodyTemplate body)
        {
            _skills = new Dictionary<int, Skill>();
            Body = new Body(body);
        }

        public Soldier(List<HitLocation> hitLocations, List<Skill> skills)
        {
            _skills = skills.ToDictionary(skill => skill.BaseSkill.Id);
            Body = new Body(hitLocations);
        }

        public int FunctioningHands
        {
            get
            {
                int functioningHands = 2;
                if (Body.HitLocations.Any(hl => hl.Template.IsMeleeWeaponHolder && hl.IsCrippled))
                {
                    functioningHands--;
                }
                if (Body.HitLocations.Any(hl => hl.Template.IsRangedWeaponHolder && hl.IsCrippled))
                {
                    functioningHands--;
                }
                return functioningHands;
            }
        }

        public float Strength { get; set; }
        public float Dexterity { get; set; }
        public float Perception { get; set; }
        public float Intelligence { get; set; }
        public float Ego { get; set; }
        public float Charisma { get; set; }
        public float Constitution { get; set; }
        public float PsychicPower { get; set; }
        public float AttackSpeed { get; set; }
        public float Size { get; set; }
        public float MoveSpeed { get; set; }
        public Body Body { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public SoldierType Type { get; set; }
        public IReadOnlyCollection<Skill> Skills { get => _skills.Values; }

        public Squad AssignedSquad { get; set; }

        public void AddSkillPoints(BaseSkill skill, float points)
        {
            if(!_skills.ContainsKey(skill.Id))
            {
                _skills[skill.Id] = new Skill(skill, points);
            }
            else
            {
                _skills[skill.Id].AddPoints(points);
            }
        }

        public float GetTotalSkillValue(BaseSkill skill)
        {
            float attribute = GetStatForBaseAttribute(skill.BaseAttribute);
            if(!_skills.ContainsKey(skill.Id))
            {
                return attribute - 4;
            }

            return _skills[skill.Id].SkillBonus + attribute;
        }

        public float GetStatForBaseAttribute(Attribute attribute)
        {
            switch (attribute)
            {
                case Attribute.Dexterity:
                    return Dexterity;
                case Attribute.Intelligence:
                    return Intelligence;
                case Attribute.Ego:
                    return Ego;
                case Attribute.Presence:
                    return Charisma;
                case Attribute.Strength:
                    return Strength;
                case Attribute.Constitution:
                    return Constitution;
                default:
                    return Dexterity;
            }
        }

        public void AddAttributePoints(Attribute attribute, float points)
        {
            float curPoints;
            switch(attribute)
            {
                case Attribute.Constitution:
                    curPoints = Mathf.Pow(2, Constitution - 11) * 10;
                    Constitution = Mathf.Log((curPoints + points) / 10.0f, 2) + 11;
                    break;
                case Attribute.Dexterity:
                    curPoints = Mathf.Pow(2, Dexterity - 11) * 10;
                    Dexterity = Mathf.Log((curPoints + points) / 10.0f, 2) + 11;
                    break;
                case Attribute.Ego:
                    curPoints = Mathf.Pow(2, Ego - 11) * 10;
                    Ego = Mathf.Log((curPoints + points) / 10.0f, 2) + 11;
                    break;
                case Attribute.Intelligence:
                    curPoints = Mathf.Pow(2, Intelligence - 11) * 10;
                    Intelligence = Mathf.Log((curPoints + points) / 10.0f, 2) + 11;
                    break;
                case Attribute.Presence:
                    curPoints = Mathf.Pow(2, Charisma - 11) * 10;
                    Charisma = Mathf.Log((curPoints + points) / 10.0f, 2) + 11;
                    break;
                case Attribute.Strength:
                    curPoints = Mathf.Pow(2, Strength - 11) * 10;
                    Strength = Mathf.Log((curPoints + points) / 10.0f, 2) + 11;
                    break;
            }

        }
    
        public Skill GetBestSkillInCategory(SkillCategory category)
        {
            return _skills.Values.Where(s => s.BaseSkill.Category == category).OrderByDescending(s => s.SkillBonus).First();
        }
    }
}
