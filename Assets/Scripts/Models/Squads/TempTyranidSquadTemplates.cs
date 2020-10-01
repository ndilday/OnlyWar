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
                CreateTyrantSquad(),
                CreatePrimeSquad(),
                CreateBroodlordSquad(),
                CreateTyranidWarriorSquad(),
                CreateGenestealerSquad(),
                CreateHormagauntSquad(),
                CreateTermagauntSquad(),
            }.ToDictionary(st => st.Id);
        }

        private SquadTemplate CreateTyrantSquad()
        {
            return new SquadTemplate(TempSoldierTypes.TYRANT, "Hive Tyrant",
                                     TempTyranidWeaponSets.Instance.WeaponSets[105],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[204],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.TYRANT], 1, 1)
                                     }, SquadTypes.HQ);
        }

        private SquadTemplate CreatePrimeSquad()
        {
            return new SquadTemplate(TempSoldierTypes.PRIME, "Tyranid Prime",
                                     TempTyranidWeaponSets.Instance.WeaponSets[1],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[204],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.PRIME], 1, 1)
                                     }, SquadTypes.HQ);
        }

        private SquadTemplate CreateBroodlordSquad()
        {
            return new SquadTemplate(TempSoldierTypes.BROODLORD, "Broodlord",
                                     TempTyranidWeaponSets.Instance.WeaponSets[104],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[203],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.BROODLORD], 1, 1)
                                     }, SquadTypes.HQ);
        }

        private SquadTemplate CreateGenestealerSquad()
        {
            return new SquadTemplate(TempSoldierTypes.GENESTEALER, "Genestealer Squad",
                                     TempTyranidWeaponSets.Instance.WeaponSets[102],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[202],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.GENESTEALER], 5, 20)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateTyranidWarriorSquad()
        {
            return new SquadTemplate(TempSoldierTypes.WARRIOR, "Tyranid Warrior Squad", 
                                     TempTyranidWeaponSets.Instance.WeaponSets[1], 
                                     null, 
                                     TempTyranidEquippables.Instance.ArmorTemplates[203],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.WARRIOR], 3, 9)
                                     }, SquadTypes.None);
        }
        private SquadTemplate CreateTermagauntSquad()
        {
            return new SquadTemplate(TempSoldierTypes.TERMAGAUNT, "Termagaunt Squad",
                                     TempTyranidWeaponSets.Instance.WeaponSets[2],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[201],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.TERMAGAUNT], 10, 30)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateHormagauntSquad()
        {
            return new SquadTemplate(TempSoldierTypes.HORMAGAUNT, "Hormagaunt Squad",
                                     TempTyranidWeaponSets.Instance.WeaponSets[101],
                                     null,
                                     TempTyranidEquippables.Instance.ArmorTemplates[201],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.TyranidSoldierTypes[TempSoldierTypes.HORMAGAUNT], 10, 30)
                                     }, SquadTypes.None);
        }
    }
}
