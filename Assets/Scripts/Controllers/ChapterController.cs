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

namespace Iam.Scripts.Controllers
{
    public class ChapterController : ChapterUnitTreeController
    {
        public UnityEvent OnChapterCreated;

        [SerializeField]
        private UnitTreeView UnitTreeView;
        [SerializeField]
        private SquadMemberView SquadMemberView;
        [SerializeField]
        private GameSettings GameSettings;
        private SoldierTrainingHelper _trainingHelper;
        private PlayerSoldier _selectedSoldier;

        // Start is called before the first frame update
        void Start()
        {
            GameSettings.SquadMap = new Dictionary<int, Squad>();
            _trainingHelper = new SoldierTrainingHelper();
            GameSettings.PlayerSoldierMap = new Dictionary<int, PlayerSoldier>();
            CreateChapter();
            OnChapterCreated.Invoke();
        }

        public void ChapterButton_OnClick()
        {
            GameSettings.PlayerSoldierMap.Values.First().Body.HitLocations[0].Wounds.AddWound(WoundLevel.Unsurvivable);
            UnitTreeView.gameObject.SetActive(true);
            SquadMemberView.gameObject.SetActive(true);
            if (!UnitTreeView.Initialized)
            {
                BuildUnitTree(UnitTreeView, 
                              GameSettings.Chapter.OrderOfBattle,
                              GameSettings.PlayerSoldierMap,
                              GameSettings.SquadMap);
                UnitTreeView.Initialized = true;
            }
        }

        public void UnitTreeView_OnUnitSelected(int squadId)
        {
            // populate view with members of selected squad
            if (!GameSettings.SquadMap.ContainsKey(squadId))
            {
                UnitSelected(squadId);
            }
            else
            {
                Squad selectedSquad = GameSettings.SquadMap[squadId];
                List<Tuple<int, string, string, Color>> memberList = selectedSquad.Members
                    .Select(s => new Tuple<int, string, string, Color>(s.Id, s.Type.Name, s.Name, DetermineDisplayColor(s)))
                    .ToList();
                SquadMemberView.ReplaceSquadMemberContent(memberList);
                SquadMemberView.ReplaceSelectedUnitText(GenerateSquadSummary(selectedSquad));
            }
        }

        public void SquadMemberView_OnSoldierSelected(int soldierId)
        {
            string newText = "";
            _selectedSoldier = GameSettings.PlayerSoldierMap[soldierId];
            foreach(string historyLine in _selectedSoldier.SoldierHistory)
            {
                newText += historyLine + "\n";
            }
            SquadMemberView.ReplaceSelectedUnitText(newText);
            var openings = GetOpeningsInUnit(GameSettings.Chapter.OrderOfBattle, 
                                             _selectedSoldier.AssignedSquad,
                                             _selectedSoldier.Type);
            // insert current assignment at top
            openings.Insert(0, new Tuple<int, SoldierType, string>(
                _selectedSoldier.AssignedSquad.Id,
                _selectedSoldier.Type,
                $"{_selectedSoldier.Type.Name}, {_selectedSoldier.AssignedSquad.Name}, {_selectedSoldier.AssignedSquad.ParentUnit.Name}"));
            SquadMemberView.PopulateTransferDropdown(openings);
        }

        public void SquadMemberView_OnSoldierTransferred(Tuple<int, SoldierType, string> newPosition)
        {
            Squad currentSquad = _selectedSoldier.AssignedSquad;
            // move soldier to his new role
            _selectedSoldier.RemoveFromSquad();
            if(_selectedSoldier.Type.IsSquadLeader 
                && (currentSquad.SquadTemplate.SquadType & SquadTypes.HQ) == 0)
            {
                // if soldier is squad leader and its not an HQ Squad, change name
                currentSquad.Name = currentSquad.SquadTemplate.Name;
            }
            Squad newSquad = GameSettings.SquadMap[newPosition.Item1];
            _selectedSoldier.AssignToSquad(newSquad);
            if(_selectedSoldier.Type != newPosition.Item2)
            {
                string entry = $"{GameSettings.Date}: promoted to {newPosition.Item2.Name}";
                _selectedSoldier.AddEntryToHistory(entry);
                _selectedSoldier.Type = newPosition.Item2;
            }
            if(_selectedSoldier.Type.IsSquadLeader
                && (newSquad.SquadTemplate.SquadType & SquadTypes.HQ) == 0)
            {
                // if soldier is squad leader and its not an HQ Squad, change name
                newSquad.Name = _selectedSoldier.Name.Split(' ')[1] + " Squad";
            }
            if(currentSquad != newSquad)
            {
                string entry = $"{GameSettings.Date} transferred to {newSquad.Name}, {newSquad.ParentUnit.Name}";
                _selectedSoldier.AddEntryToHistory(entry);
            }
            
            // refresh the unit layout
            BuildUnitTree(UnitTreeView,
                              GameSettings.Chapter.OrderOfBattle,
                              GameSettings.PlayerSoldierMap,
                              GameSettings.SquadMap);
            UnitTreeView.UnitButton_OnClick(currentSquad.Id);
        }

        public void UIController_OnTurnEnd()
        {
            // make sure the dialog is closed
            UnitTreeView.gameObject.SetActive(false);

            // set the unit tree view to dirty as there may be casualties between turns
            UnitTreeView.Initialized = false;

            // handle work experience
            // "work" is worth 1/4 as much as training. 12 hrs/day, 7 days/week,
            // works out to 21 hours of training equivalent, call it 20, so 0.1 points
            foreach (PlayerSoldier soldier in GameSettings.PlayerSoldierMap.Values)
            {
                _trainingHelper.ApplySoldierWorkExperience(soldier, 0.1f);
            }
        }

        private void UnitSelected(int unitId)
        {
            Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == unitId);
            List<Tuple<int, string, string, Color>> memberList = selectedUnit.HQSquad.Members
                .Select(s => new Tuple<int, string, string, Color>(s.Id, s.Type.Name, s.Name, DetermineDisplayColor(s)))
                .ToList();
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
            List<Tuple<SquadTemplateElement, int, int>> toe = 
                new List<Tuple<SquadTemplateElement, int, int>>();
            foreach (Squad squad in unit.Squads)
            {
                toe.AddRange(GetSquadHeadcounts(squad));
            }
            var summedList = toe.GroupBy(tuple => tuple.Item1)
                    .Select(g => new Tuple<SquadTemplateElement, int, int>(g.Key, g.Sum(t => t.Item2), g.Sum(q => q.Item3)))
                    .OrderBy(tuple => tuple.Item1.SoldierType.Id);
            foreach(Tuple<SquadTemplateElement, int, int> tuple in summedList)
            {
                unitReport += $"{tuple.Item1.SoldierType.Name}: {tuple.Item2}/{tuple.Item3}\n";
            }
            return unitReport;
        }

        private string GenerateSquadSummary(Squad squad)
        {
            string alerts = "";
            string popReport = "";
            List<Tuple<SquadTemplateElement, int, int>> headcounts = GetSquadHeadcounts(squad);
            
            foreach (Tuple<SquadTemplateElement, int, int> tuple in headcounts)
            {
                if(tuple.Item3 < tuple.Item1.MinimumNumber)
                {
                    alerts += $"Insufficient healthy {tuple.Item1.SoldierType.Name} to field this squad.\n";
                }
                popReport += $"{tuple.Item1.SoldierType.Name}: {tuple.Item2}/{tuple.Item1.MaximumNumber}";
                if(tuple.Item3 != tuple.Item2)
                {
                    popReport += $" ({tuple.Item2 - tuple.Item3} unfit for duty)";
                }
                popReport += "\n";
            }
            
            return alerts + popReport;
        }

        private List<Tuple<SquadTemplateElement, int, int>> GetSquadHeadcounts(Squad squad)
        {
            List<Tuple<SquadTemplateElement, int, int>> entryList = 
                new List<Tuple<SquadTemplateElement, int, int>>();
            IEnumerable<ISoldier> soldiers = squad.Members;
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                // get the members of the squad that match this element
                var matches = soldiers?.Where(s => element.SoldierType == s.Type);
                var healthyMatches = matches?.Where(s => GameSettings.PlayerSoldierMap[s.Id].IsDeployable);
                int count = matches == null ? 0 : matches.Count();
                int healthyCount = healthyMatches == null ? 0 : healthyMatches.Count();
                entryList.Add(new Tuple<SquadTemplateElement, int, int>(element, count, healthyCount));
            }
            return entryList;
        }

        private void CreateChapter()
        {
            Date basicTrainingEndDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 3, 52);
            Date trainingStartDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 4, 1);
            var soldierTemplate = GameSettings.Galaxy.PlayerFaction.SoldierTemplates[0];
            GameSettings.PlayerSoldierMap = SoldierFactory.Instance.GenerateNewSoldiers(1000, soldierTemplate)
                .Select(s => new PlayerSoldier(s, $"{TempNameGenerator.GetName()} {TempNameGenerator.GetName()}"))
                .ToDictionary(m => m.Id);
            foreach (PlayerSoldier soldier in GameSettings.PlayerSoldierMap.Values)
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
            GameSettings.Chapter = 
                NewChapterBuilder.CreateChapter(GameSettings.PlayerSoldierMap.Values, 
                                                GameSettings.Galaxy.PlayerFaction, 
                                                new Date(GameSettings.Date.Millenium, 
                                                    (GameSettings.Date.Year), 1).ToString());
            PopulateSquadMap();
            GameSettings.ChapterPlanetId = 11;
        }

        private void PopulateSquadMap()
        {
            Unit oob = GameSettings.Chapter.OrderOfBattle;
            GameSettings.SquadMap[oob.HQSquad.Id] = oob.HQSquad;
            foreach(Squad squad in oob.Squads)
            {
                GameSettings.SquadMap[squad.Id] = squad;
            }
            foreach(Unit company in oob.ChildUnits)
            {
                if(company.HQSquad != null)
                {
                    GameSettings.SquadMap[company.HQSquad.Id] = company.HQSquad;
                }
                foreach(Squad squad in company.Squads)
                {
                    GameSettings.SquadMap[squad.Id] = squad;
                }
            }
        }

        private List<Tuple<int, SoldierType, string>> GetOpeningsInUnit(Unit unit, Squad currentSquad, SoldierType soldierType)
        {
            List<Tuple<int, SoldierType, string>> openSlots = 
                new List<Tuple<int, SoldierType, string>>();
            IEnumerable<SoldierType> squadTypes;
            if (unit.HQSquad != null)
            {
                squadTypes = GetOpeningsInSquad(unit.HQSquad, currentSquad, soldierType);
                if(squadTypes.Count() > 0)
                {
                    foreach(SoldierType type in squadTypes)
                    {
                        openSlots.Add(new Tuple<int, SoldierType, string>(unit.HQSquad.Id,type,
                            $"{type.Name}, {unit.HQSquad.Name}, {unit.Name}"));
                    }
                }
            }
            foreach(Squad squad in unit.Squads)
            {
                squadTypes = GetOpeningsInSquad(squad, currentSquad, soldierType);
                if (squadTypes.Count() > 0)
                {
                    foreach (SoldierType type in squadTypes)
                    {
                        openSlots.Add(new Tuple<int, SoldierType, string>(squad.Id, type,
                            $"{type.Name}, {squad.Name}, {unit.Name}"));
                    }
                }
            }
            foreach(Unit childUnit in unit.ChildUnits)
            {
                openSlots.AddRange(GetOpeningsInUnit(childUnit, currentSquad, soldierType));
            }
            return openSlots;
        }

        private IEnumerable<SoldierType> GetOpeningsInSquad(Squad squad, Squad currentSquad, SoldierType soldierType)
        {
            List<SoldierType> openTypes = new List<SoldierType>();
            bool hasSquadLeader = squad.SquadLeader != null;
            // get the count of each soldier type in the squad
            // compare to the max count of each type
            Dictionary<SoldierType, int> typeCountMap = 
                squad.Members.GroupBy(s => s.Type).ToDictionary(g => g.Key, g => g.Count());
            foreach(SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                // if the squad has no squad leader, only squad leader elements can be added now
                if(!hasSquadLeader && !element.SoldierType.IsSquadLeader)
                {
                    continue;
                }
                if(currentSquad == squad && element.SoldierType == soldierType)
                {
                    continue;
                }
                if(element.SoldierType.Rank < soldierType.Rank 
                    || element.SoldierType.Rank > soldierType.Rank + 1)
                {
                    continue;
                }
                int existingHeadcount = 0;
                if(typeCountMap.ContainsKey(element.SoldierType))
                {
                    existingHeadcount += typeCountMap[element.SoldierType];
                }
                if(existingHeadcount < element.MaximumNumber)
                {
                    openTypes.Add(element.SoldierType);
                }
            }
            return openTypes;
        }

        protected Color DetermineDisplayColor(ISoldier soldier)
        {
            PlayerSoldier playerSoldier = GameSettings.PlayerSoldierMap[soldier.Id];
            if (!playerSoldier.IsDeployable)
            {
                return Color.red;
            }
            if (playerSoldier.IsWounded)
            {
                return Color.yellow;
            }
            return Color.white;
        }
    }
}