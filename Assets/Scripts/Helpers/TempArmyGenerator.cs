using System.Linq;

using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;

namespace OnlyWar.Scripts.Helpers
{
    public sealed class TempArmyGenerator
    {
        private static int id = 0;
        public static Unit GenerateArmy(int armyId, Faction faction)
        {
            UnitTemplate template = faction.UnitTemplates.Values
                                                         .Where(ut => ut.IsTopLevelUnit)
                                                         .ToList()[armyId];
            Unit root = CreateUnit(template);
            return root;
        }

        private static Unit CreateUnit(UnitTemplate template)
        {
            Unit unit = template.GenerateUnitFromTemplateWithoutChildren(template.Name);
            foreach (UnitTemplate childUnit in template.GetChildUnits())
            {
                unit.ChildUnits.Add(CreateUnit(childUnit));
            }

            if (unit.HQSquad != null)
            {
                AddSquad(unit.HQSquad);
            }
            
            foreach(Squad squad in unit.Squads)
            {
                AddSquad(squad);
            }
            return unit;
        }

        private static void AddSquad(Squad squad)
        {
            squad.IsInReserve = false;
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                SoldierTemplate template = element.SoldierTemplate;
                Soldier[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers(element.MaximumNumber, template);

                foreach (Soldier soldier in soldiers)
                {
                    squad.AddSquadMember(soldier);
                    soldier.AssignedSquad = squad;
                    soldier.Template = template;
                    soldier.Name = $"{soldier.Template.Name} {soldier.Id}";
                }
            }
        }
    }
}
