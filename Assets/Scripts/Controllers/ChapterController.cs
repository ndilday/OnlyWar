using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class ChapterController : MonoBehaviour
    {
        public ChapterView ChapterView;
        public GameSettings GameSettings;
        Unit _chapter;
        Dictionary<int, Unit> _unitMap;
        SpaceMarine[] _marines;
        // Start is called before the first frame update
        void Start()
        {
            _unitMap = new Dictionary<int, Unit>();
            Debug.Log("Creating Chapter");
            CreateChapter();
            Debug.Log("Done creating Chapter");
        }

        private void CreateChapter()
        {
            _marines = SoldierFactory.Instance.GenerateNewSoldiers<SpaceMarine>(1000, TempSpaceMarineTemplate.Instance);
            foreach(SpaceMarine marine in _marines)
            {
                marine.EvaluateSoldier(new Date(40, (GameSettings.Year - 4), 1));
            }
           _chapter = NewChapterBuilder.AssignSoldiersToChapter(_marines, GameSettings.ChapterTemplate, new Date(40, (GameSettings.Year), 1).ToString());
        }

        public void OnChapterButtonClick()
        {
            ChapterView.gameObject.SetActive(true);
            if (!ChapterView.Initialized)
            {
                ChapterView.AddChapterHq(_chapter.Id, _chapter.Name + " HQ Squad");
                //_unitMap[_chapter.Id + 1000] = _chapter;
                _unitMap[_chapter.Id] = _chapter;
                foreach (Unit company in _chapter.ChildUnits)
                {
                    _unitMap[company.Id] = company;
                    if (company.ChildUnits == null || company.ChildUnits.Count == 0)
                    {
                        ChapterView.AddChapterHq(company.Id, company.Name);
                    }
                    else
                    {
                        List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
                        //squadList.Add(new Tuple<int, string>(company.Id + 1000, company.Name + " HQ Squad"));
                        //_unitMap[company.Id + 1000] = company;
                        foreach (Unit squad in company.ChildUnits)
                        {

                            squadList.Add(new Tuple<int, string>(squad.Id, squad.Name));
                            _unitMap[squad.Id] = squad;
                        }
                        ChapterView.AddCompany(company.Id, company.Name, squadList);
                    }
                }
                ChapterView.Initialized = true;
            }
        }

        public void UnitSelected(int unitId)
        {
            // populate view with members of selected unit
            Unit selectedUnit = _unitMap[unitId];
            List<Tuple<int, string, string>> memberList = selectedUnit.Members.Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
            ChapterView.ReplaceSquadMemberContent(memberList);
            ChapterView.ReplaceSelectedUnitText(GenerateUnitSummary(selectedUnit));
        }

        private string GenerateUnitSummary(Unit unit)
        {
            string unitReport = "";
            Dictionary<SpaceMarineRank, int> toe = new Dictionary<SpaceMarineRank, int>();
            var soldiers = unit.GetAllMembers().Select(s => (SpaceMarine)s);
            var rankCountMapActual = soldiers.GroupBy(s => s.Rank).ToDictionary(g => g.Key, g => g.Count());
            unit.UnitTemplate.AddRankCounts(toe);
            var orderedToe = toe.OrderByDescending(kvp => kvp.Key.IsOfficer).ThenByDescending(kvp => kvp.Key.Level);
            foreach (KeyValuePair<SpaceMarineRank, int> nominalSize in orderedToe)
            {
                int nominalCount = nominalSize.Value;
                int realCount = rankCountMapActual.ContainsKey(nominalSize.Key) ? rankCountMapActual[nominalSize.Key] : 0;
                unitReport += nominalSize.Key.Name + ": " + realCount.ToString() + " / " + nominalCount.ToString() + "\n";
            }
            return unitReport;
        }

        public void SoldierSelected(int soldierId)
        {
            string newText = "";
            Soldier soldier = _marines[soldierId];
            foreach(string historyLine in soldier.SoldierHistory)
            {
                newText += historyLine + "\n";
            }
            ChapterView.ReplaceSelectedUnitText(newText);
        }
    }
}