using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Soldiers
{
    class TempTyranidSoldierTemplates
    {
        private static TempTyranidSoldierTemplates _instance = null;

        public static TempTyranidSoldierTemplates Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempTyranidSoldierTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, SoldierTemplate> SoldierTemplates { get; }

        private TempTyranidSoldierTemplates()
        {
            SoldierTemplates = new List<SoldierTemplate>
            {
                CreateTyranidWarriorSoldierTemplate(),
                CreateHormagauntSoldierTemplate(),
                CreateTermagauntSoldierTemplate(),
                CreateGenestealerSoldierTemplate()
            }.ToDictionary(st => st.Id);
        }

        private SoldierTemplate CreateTyranidWarriorSoldierTemplate()
        {
            AttributeTemplate con = new AttributeTemplate { BaseValue = 48, StandardDeviation = 3 };
            AttributeTemplate sixteen = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            AttributeTemplate twenty = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            AttributeTemplate intl = new AttributeTemplate { BaseValue = 9, StandardDeviation = 1 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 8.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 1.0f, StandardDeviation = 0 };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = 0, StandardDeviation = 0 };
            return new SoldierTemplate(102, "Tyranid Warrior", TempSoldierTypes.Instance.TyranidSoldierTypes[102],
                                       sixteen, sixteen, sixteen, intl, twenty, twenty, con,
                                       psy, twenty, mov, siz,
                                       new List<SkillTemplate>
                                       {
                                           new SkillTemplate 
                                           { 
                                               BaseSkill = TempBaseSkillList.Instance.OpponentRanged, 
                                               BaseValue = 1, 
                                               StandardDeviation = 0 
                                           },
                                           new SkillTemplate 
                                           { 
                                               BaseSkill = TempBaseSkillList.Instance.OpponentMelee, 
                                               BaseValue = 1, 
                                               StandardDeviation = 0 
                                           }
                                       },
                                       TyranidWarriorBodyTemplate.Instance);
        }

        private SoldierTemplate CreateHormagauntSoldierTemplate()
        {
            AttributeTemplate eight = new AttributeTemplate { BaseValue = 8, StandardDeviation = 1 };
            AttributeTemplate twelve = new AttributeTemplate { BaseValue = 12, StandardDeviation = 1 };
            AttributeTemplate twenty = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 8.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 1f, StandardDeviation = 0 };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = 0, StandardDeviation = 0 };
            return new SoldierTemplate(105, "Hormagaunt", TempSoldierTypes.Instance.TyranidSoldierTypes[105],
                                       twelve, twelve, twelve, eight, eight, eight, twelve,
                                       psy, twenty, mov, siz,
                                       new List<SkillTemplate>
                                       {
                                           new SkillTemplate
                                           {
                                               BaseSkill = TempBaseSkillList.Instance.OpponentMelee,
                                               BaseValue = 1,
                                               StandardDeviation = 0
                                           }
                                       },
                                       TyranidWarriorBodyTemplate.Instance);
        }

        private SoldierTemplate CreateTermagauntSoldierTemplate()
        {
            AttributeTemplate eight = new AttributeTemplate { BaseValue = 8, StandardDeviation = 1 };
            AttributeTemplate twelve = new AttributeTemplate { BaseValue = 12, StandardDeviation = 1 };
            AttributeTemplate atk = new AttributeTemplate { BaseValue = 10, StandardDeviation = 1 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 6.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 1f, StandardDeviation = 0 };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = 0, StandardDeviation = 0 };
            return new SoldierTemplate(104, "Termagaunt", TempSoldierTypes.Instance.TyranidSoldierTypes[104],
                                       twelve, twelve, twelve, eight, eight, eight, twelve,
                                       psy, atk, mov, siz,
                                       new List<SkillTemplate>
                                       {
                                           new SkillTemplate
                                           {
                                               BaseSkill = TempBaseSkillList.Instance.OpponentRanged,
                                               BaseValue = 1,
                                               StandardDeviation = 0
                                           },
                                           new SkillTemplate
                                           {
                                               BaseSkill = TempBaseSkillList.Instance.OpponentMelee,
                                               BaseValue = 1,
                                               StandardDeviation = 0
                                           }
                                       },
                                       TyranidWarriorBodyTemplate.Instance);
        }

        private SoldierTemplate CreateGenestealerSoldierTemplate()
        {
            AttributeTemplate eight = new AttributeTemplate { BaseValue = 8, StandardDeviation = 1 };
            AttributeTemplate sixteen = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            AttributeTemplate atk = new AttributeTemplate { BaseValue = 30, StandardDeviation = 1 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 8.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 2.08f, StandardDeviation = 0 };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = 0, StandardDeviation = 0 };
            return new SoldierTemplate(103, "Genestealer", TempSoldierTypes.Instance.TyranidSoldierTypes[103],
                                       sixteen, sixteen, sixteen, eight, sixteen, sixteen, sixteen,
                                       psy, atk, mov, siz,
                                       new List<SkillTemplate>
                                       {
                                           new SkillTemplate
                                           {
                                               BaseSkill = TempBaseSkillList.Instance.OpponentMelee,
                                               BaseValue = 1,
                                               StandardDeviation = 0
                                           }
                                       },
                                       TyranidWarriorBodyTemplate.Instance);
        }
    }
}
