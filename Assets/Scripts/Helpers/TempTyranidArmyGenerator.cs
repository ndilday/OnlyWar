using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Helpers
{
    public sealed class TempTyranidArmyGenerator
    {
        public static Unit GenerateTyranidArmy(int armyId)
        {
            Unit root = TempTyranidUnitTemplates.Instance.UnitTemplates[armyId].GenerateUnitFromTemplateWithoutChildren(666, "Tyranid Challenge Force");
            if(root.HQSquad != null)
            {
                root.HQSquad.IsInReserve = false;
                foreach (SquadTemplateElement element in root.HQSquad.SquadTemplate.Elements)
                {
                    // this is cheat... the soldier type id and the template ids match
                    SoldierType type = element.SoldierType;
                    SoldierTemplate template = TempTyranidSoldierTemplates.Instance.SoldierTemplates[type.Id];
                    Soldier[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers(element.MaximumNumber, template);

                    foreach (Soldier soldier in soldiers)
                    {
                        root.HQSquad.AddSquadMember(soldier);
                        soldier.Type = type;
                        soldier.Name = $"{soldier.Type.Name} {soldier.Id}";
                    }
                }
            }
            foreach(Squad squad in root.Squads)
            {
                squad.IsInReserve = false;
                foreach(SquadTemplateElement element in squad.SquadTemplate.Elements)
                {
                    // this is cheat... the soldier type id and the template ids match
                    SoldierType type = element.SoldierType;
                    SoldierTemplate template = TempTyranidSoldierTemplates.Instance.SoldierTemplates[type.Id];
                    Soldier[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers(element.MaximumNumber, template);

                    foreach(Soldier soldier in soldiers)
                    {
                        squad.AddSquadMember(soldier);
                        soldier.Type = type;
                        soldier.Name = $"{soldier.Type.Name} {soldier.Id}";
                    }
                }
            }
            return root;
        }
    }
}
