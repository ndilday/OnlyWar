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

        [SerializeField]
        private UnitTreeView UnitTreeView;
        [SerializeField]
        private SquadMemberView SquadMemberView;
        [SerializeField]
        private GameSettings GameSettings;
        private Dictionary<int, Squad> _squadMap;
        private Dictionary<int, SpaceMarine> _marineMap;
        private SpaceMarineTrainingHelper _trainingHelper;

        // Start is called before the first frame update
        void Start()
        {
            _squadMap = new Dictionary<int, Squad>();
            _trainingHelper = new SpaceMarineTrainingHelper();
            _marineMap = new Dictionary<int, SpaceMarine>();
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
                UnitTreeView.ClearTree();
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.OrderOfBattle.HQSquad.Id, GameSettings.Chapter.OrderOfBattle.HQSquad.Name);
                _squadMap[GameSettings.Chapter.OrderOfBattle.HQSquad.Id] = GameSettings.Chapter.OrderOfBattle.HQSquad;

                foreach(Squad squad in GameSettings.Chapter.OrderOfBattle.Squads)
                {
                    _squadMap[squad.Id] = squad;
                    UnitTreeView.AddLeafUnit(squad.Id, squad.Name);
                }
                foreach (Unit company in GameSettings.Chapter.OrderOfBattle.ChildUnits)
                {
                    _squadMap[company.HQSquad.Id] = company.HQSquad;
                    if (company.Squads?.Count == 0)
                    {
                        // this is unexpected, currently
                        Debug.Log("We have a company with no squads?");
                        UnitTreeView.AddLeafUnit(company.HQSquad.Id, company.HQSquad.Name);
                    }
                    else
                    {
                        List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
                        //squadList.Add(new Tuple<int, string>(company.Id + 1000, company.Name + " HQ Squad"));
                        //_squadMap[company.Id + 1000] = company;
                        foreach (Squad squad in company.Squads)
                        {

                            squadList.Add(new Tuple<int, string>(squad.Id, squad.Name));
                            _squadMap[squad.Id] = squad;
                        }
                        UnitTreeView.AddTreeUnit(company.Id, company.Name, squadList);
                    }
                }
                UnitTreeView.Initialized = true;
            }
        }

        public void SquadSelected(int squadId)
        {
            // populate view with members of selected squad
            if (!_squadMap.ContainsKey(squadId))
            {
                UnitSelected(squadId);
            }
            else
            {
                Squad selectedSquad = _squadMap[squadId];
                List<Tuple<int, string, string>> memberList = selectedSquad.GetAllMembers().Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
                SquadMemberView.ReplaceSquadMemberContent(memberList);
                SquadMemberView.ReplaceSelectedUnitText(GenerateSquadSummary(selectedSquad));
            }
        }

        public void SoldierSelected(int soldierId)
        {
            string newText = "";
            Soldier soldier = _marineMap[soldierId];
            foreach(string historyLine in soldier.SoldierHistory)
            {
                newText += historyLine + "\n";
            }
            SquadMemberView.ReplaceSelectedUnitText(newText);
        }

        public void EndTurnButton_OnClick()
        {
            // set the unit tree view to dirty as there may be casualties between turns
            UnitTreeView.Initialized = false;
            // handle work experience
            // "work" is worth 1/4 as much as training. 12 hrs/day, 7 days/week,
            // works out to 21 hours of training equivalent, call it 20, so 0.1 points
            foreach (SpaceMarine marine in _marineMap.Values)
            {
                _trainingHelper.ApplyMarineWorkExperience(marine, 0.1f);
            }
        }

        private void UnitSelected(int unitId)
        {
            Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == unitId);
            List<Tuple<int, string, string>> memberList = selectedUnit.HQSquad.GetAllMembers().Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
            SquadMemberView.ReplaceSquadMemberContent(memberList);
            SquadMemberView.ReplaceSelectedUnitText(GenerateUnitSummary(selectedUnit));
        }

        private string GenerateUnitSummary(Unit unit)
        {
            string unitReport = unit.Name + " Order of Battle\n\n";
            Dictionary<SpaceMarineRank, int> toe = new Dictionary<SpaceMarineRank, int>();
            if(unit.HQSquad != null)
            {
                if(unit.HQSquad.SquadLeader != null)
                {
                    unitReport += "Captain: " + unit.HQSquad.SquadLeader.ToString() + "\n";
                }
                else
                {
                    unitReport += "Needs a Captain Assigned!\n";
                }

                if(unit.Name != "Tenth Company")
                {
                    var marines = unit.HQSquad.Members.Select(m => (SpaceMarine)m);
                    SpaceMarine ancient = marines.FirstOrDefault(m => m.Rank == TempSpaceMarineRanks.Ancient || m.Rank == TempSpaceMarineRanks.ChapterAncient);
                    if(ancient != null)
                    {
                        unitReport += "Ancient: " + ancient.ToString() + "\n";
                    }
                    else
                    {
                        unitReport += "Needs an Ancient Assigned!\n";
                    }
                    SpaceMarine champion = marines.FirstOrDefault(m => m.Rank == TempSpaceMarineRanks.Champion || m.Rank == TempSpaceMarineRanks.ChapterChampion);
                    if (champion != null)
                    {
                        unitReport += "Champion: " + champion.ToString() + "\n";
                    }
                    else
                    {
                        unitReport += "Needs a Champion Assigned!\n";
                    }
                }
            }
            else
            {
                unitReport += "Entire HQ Missing\n";
            }
            unitReport += "\nCurrent Company size: " + unit.GetAllMembers().Count().ToString() + "\n\n";
            foreach(Squad squad in unit.Squads)
            {
                unitReport += GenerateSquadSummary(squad);
            }
            return unitReport;
        }

        private void CreateChapter()
        {
            Date basicTrainingEndDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 3, 52);
            Date trainingStartDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 4, 1);
            _marineMap = SoldierFactory.Instance.GenerateNewSoldiers<SpaceMarine>(1000, TempSpaceMarineTemplate.Instance)
                .ToDictionary(m => m.Id);
            foreach (SpaceMarine marine in _marineMap.Values)
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
                marine.ProgenoidImplantDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 1, RNG.GetIntBelowMax(1, 53));
            }
            GameSettings.Chapter = NewChapterBuilder.AssignSoldiersToChapter(_marineMap.Values, GameSettings.ChapterTemplate, 
                new Date(GameSettings.Date.Millenium, (GameSettings.Date.Year), 1).ToString());
        }

        private string GenerateSquadSummary(Squad squad)
        {
            int headCount = squad.GetAllMembers().Count();
            return squad.Name + ": " + headCount.ToString() + "/" + (squad.SquadTemplate.Members.Count + 1).ToString() + " Marines\n";
        }

        private void DetermineSquadOpenings()
        {
            // if a tech marine, can't be reassigned
            // if an apoc or chaplain, can be assigned to a company HQ Squad
            // should we allow specialists to take on "normal" leadership roles, like being captains?
            // for now, no
            // regular marines can only be sent to a company of lower number for a higher ranked position
            // regular marines can only be demoted as part of joining the first company
            // priority order for regular marines:
            // chapter master, captain, Chapter HQ, Company HQ, Squad Sgt
            // assume scout -> devestator -> assault -> tactical is the normal progression, and reserve -> battle
            Unit chapterRoot = GameSettings.Chapter.OrderOfBattle;
            if(chapterRoot.HQSquad.SquadLeader == null)
            {

            }
            foreach(Unit company in chapterRoot.ChildUnits)
            {
                if(company.HQSquad.SquadLeader == null)
                {

                }
            }
            //if(chapterRoot.HQSquad.Members.Any(s))
        }

        // TODO: this should probably live somewhere else
        private void EvaluateSoldier(SpaceMarine marine, Date trainingFinishedYear)
        {
            SpaceMarineEvaluator.Instance.EvaluateMarine(marine);

            if (marine.MeleeScore > 700) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Adamantium Sword of the Emperor badge during training");
            else if (marine.MeleeScore > 600) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Sword of the Emperor badge during training");
            else if (marine.MeleeScore > 500) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Sword of the Emperor badge during training");
            else if (marine.MeleeScore > 400) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Sword of the Emperor badge during training");

            if (marine.RangedScore > 75) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Marksman badge during training with " + marine.GetBestSkillByCategory(SkillCategory.Ranged).BaseSkill.Name);
            else if (marine.RangedScore > 65) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Marksman badge during training with " + marine.GetBestSkillByCategory(SkillCategory.Ranged).BaseSkill.Name);
            else if (marine.RangedScore > 60) marine.SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Marksman badge during training with " + marine.GetBestSkillByCategory(SkillCategory.Ranged).BaseSkill.Name);

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