using System.Collections.Generic;

namespace Iam.Scripts.Models.Soldiers
{
    public class SpaceMarineRank
    {
        public int Id { get; private set; }
        public int Level { get; private set; }
        public bool IsOfficer { get; private set; }
        public string Name { get; private set; }
        public List<Equippable> AllowedEquipment;
        public SpaceMarineRank(int id, int level, bool isOfficer, string name)
        {
            Id = id;
            Level = level;
            IsOfficer = isOfficer;
            Name = name;
            AllowedEquipment = new List<Equippable>();
        }
    }

    class TempSpaceMarineTemplate : SoldierTemplate
    {
        private static TempSpaceMarineTemplate _instance = null;

        public static TempSpaceMarineTemplate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempSpaceMarineTemplate();
                }
                return _instance;
            }
        }

        private TempSpaceMarineTemplate()
        {
            Id = 0;
            Melee = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1};
            Ranged = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Strength = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Constitution = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Dexterity = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Perception = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Ego = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Presence = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Intelligence = new AttributeTemplate { BaseValue = 10, StandardDeviation = 2 };

            AttackSpeed = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            MoveSpeed = new AttributeTemplate { BaseValue = 6, StandardDeviation = 0 };
            Size = new AttributeTemplate { BaseValue = 2.4f, StandardDeviation = 0 };

            SkillTemplates = new List<SkillTemplate>();
            SkillTemplates.Add(new SkillTemplate { Name = "Piety", Id = 1, BaseValue = 10, StandardDeviation = 2 });
            SkillTemplates.Add(new SkillTemplate { Name = "Piloting", Id = 2, BaseValue = 15, StandardDeviation = 1 });
            SkillTemplates.Add(new SkillTemplate { Name = "TechRepair", Id = 3, BaseValue = 10, StandardDeviation = 1 });
            SkillTemplates.Add(new SkillTemplate { Name = "Medicine", Id = 4, BaseValue = 10, StandardDeviation = 1 });
            // Psychic Ability is 0 or less for non-psychics
            SkillTemplates.Add(new SkillTemplate { Name = "Psychic Power", Id = 0, BaseValue = -2.5f, StandardDeviation = 1 });
            BodyTemplate = HumanBodyTemplate.Instance;
        }
    }

    class TempTyranidWarriorTemplate : SoldierTemplate
    {
        private static TempTyranidWarriorTemplate _instance = null;

        public static TempTyranidWarriorTemplate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempTyranidWarriorTemplate();
                }
                return _instance;
            }
        }

        private TempTyranidWarriorTemplate()
        {
            Id = 1;
            Melee = new AttributeTemplate { BaseValue = 15, StandardDeviation = 1 };
            Ranged = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Strength = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Constitution = new AttributeTemplate { BaseValue = 60, StandardDeviation = 3 };
            Dexterity = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Perception = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Ego = new AttributeTemplate { BaseValue = 25, StandardDeviation = 1 };
            Presence = new AttributeTemplate { BaseValue = 22.5f, StandardDeviation = 1 };
            Intelligence = new AttributeTemplate { BaseValue = 10, StandardDeviation = 2 };

            AttackSpeed = new AttributeTemplate { BaseValue = 25, StandardDeviation = 1 };
            MoveSpeed = new AttributeTemplate { BaseValue = 6, StandardDeviation = 0 };
            Size = new AttributeTemplate { BaseValue = 2.4f, StandardDeviation = 0 };

            BodyTemplate = TyranidWarriorBodyTemplate.Instance;

            SkillTemplates = new List<SkillTemplate>();
        }
    }
}
