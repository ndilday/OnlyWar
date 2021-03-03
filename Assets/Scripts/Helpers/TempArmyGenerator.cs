﻿using System.Linq;

using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;

namespace OnlyWar.Scripts.Helpers
{
    public sealed class TempArmyGenerator
    {
        public static Unit GenerateArmy(int armyId, Faction faction)
        {
            Unit root = faction.UnitTemplates.Values.Where(ut => ut.IsTopLevelUnit).ToList()[armyId]
                            .GenerateUnitFromTemplateWithoutChildren(faction.Name + " Force");
            if(root.HQSquad != null)
            {
                root.HQSquad.IsInReserve = false;
                foreach (SquadTemplateElement element in root.HQSquad.SquadTemplate.Elements)
                {
                    SoldierTemplate template = element.SoldierTemplate;
                    Soldier[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers(element.MaximumNumber, template);

                    foreach (Soldier soldier in soldiers)
                    {
                        root.HQSquad.AddSquadMember(soldier);
                        soldier.AssignedSquad = root.HQSquad;
                        soldier.Template = template;
                        soldier.Name = $"{soldier.Template.Name} {soldier.Id}";
                    }
                }
            }
            foreach(Squad squad in root.Squads)
            {
                squad.IsInReserve = false;
                foreach(SquadTemplateElement element in squad.SquadTemplate.Elements)
                {
                    SoldierTemplate template = element.SoldierTemplate;
                    Soldier[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers(element.MaximumNumber, template);

                    foreach(Soldier soldier in soldiers)
                    {
                        squad.AddSquadMember(soldier);
                        soldier.AssignedSquad = squad;
                        soldier.Template = template;
                        soldier.Name = $"{soldier.Template.Name} {soldier.Id}";
                    }
                }
            }
            return root;
        }
    }
}
