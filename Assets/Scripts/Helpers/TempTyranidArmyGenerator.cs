using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using System.Linq;

namespace Iam.Scripts.Helpers
{
    public sealed class TempTyranidArmyGenerator
    {
        public static Unit GenerateTyranidArmy()
        {
            Unit root = TempTyranidUnitTemplates.Instance.UnitTemplates[1000].GenerateUnitFromTemplateWithoutChildren(666, "Tyranid Challenge Force");
            foreach(Squad squad in root.Squads)
            {
                squad.IsInReserve = false;
                foreach(SquadTemplateElement element in squad.SquadTemplate.Elements)
                {
                    // this is cheat... the soldier type id and the template ids match
                    SoldierType type = element.AllowedSoldierTypes.First();
                    SoldierTemplate template = TempTyranidSoldierTemplates.Instance.SoldierTemplates[type.Id];
                    Soldier[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers(element.MaximumNumber, template);

                    squad.Members.AddRange(soldiers);
                    foreach(Soldier soldier in soldiers)
                    {
                        soldier.Type = type;
                        soldier.Name = $"{soldier.Type.Name} {soldier.Id}";
                    }
                }
            }
            return root;
        }
    }
}
