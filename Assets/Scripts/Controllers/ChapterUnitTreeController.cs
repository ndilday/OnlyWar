using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using OnlyWar.Models;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using OnlyWar.Views;

namespace OnlyWar.Controllers
{
    public class ChapterUnitTreeController : MonoBehaviour
    {
        protected void BuildUnitTree(UnitTreeView unitTreeView,
                                     Army army)
        {
            unitTreeView.ClearTree();
            
            if (army.OrderOfBattle.HQSquad != null)
            {
                unitTreeView.AddLeafSquad(army.OrderOfBattle.HQSquad.Id,
                                         army.OrderOfBattle.HQSquad.Name,
                                         DetermineDisplayColor(army.OrderOfBattle.HQSquad, army.PlayerSoldierMap));
                army.SquadMap[army.OrderOfBattle.HQSquad.Id] = army.OrderOfBattle.HQSquad;
            }

            foreach (Squad squad in army.OrderOfBattle.Squads)
            {
                if (army.OrderOfBattle.HQSquad == null || squad.Id != army.OrderOfBattle.HQSquad.Id)
                {
                    army.SquadMap[squad.Id] = squad;
                    unitTreeView.AddLeafSquad(squad.Id, squad.Name, DetermineDisplayColor(squad, army.PlayerSoldierMap));
                }
            }
            foreach (Unit company in army.OrderOfBattle.ChildUnits)
            {
                if (company.HQSquad != null)
                {
                    army.SquadMap[company.HQSquad.Id] = company.HQSquad;
                }
                if (company.Squads?.Count == 0)
                {
                    // this is unexpected, currently
                    Debug.Log("We have a company with no squads?");
                    unitTreeView.AddLeafSquad(company.HQSquad.Id, company.HQSquad.Name,
                                             DetermineDisplayColor(company.HQSquad, army.PlayerSoldierMap));
                }
                else
                {
                    List<Tuple<int, string, Color, int>> squadList = 
                        new List<Tuple<int, string, Color, int>>();
                    foreach (Squad squad in company.Squads)
                    {
                        if (company.HQSquad == null || squad != company.HQSquad)
                        {
                            squadList.Add(new Tuple<int, string, Color, int>(squad.Id, squad.Name,
                                                                             DetermineDisplayColor(squad, army.PlayerSoldierMap),
                                                                             -1));
                            army.SquadMap[squad.Id] = squad;
                        }
                    }
                    unitTreeView.AddTreeUnit(company.Id, company.Name,
                                             DetermineDisplayColor(company.HQSquad, army.PlayerSoldierMap), 
                                             -1, squadList);
                }
            }
        }

        protected Color DetermineDisplayColor(Squad squad, Dictionary<int, PlayerSoldier> soldierMap)
        {
            var deployables = squad.Members.Select(s => soldierMap[s.Id])
                                                        .Where(ps => ps.IsDeployable);
            var typeGroups = deployables.GroupBy(ps => ps.Template).ToDictionary(g => g.Key);
            bool isFull = true;
            // if any element has less than the minimum number, display red
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                if (typeGroups.ContainsKey(element.SoldierTemplate))
                {
                    if (typeGroups[element.SoldierTemplate].Count() < element.MinimumNumber)
                    {
                        return Color.red;
                    }
                    else if(typeGroups[element.SoldierTemplate].Count() < element.MaximumNumber)
                    {
                        isFull = false;
                    }
                }
                else
                {
                    return Color.red;
                }
            }
            if (deployables.Count() < squad.Members.Count)
            {
                return new Color(255, 200, 50);
            }
            return isFull ? Color.white : Color.yellow;
        }
    }
}
