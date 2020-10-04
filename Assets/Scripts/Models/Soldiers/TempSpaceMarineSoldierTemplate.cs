using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Soldiers
{
    public class TempSpaceMarineSoldierTemplate
    {
        private static TempSpaceMarineSoldierTemplate _instance;
        public static TempSpaceMarineSoldierTemplate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempSpaceMarineSoldierTemplate();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, SoldierTemplate> SoldierTemplates { get; }

        private TempSpaceMarineSoldierTemplate()
        {
            AttributeTemplate fifteen = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            AttributeTemplate pre = new AttributeTemplate { BaseValue = 10, StandardDeviation = 1f };
            AttributeTemplate intl = new AttributeTemplate { BaseValue = 15, StandardDeviation = 0.5f };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 6.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 2.4f, StandardDeviation = 0.1f };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = -2.5f, StandardDeviation = 1f };
            List<SkillTemplate> skills = new List<SkillTemplate>
            {
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Piety, BaseValue = 2.5f, StandardDeviation = 0.5f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Ritual, BaseValue = -1.285f, StandardDeviation = 1f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Persuade, BaseValue = -1.285f, StandardDeviation = 1f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Leadership, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Tactics, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Marine, BaseValue = 2.5f, StandardDeviation = 0.5f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.ArmorySmallArms, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.ArmoryVehicle, BaseValue = -1.285f, StandardDeviation = 1f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.FirstAid, BaseValue = 2.5f, StandardDeviation = 0.5f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Diagnosis, BaseValue = 0.5f, StandardDeviation = 1f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.PowerArmor, BaseValue = -0.5f, StandardDeviation = 0.5f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Rhino, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.LandSpeeder, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryBeam, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryBolter, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryCannon, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.GunneryRocket, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.JumpPack, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Bolter, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Lascannon, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Plasma, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.MissileLauncher, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Flamer, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Throwing, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Shield, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Sword, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Axe, BaseValue = 1.6f, StandardDeviation = 0.2f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Fist, BaseValue = 2.5f, StandardDeviation = 0.5f },
                new SkillTemplate { BaseSkill = TempBaseSkillList.Instance.Stealth, BaseValue = 0.5f, StandardDeviation = 1f }
            };
            SoldierTemplates = new List<SoldierTemplate>
            {
                new SoldierTemplate(0, "Space Marine", null, fifteen, fifteen, fifteen, intl, fifteen,
                pre, fifteen, psy, fifteen, mov, siz, skills, HumanBodyTemplate.Instance)
            }.ToDictionary(template => template.Id);
        }
    }
}
