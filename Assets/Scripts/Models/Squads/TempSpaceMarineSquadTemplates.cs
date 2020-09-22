using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Squads
{
    public sealed class TempSpaceMarineSquadTemplates
    {
        private static TempSpaceMarineSquadTemplates _instance;
        public static TempSpaceMarineSquadTemplates Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempSpaceMarineSquadTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, SquadTemplate> SquadTemplates { get; }

        private TempSpaceMarineSquadTemplates()
        {
            SquadTemplates = new List<SquadTemplate>
            {
                CreateArmory(),
                CreateLibrarius(),
                CreateApothecarion(),
                CreateReclusium(),
                CreateChapterHQSquad(),
                CreateCompanyHQSquad(),
                CreateVeteranCompanyHQSquad(),
                CreateVeteranSquad(),
                CreateTacticalSquad(),
                CreateAssaultSquad(),
                CreateDevastatorSquad(),
                CreateScoutHQSquad(),
                CreateScoutSquad(),
            }.ToDictionary(st => st.Id);
        }

        private SquadTemplate CreateVeteranSquad()
        {
            return new SquadTemplate(16, "Veteran Squad", 
                                     TempSpaceMarineWeaponSets.Instance.WeaponSets[1], 
                                     null, 
                                     TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13],
                                         1, 1),
                                         new SquadTemplateElement(TempSoldierTypes.Instance.SpaceMarineSoldierTypes[16],
                                         4, 9)
                                     }, SquadTypes.Elite);
        }

        private SquadTemplate CreateTacticalSquad()
        {
            List<WeaponSet> options1 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[2],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[3],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[4]
            };
            SquadWeaponOption uwo1 = new SquadWeaponOption("Specialty Weapons", 0, 1, options1);

            List<WeaponSet> options2 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[5],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[6],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[7],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[8],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[9]
            };
            SquadWeaponOption uwo2 = new SquadWeaponOption("Heavy Weapons", 0, 1, options2);

            List<SquadWeaponOption> options = new List<SquadWeaponOption>
            {
                uwo1,
                uwo2
            };

            return new SquadTemplate(19, "Tactical Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[0], 
                                     options, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13],1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[19], 4, 9)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateAssaultSquad()
        {
            List<WeaponSet> options1 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[2],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[10]
            };

            SquadWeaponOption uwo1 = new SquadWeaponOption("Specialty Weapons", 0, 2, options1);

            List<WeaponSet> options2 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[13]
            };
            SquadWeaponOption uwo2 = new SquadWeaponOption("Two-handed Sword", 0, 2, options2);

            List<SquadWeaponOption> options = new List<SquadWeaponOption>
            {
                uwo1,
                uwo2
            };

            return new SquadTemplate(20, "Assault Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[1],
                                     options, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[20], 4, 9)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateDevastatorSquad()
        {
            List<WeaponSet> options2 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[5],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[6],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[7],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[8],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[9]
            };
            SquadWeaponOption uwo2 = new SquadWeaponOption("Heavy Weapons", 0, 4, options2);

            List<SquadWeaponOption> options = new List<SquadWeaponOption>
            {
                uwo2
            };

            return new SquadTemplate(21, "Devastator Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[0],
                                     options, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[21], 4, 9)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateScoutSquad()
        {
            List<WeaponSet> options1 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[11],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[12]
            };
            SquadWeaponOption uwo1 = new SquadWeaponOption("Optional Armament", 0, 8, options1);

            List<WeaponSet> options2 = new List<WeaponSet>
            {
                TempSpaceMarineWeaponSets.Instance.WeaponSets[5],
                TempSpaceMarineWeaponSets.Instance.WeaponSets[7]
            };
            SquadWeaponOption uwo2 = new SquadWeaponOption("Heavy Weapons", 0, 1, options2);

            List<SquadWeaponOption> options = new List<SquadWeaponOption>
            {
                uwo1,
                uwo2
            };

            return new SquadTemplate(22, "Scout Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[0],
                                     options, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[13], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[22], 4, 9)
                                     }, SquadTypes.Scout);
        }

        private SquadTemplate CreateVeteranCompanyHQSquad()
        {
            return new SquadTemplate(14, "Veteran Company HQ Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[1],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[12], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[14], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[15], 1, 1)
                                     }, SquadTypes.HQ | SquadTypes.Elite);
        }

        private SquadTemplate CreateChapterHQSquad()
        {
            return new SquadTemplate(1, "Chapter HQ Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[1],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[1], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[14], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[15], 1, 1)
                                     }, SquadTypes.HQ);
        }

        private SquadTemplate CreateCompanyHQSquad()
        {
            return new SquadTemplate(12, "Company HQ Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[1],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[12], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[14], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[15], 1, 1)
                                     }, SquadTypes.HQ);
        }

        private SquadTemplate CreateScoutHQSquad()
        {
            return new SquadTemplate(4, "Scout Company HQ Squad", TempSpaceMarineWeaponSets.Instance.WeaponSets[1],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[12], 1, 1)
                                     }, SquadTypes.HQ);
        }

        private SquadTemplate CreateArmory()
        {
            return new SquadTemplate(3, "Armory", TempSpaceMarineWeaponSets.Instance.WeaponSets[0],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[3], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[4], 0, 20),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[5], 0, 50)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateLibrarius()
        {
            return new SquadTemplate(6, "Librarius", TempSpaceMarineWeaponSets.Instance.WeaponSets[0],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[6], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[7], 0, 10),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[8], 0, 10),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[9], 0, 10)
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateApothecarion()
        {
            return new SquadTemplate(2, "Apothecarion", TempSpaceMarineWeaponSets.Instance.WeaponSets[0],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[2], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[18], 0, 50),
                                     }, SquadTypes.None);
        }

        private SquadTemplate CreateReclusium()
        {
            return new SquadTemplate(10, "Reclusium", TempSpaceMarineWeaponSets.Instance.WeaponSets[0],
                                     null, TempSpaceMarineEquippables.Instance.ArmorTemplates[200],
                                     new List<SquadTemplateElement>
                                     {
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[10], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[11], 1, 1),
                                         new SquadTemplateElement(
                                             TempSoldierTypes.Instance.SpaceMarineSoldierTypes[17], 0, 50)
                                     }, SquadTypes.None);
        }
    }
}
