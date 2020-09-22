using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Squads
{
    public sealed class TempTyranidSquadTemplates
    {
        private static TempTyranidSquadTemplates _instance;
        public static TempTyranidSquadTemplates Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempTyranidSquadTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, SquadTemplate> SquadTemplates { get; }

        private TempTyranidSquadTemplates()
        {
            SquadTemplates = new List<SquadTemplate>
            {
                CreateTyranidWarriorSquad(),
                CreateHormagauntSquad(),
                CreateTermagauntSquad(),
            }.ToDictionary(st => st.Id);
        }

        private SquadTemplate CreateTyranidWarriorSquad()
        {
            return new SquadTemplate(102, "Tyranid Warrior Squad", 
                                     TempTyranidWeaponSets.Instance.WeaponSets[1], 
                                     null, 
                                     TempTyranidEquippables.Instance.ArmorTemplates[203],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[102], 3, 9)
                                     }, SquadTypes.None);
        }
        private SquadTemplate CreateTermagauntSquad()
        {
            return new SquadTemplate(104, "Termagaunt Squad",
                                     TempTyranidWeaponSets.Instance.WeaponSets[2],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[201],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[104], 10, 30)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateHormagauntSquad()
        {
            return new SquadTemplate(105, "Hormagaunt Squad",
                                     TempTyranidWeaponSets.Instance.WeaponSets[101],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[201],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[105], 10, 30)
                                     }, SquadTypes.None);
        }
    }
}
