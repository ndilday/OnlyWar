using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class ChapterController : MonoBehaviour
    {
        public UnityEvent OnChapterCreated;
        public UnitTreeView UnitTreeView;
        public SquadMemberView SquadMemberView;
        public GameSettings GameSettings;
        Dictionary<int, Unit> _unitMap;
        SpaceMarine[] _marines;
        // Start is called before the first frame update
        void Start()
        {
            _unitMap = new Dictionary<int, Unit>();
            Debug.Log("Creating Chapter");
            CreateChapter();
            Debug.Log("Done creating Chapter");
            OnChapterCreated.Invoke();
        }

        public void ChapterButton_OnClick()
        {
            UnitTreeView.gameObject.SetActive(true);
            SquadMemberView.gameObject.SetActive(true);
            if (!UnitTreeView.Initialized)
            {
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.Id, GameSettings.Chapter.Name + " HQ Squad");
                //_unitMap[GameSettings.Chapter.Id + 1000] = GameSettings.Chapter;
                _unitMap[GameSettings.Chapter.Id] = GameSettings.Chapter;
                foreach (Unit company in GameSettings.Chapter.ChildUnits)
                {
                    _unitMap[company.Id] = company;
                    if (company.ChildUnits == null || company.ChildUnits.Count == 0)
                    {
                        UnitTreeView.AddLeafUnit(company.Id, company.Name);
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
                        UnitTreeView.AddTreeUnit(company.Id, company.Name, squadList);
                    }
                }
                UnitTreeView.Initialized = true;
            }
        }

        public void UnitSelected(int unitId)
        {
            // populate view with members of selected unit
            Unit selectedUnit = _unitMap[unitId];
            List<Tuple<int, string, string>> memberList = selectedUnit.Members.Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
            SquadMemberView.ReplaceSquadMemberContent(memberList);
            SquadMemberView.ReplaceSelectedUnitText(GenerateUnitSummary(selectedUnit));
        }

        public void SoldierSelected(int soldierId)
        {
            string newText = "";
            Soldier soldier = _marines[soldierId];
            foreach(string historyLine in soldier.SoldierHistory)
            {
                newText += historyLine + "\n";
            }
            SquadMemberView.ReplaceSelectedUnitText(newText);
        }

        private void CreateChapter()
        {
            Date basicTrainingEndDate = new Date(40, GameSettings.Year - 3, 52);
            Date trainingStartDate = new Date(40, GameSettings.Year - 4, 1);
            _marines = SoldierFactory.Instance.GenerateNewSoldiers<SpaceMarine>(1000, TempSpaceMarineTemplate.Instance);
            foreach (SpaceMarine marine in _marines)
            {
                marine.FirstName = TempNameGenerator.GetName();
                marine.LastName = TempNameGenerator.GetName();
                marine.SoldierHistory.Add(trainingStartDate + ": accepted into training");
                if (marine.PsychicPower > 0)
                {
                    marine.SoldierHistory.Add(trainingStartDate + ": psychic ability detected, acolyte training initiated");
                    // add psychic specific training here
                }
                EvaluateSoldier(marine, basicTrainingEndDate);
            }
            GameSettings.Chapter = NewChapterBuilder.AssignSoldiersToChapter(_marines, GameSettings.ChapterTemplate, new Date(40, (GameSettings.Year), 1).ToString());
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

        public void EvaluateSoldier(SpaceMarine marine, Date trainingFinishedYear)
        {
            SpaceMarineEvaluator.Instance.EvaluateMarine(marine);

            if (marine.MeleeScore > 700) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Adamantium Sword of the Emperor badge during training");
            else if (marine.MeleeScore > 600) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Sword of the Emperor badge during training");
            else if (marine.MeleeScore > 500) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Sword of the Emperor badge during training");
            else if (marine.MeleeScore > 400) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Sword of the Emperor badge during training");

            if (marine.RangedScore > 75) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Marksman badge during training with " + marine.GetBestRangedSkill().BaseSkill.Name);
            else if (marine.RangedScore > 65) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Marksman badge during training with " + marine.GetBestRangedSkill().BaseSkill.Name);
            else if (marine.RangedScore > 60) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Marksman badge during training with " + marine.GetBestRangedSkill().BaseSkill.Name);

            if (marine.LeadershipScore > 235) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Voice of the Emperor badge during training");
            else if (marine.LeadershipScore > 160) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Voice of the Emperor badge during training");
            else if (marine.LeadershipScore > 135) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Voice of the Emperor badge during training");

            if (marine.AncientScore > 72) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Banner of the Emperor badge during training");
            else if (marine.AncientScore > 65) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Banner of the Emperor badge during training");
            else if (marine.AncientScore > 57) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Banner of the Emperor badge during training");

            if (marine.MedicalScore > 100) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Flagged for potential training as Apothecary");

            if (marine.TechScore > 100) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Flagged for potential training as Techmarine");

            if (marine.PietyScore > 110) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Devout badge and declared a Novice");
        }
    }
}