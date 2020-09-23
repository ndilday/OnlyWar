using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class ChapterUnitTreeController : MonoBehaviour
    {
        protected void BuildUnitTree(UnitTreeView unitTreeView,
                                     Unit chapterRoot,
                                     Dictionary<int, PlayerSoldier> soldierMap,
                                     Dictionary<int, Squad> squadMap)
        {
            unitTreeView.ClearTree();
            unitTreeView.AddLeafUnit(chapterRoot.HQSquad.Id,
                                     chapterRoot.HQSquad.Name,
                                     DetermineDisplayColor(chapterRoot.HQSquad, soldierMap));
            squadMap[chapterRoot.HQSquad.Id] = chapterRoot.HQSquad;

            foreach (Squad squad in chapterRoot.Squads)
            {
                squadMap[squad.Id] = squad;
                unitTreeView.AddLeafUnit(squad.Id, squad.Name, DetermineDisplayColor(squad, soldierMap));
            }
            foreach (Unit company in chapterRoot.ChildUnits)
            {
                squadMap[company.HQSquad.Id] = company.HQSquad;
                if (company.Squads?.Count == 0)
                {
                    // this is unexpected, currently
                    Debug.Log("We have a company with no squads?");
                    unitTreeView.AddLeafUnit(company.HQSquad.Id, company.HQSquad.Name,
                                             DetermineDisplayColor(company.HQSquad, soldierMap));
                }
                else
                {
                    List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
                    //squadList.Add(new Tuple<int, string>(company.Id + 1000, company.Name + " HQ Squad"));
                    //_squadMap[company.Id + 1000] = company;
                    foreach (Squad squad in company.Squads)
                    {

                        squadList.Add(new Tuple<int, string>(squad.Id, squad.Name));
                        squadMap[squad.Id] = squad;
                    }
                    unitTreeView.AddTreeUnit(company.Id, company.Name, squadList);
                }
            }
        }

        protected Color DetermineDisplayColor(Squad squad, Dictionary<int, PlayerSoldier> soldierMap)
        {
            var deployables = squad.Members.Select(s => soldierMap[s.Id])
                                                        .Where(ps => ps.IsDeployable);
            var typeGroups = deployables.GroupBy(ps => ps.Type).ToDictionary(g => g.Key);
            // if any element has less than the minimum number, display red
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                if (typeGroups.ContainsKey(element.SoldierType))
                {
                    if (typeGroups[element.SoldierType].Count() < element.MinimumNumber)
                    {
                        return Color.red;
                    }
                }
            }
            if (deployables.Count() < squad.Members.Count)
            {
                return Color.yellow;
            }
            return Color.white;
        }
    }
}
