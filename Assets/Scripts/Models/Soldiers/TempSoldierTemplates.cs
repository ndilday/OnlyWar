using System.Collections.Generic;

using Iam.Scripts.Models.Equippables;

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
            Strength = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            Constitution = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            Dexterity = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            Perception = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            Ego = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            Presence = new AttributeTemplate { BaseValue = 11, StandardDeviation = 0.5f };
            Intelligence = new AttributeTemplate { BaseValue = 10, StandardDeviation = 0.5f };

            AttackSpeed = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            MoveSpeed = new AttributeTemplate { BaseValue = 6, StandardDeviation = 0 };
            Size = new AttributeTemplate { BaseValue = 2.4f, StandardDeviation = 0.1f };
            PsychicPower = new AttributeTemplate { BaseValue = -2.5f, StandardDeviation = 1 };

            SkillTemplates = new List<SkillTemplate>();
            SkillTemplates.Add(new SkillTemplate { BaseSkill= TempBaseSkillList.Instance.Piety, BaseValue = 2.5f, StandardDeviation = 0.5f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Ritual, BaseValue = -1.285f, StandardDeviation = 1f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Persuade, BaseValue = -1.285f, StandardDeviation = 1f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Leadership, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Tactics, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Marine, BaseValue = 2.5f, StandardDeviation = 0.5f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.ArmorySmallArms, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.ArmoryVehicle, BaseValue = -1.285f, StandardDeviation = 1f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.FirstAid, BaseValue = 2.5f, StandardDeviation = 0.5f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Diagnosis, BaseValue = 0.5f, StandardDeviation = 1f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.PowerArmor, BaseValue = -0.5f, StandardDeviation = 0.5f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Rhino, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.LandSpeeder, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryBeam, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryBolter, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryCannon, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryRocket, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.JumpPack, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Bolter, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Lascannon, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Plasma, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.MissileLauncher, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Flamer, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Throwing, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Shield, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Sword, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Axe, BaseValue = 1.6f, StandardDeviation = 0.2f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Fist, BaseValue = 2.5f, StandardDeviation = 0.5f });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Stealth, BaseValue = 0.5f, StandardDeviation = 1f });

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

            Strength = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            Constitution = new AttributeTemplate { BaseValue = 48, StandardDeviation = 3 };
            Dexterity = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            Perception = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            Ego = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            Presence = new AttributeTemplate { BaseValue = 20f, StandardDeviation = 1 };
            Intelligence = new AttributeTemplate { BaseValue = 9, StandardDeviation = 1 };

            AttackSpeed = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            MoveSpeed = new AttributeTemplate { BaseValue = 6, StandardDeviation = 0 };
            Size = new AttributeTemplate { BaseValue = 2.4f, StandardDeviation = 0 };
            PsychicPower = new AttributeTemplate { BaseValue = 0f, StandardDeviation = 0f };

            BodyTemplate = TyranidWarriorBodyTemplate.Instance;

            SkillTemplates = new List<SkillTemplate>();
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.OpponentMelee, BaseValue = 1, StandardDeviation = 0 });
            SkillTemplates.Add(new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.OpponentMelee, BaseValue = 1, StandardDeviation = 0 });
        }
    }
}
