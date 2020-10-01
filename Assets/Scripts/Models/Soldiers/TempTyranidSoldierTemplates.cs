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
                CreateTyrantSoldierTemplate(),
                CreateBroodlordSoldierTemplate(),
                CreatePrimeSoldierTemplate(),
                CreateTyranidWarriorSoldierTemplate(),
                CreateHormagauntSoldierTemplate(),
                CreateTermagauntSoldierTemplate(),
                CreateGenestealerSoldierTemplate()
            }.ToDictionary(st => st.Id);
        }

        private SoldierTemplate CreateTyrantSoldierTemplate()
        {
            AttributeTemplate con = new AttributeTemplate { BaseValue = 240, StandardDeviation = 24 };
            AttributeTemplate a24 = new AttributeTemplate { BaseValue = 24, StandardDeviation = 2.4f };
            AttributeTemplate a16 = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1.6f };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 9.001f, StandardDeviation = 0 };
            AttributeTemplate atk = new AttributeTemplate { BaseValue = 60, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 6.8f, StandardDeviation = .68f };
            return new SoldierTemplate(TempSoldierTypes.TYRANT, "Tyranid Warrior Prime",
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.TYRANT],
                                       a24, a16, a16, a16, a24, a24, con,
                                       a24, atk, mov, siz,
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
                                               BaseValue = 8,
                                               StandardDeviation = 0
                                           }
                                       },
                                       TyranidWarriorBodyTemplate.Instance);
        }

        private SoldierTemplate CreateBroodlordSoldierTemplate()
        {
            AttributeTemplate a12 = new AttributeTemplate { BaseValue = 12, StandardDeviation = 1 };
            AttributeTemplate a20 = new AttributeTemplate { BaseValue = 20, StandardDeviation = 2 };
            AttributeTemplate a24 = new AttributeTemplate { BaseValue = 24, StandardDeviation = 2 };
            AttributeTemplate con = new AttributeTemplate { BaseValue = 120, StandardDeviation = 12 };
            AttributeTemplate atk = new AttributeTemplate { BaseValue = 30, StandardDeviation = 0 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 8, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 3.06f, StandardDeviation = 0.1f };

            return new SoldierTemplate(TempSoldierTypes.BROODLORD, "Broodlord",
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.BROODLORD],
                                       a20, a20, a12, a12, a24, a24, con, a12, atk,
                                       mov, siz,
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

        private SoldierTemplate CreatePrimeSoldierTemplate()
        {
            AttributeTemplate con = new AttributeTemplate { BaseValue = 120, StandardDeviation = 10 };
            AttributeTemplate a16 = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            AttributeTemplate a20 = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            AttributeTemplate a14 = new AttributeTemplate { BaseValue = 14, StandardDeviation = 1 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 6.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 2.6f, StandardDeviation = 0 };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = 0, StandardDeviation = 0 };
            return new SoldierTemplate(TempSoldierTypes.PRIME, "Tyranid Warrior Prime",
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.PRIME],
                                       a20, a16, a14, a14, a20, a20, con,
                                       psy, a20, mov, siz,
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
                                               BaseValue = 8,
                                               StandardDeviation = 0
                                           }
                                       },
                                       TyranidWarriorBodyTemplate.Instance);
        }

        private SoldierTemplate CreateTyranidWarriorSoldierTemplate()
        {
            AttributeTemplate con = new AttributeTemplate { BaseValue = 48, StandardDeviation = 3 };
            AttributeTemplate sixteen = new AttributeTemplate { BaseValue = 16, StandardDeviation = 1 };
            AttributeTemplate twenty = new AttributeTemplate { BaseValue = 20, StandardDeviation = 1 };
            AttributeTemplate intl = new AttributeTemplate { BaseValue = 9, StandardDeviation = 1 };
            AttributeTemplate mov = new AttributeTemplate { BaseValue = 6.001f, StandardDeviation = 0 };
            AttributeTemplate siz = new AttributeTemplate { BaseValue = 2.6f, StandardDeviation = 0 };
            AttributeTemplate psy = new AttributeTemplate { BaseValue = 0, StandardDeviation = 0 };
            return new SoldierTemplate(TempSoldierTypes.WARRIOR, "Tyranid Warrior", 
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.WARRIOR],
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
            return new SoldierTemplate(TempSoldierTypes.HORMAGAUNT, "Hormagaunt", 
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.HORMAGAUNT],
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
            return new SoldierTemplate(TempSoldierTypes.TERMAGAUNT, "Termagaunt", 
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.TERMAGAUNT],
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
            return new SoldierTemplate(TempSoldierTypes.GENESTEALER, "Genestealer", 
                                       TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.GENESTEALER],
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
