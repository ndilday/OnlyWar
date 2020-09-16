using System.Collections.Generic;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Models.Soldiers
{
    public abstract class Soldier
    {
        public int Id;
        public abstract string JobRole { get; }
        public Squad AssignedSquad;

        public float Strength;
        public float Dexterity;
        public float Perception;
        public float Intelligence;
        public float Ego;
        public float Presence;
        public float Constitution;
        public float PsychicPower;

        public float AttackSpeed;
        public float Size;
        // 1mph is approximately 0.5 yards/sec
        // military walk is about 3-4mph, so say 2 yd/s
        // military double-time is about 6mph, or 3 yd/s
        // military sprint with equipment is about 8-9 yd/s
        // in fiction, Impys and Marines move at the same speed, 6"
        // with 2s turns, a double time move of 6yd would match the 6" speed of double time
        public float MoveSpeed;
        private readonly Dictionary<WeaponTemplate, ushort> _weaponCasualtyCountMap;
        private readonly Dictionary<Faction, ushort> _factionCasualtyCountMap;
        public Dictionary<int, Skill> Skills;
        public List<string> SoldierHistory;
        public Body Body { get; private set; }

        public Soldier()
        {
            SoldierHistory = new List<string>();
            Skills = new Dictionary<int, Skill>();
            _weaponCasualtyCountMap = new Dictionary<WeaponTemplate, ushort>();
            _factionCasualtyCountMap = new Dictionary<Faction, ushort>();
        }

        public void InitializeBody(BodyTemplate bodyTemplate)
        {
            Body = new Body(bodyTemplate);
        }

        public void AddSkillPoints(BaseSkill skill, float points)
        {
            if(!Skills.ContainsKey(skill.Id))
            {
                Skills[skill.Id] = new Skill(skill, points);
            }
            else
            {
                Skills[skill.Id].AddPoints(points);
            }
        }

        public float GetTotalSkillValue(BaseSkill skill)
        {
            float attribute = GetStatForBaseAttribute(skill.BaseAttribute);
            if(!Skills.ContainsKey(skill.Id))
            {
                return attribute - 4;
            }

            return Skills[skill.Id].SkillBonus + attribute;
        }

        public void AddKill(Faction faction, WeaponTemplate weapon)
        {
            if(_weaponCasualtyCountMap.ContainsKey(weapon))
            {
                _weaponCasualtyCountMap[weapon]++;
            }
            else
            {
                _weaponCasualtyCountMap[weapon] = 1;
            }

            if (_factionCasualtyCountMap.ContainsKey(faction))
            {
                _factionCasualtyCountMap[faction]++;
            }
            else
            {
                _factionCasualtyCountMap[faction] = 1;
            }
        }

        private float GetStatForBaseAttribute(SkillAttribute attribute)
        {
            switch (attribute)
            {
                case SkillAttribute.Dexterity:
                    return Dexterity;
                case SkillAttribute.Intelligence:
                    return Intelligence;
                case SkillAttribute.Ego:
                    return Ego;
                case SkillAttribute.Presence:
                    return Presence;
                default:
                    return Dexterity;
            }
        }

        public abstract int FunctioningHands { get; }
    }
}
