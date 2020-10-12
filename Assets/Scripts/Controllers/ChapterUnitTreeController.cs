using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;
using OnlyWar.Scripts.Views;

namespace OnlyWar.Scripts.Controllers
{
    public class ChapterUnitTreeController : MonoBehaviour
    {
        protected void BuildUnitTree(UnitTreeView unitTreeView,
                                     Unit chapterRoot,
                                     Dictionary<int, PlayerSoldier> soldierMap,
                                     Dictionary<int, Squad> squadMap)
        {
            unitTreeView.ClearTree();
            unitTreeView.AddLeafSquad(chapterRoot.HQSquad.Id,
                                     chapterRoot.HQSquad.Name,
                                     DetermineDisplayColor(chapterRoot.HQSquad, soldierMap));
            squadMap[chapterRoot.HQSquad.Id] = chapterRoot.HQSquad;

            foreach (Squad squad in chapterRoot.Squads)
            {
                squadMap[squad.Id] = squad;
                unitTreeView.AddLeafSquad(squad.Id, squad.Name, DetermineDisplayColor(squad, soldierMap));
            }
            foreach (Unit company in chapterRoot.ChildUnits)
            {
                squadMap[company.HQSquad.Id] = company.HQSquad;
                if (company.Squads?.Count == 0)
                {
                    // this is unexpected, currently
                    Debug.Log("We have a company with no squads?");
                    unitTreeView.AddLeafSquad(company.HQSquad.Id, company.HQSquad.Name,
                                             DetermineDisplayColor(company.HQSquad, soldierMap));
                }
                else
                {
                    List<Tuple<int, string, Color, int>> squadList = 
                        new List<Tuple<int, string, Color, int>>();
                    //squadList.Add(new Tuple<int, string>(company.Id + 1000, company.Name + " HQ Squad"));
                    //_squadMap[company.Id + 1000] = company;
                    foreach (Squad squad in company.Squads)
                    {

                        squadList.Add(new Tuple<int, string, Color, int>(squad.Id, squad.Name,
                                                                         DetermineDisplayColor(squad, soldierMap),
                                                                         -1));
                        squadMap[squad.Id] = squad;
                    }
                    unitTreeView.AddTreeUnit(company.Id, company.Name,
                                             DetermineDisplayColor(company.HQSquad, soldierMap), 
                                             -1, squadList);
                }
            }
        }

        protected Color DetermineDisplayColor(Squad squad, Dictionary<int, PlayerSoldier> soldierMap)
        {
            var deployables = squad.Members.Select(s => soldierMap[s.Id])
                                                        .Where(ps => ps.IsDeployable);
            var typeGroups = deployables.GroupBy(ps => ps.Type).ToDictionary(g => g.Key);
            bool isFull = true;
            // if any element has less than the minimum number, display red
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                if (typeGroups.ContainsKey(element.SoldierType))
                {
                    if (typeGroups[element.SoldierType].Count() < element.MinimumNumber)
                    {
                        return Color.red;
                    }
                    else if(typeGroups[element.SoldierType].Count() < element.MaximumNumber)
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
