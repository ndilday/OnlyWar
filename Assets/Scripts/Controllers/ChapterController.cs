using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;
using TMPro;

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
        private Dictionary<int, PlayerSoldier> _playerSoldierMap;
        private SoldierTrainingHelper _trainingHelper;
        private PlayerSoldier _selectedSoldier;

        // Start is called before the first frame update
        void Start()
        {
            _squadMap = new Dictionary<int, Squad>();
            _trainingHelper = new SoldierTrainingHelper();
            _playerSoldierMap = new Dictionary<int, PlayerSoldier>();
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

        public void UnitTreeView_OnUnitSelected(int squadId)
        {
            // populate view with members of selected squad
            if (!_squadMap.ContainsKey(squadId))
            {
                UnitSelected(squadId);
            }
            else
            {
                Squad selectedSquad = _squadMap[squadId];
                List<Tuple<int, string, string>> memberList = selectedSquad.Members
                    .Select(s => new Tuple<int, string, string>(s.Id, s.Type.Name, s.Name)).ToList();
                SquadMemberView.ReplaceSquadMemberContent(memberList);
                SquadMemberView.ReplaceSelectedUnitText(GenerateSquadSummary(selectedSquad));
            }
        }

        public void SquadMemberView_OnSoldierSelected(int soldierId)
        {
            string newText = "";
            _selectedSoldier = _playerSoldierMap[soldierId];
            foreach(string historyLine in _selectedSoldier.SoldierHistory)
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
            foreach (PlayerSoldier soldier in _playerSoldierMap.Values)
            {
                _trainingHelper.ApplySoldierWorkExperience(soldier, 0.1f);
            }
        }

        private void UnitSelected(int unitId)
        {
            Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == unitId);
            List<Tuple<int, string, string>> memberList = selectedUnit.HQSquad.Members
                .Select(s => new Tuple<int, string, string>(s.Id, s.Type.Name, s.Name)).ToList();
            SquadMemberView.ReplaceSquadMemberContent(memberList);
            SquadMemberView.ReplaceSelectedUnitText(GenerateUnitSummary(selectedUnit));
        }

        private string GenerateUnitSummary(Unit unit)
        {
            string unitReport = unit.Name + " Order of Battle\n\n";
            if (unit.HQSquad != null)
            {
                unitReport += GenerateSquadSummary(unit.HQSquad);
            }
            else
            {
                unitReport += "Entire HQ Missing\n";
            }
            unitReport += "\nCurrent Company size: " + unit.GetAllMembers().Count().ToString() + "\n\n";
            List<Tuple<SoldierType, int, int>> toe = new List<Tuple<SoldierType, int, int>>();
            foreach (Squad squad in unit.Squads)
            {
                toe.AddRange(GetSquadHeadcounts(squad));
            }
            var summedList = toe.GroupBy(tuple => tuple.Item1)
                    .Select(g => new Tuple<SoldierType, int, int>(g.Key, g.Sum(t => t.Item2), g.Sum(q => q.Item3)))
                    .OrderBy(tuple => tuple.Item1.Id);
            foreach(Tuple<SoldierType, int, int> tuple in summedList)
            {
                unitReport += $"{tuple.Item1.Name}: {tuple.Item2}/{tuple.Item3}\n";
            }
            return unitReport;
        }

        private string GenerateSquadSummary(Squad squad)
        {
            string report = "";
            foreach(Tuple<SoldierType, int, int> tuple in GetSquadHeadcounts(squad))
            {
                report += $"{tuple.Item1.Name}: {tuple.Item2}/{tuple.Item3}\n";
            }
            
            return report;
        }

        private List<Tuple<SoldierType, int, int>> GetSquadHeadcounts(Squad squad)
        {
            List<Tuple<SoldierType, int, int>> entryList = new List<Tuple<SoldierType, int, int>>();
            IEnumerable<ISoldier> soldiers = squad.Members;
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                // get the members of the squad that match this element
                var matches = soldiers.Where(s => element.AllowedSoldierTypes.Contains(s.Type));
                int count = matches == null ? 0 : matches.Count();
                entryList.Add(new Tuple<SoldierType, int, int>(element.AllowedSoldierTypes.First(), count, element.MaximumNumber));
            }
            return entryList;
        }

        private void CreateChapter()
        {
            Date basicTrainingEndDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 3, 52);
            Date trainingStartDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 4, 1);
            _playerSoldierMap = SoldierFactory.Instance.GenerateNewSoldiers(1000, TempSpaceMarineSoldierTemplate.Instance.SoldierTemplates[0])
                .Select(s => new PlayerSoldier(s, $"{TempNameGenerator.GetName()} {TempNameGenerator.GetName()}"))
                .ToDictionary(m => m.Id);
            foreach (PlayerSoldier soldier in _playerSoldierMap.Values)
            {
                soldier.AddEntryToHistory(trainingStartDate + ": accepted into training");
                if (soldier.PsychicPower > 0)
                {
                    soldier.AddEntryToHistory(trainingStartDate + ": psychic ability detected, acolyte training initiated");
                    // add psychic specific training here
                }
                _trainingHelper.EvaluateSoldier(soldier, basicTrainingEndDate);
                //soldier.ProgenoidImplantDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 1, RNG.GetIntBelowMax(1, 53));
            }
            GameSettings.Chapter = NewChapterBuilder.AssignSoldiersToChapter(_playerSoldierMap.Values, GameSettings.ChapterTemplate, 
                new Date(GameSettings.Date.Millenium, (GameSettings.Date.Year), 1).ToString());
        }

        private List<Tuple<int, int, SoldierType>> AddOpeningsInUnit(Unit unit)
        {
            List<Tuple<int, int, SoldierType>> openSlots = new List<Tuple<int, int, SoldierType>>();
            IEnumerable<SoldierType> squadTypes;
            if (unit.HQSquad != null)
            {
                squadTypes = GetOpeningsInSquad(unit.HQSquad);
                if(squadTypes.Count() > 0)
                {
                    foreach(SoldierType type in squadTypes)
                    {
                        openSlots.Add(new Tuple<int, int, SoldierType>(unit.Id, unit.HQSquad.Id,type));
                    }
                }
            }
            foreach(Squad squad in unit.Squads)
            {
                squadTypes = GetOpeningsInSquad(unit.HQSquad);
                if (squadTypes.Count() > 0)
                {
                    foreach (SoldierType type in squadTypes)
                    {
                        openSlots.Add(new Tuple<int, int, SoldierType>(unit.Id, unit.HQSquad.Id, type));
                    }
                }
            }
            foreach(Unit childUnit in unit.ChildUnits)
            {
                openSlots.AddRange(AddOpeningsInUnit(childUnit));
            }
            return openSlots;
        }

        private IEnumerable<SoldierType> GetOpeningsInSquad(Squad squad)
        {
            List<SoldierType> openTypes = new List<SoldierType>();
            // get the count of each soldier type in the squad
            // compare to the max count of each type
            Dictionary<SoldierType, int> typeCountMap = 
                squad.Members.GroupBy(s => s.Type).ToDictionary(g => g.Key, g => g.Count());
            foreach(SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                int existingHeadcount = 0;
                foreach(SoldierType type in element.AllowedSoldierTypes)
                {
                    if(typeCountMap.ContainsKey(type))
                    {
                        existingHeadcount += typeCountMap[type];
                    }
                }
                if(existingHeadcount < element.MaximumNumber)
                {
                    openTypes.AddRange(element.AllowedSoldierTypes);
                }
            }
            return openTypes;
        }
    }
}