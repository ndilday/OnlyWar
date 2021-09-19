using OnlyWar.Helpers;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using OnlyWar.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Controllers
{
    public class ChapterController : ChapterUnitTreeController
    {
        [SerializeField]
        private UnitTreeView UnitTreeView;
        [SerializeField]
        private SquadMemberView SquadMemberView;
        [SerializeField]
        private GameSettings GameSettings;
        private SoldierTrainingCalculator _trainingHelper;
        private PlayerSoldier _selectedSoldier;

        // Start is called before the first frame update
        void Start()
        {
            _trainingHelper = new SoldierTrainingCalculator(GameSettings.Galaxy.BaseSkillMap.Values);            
        }

        public void ChapterButton_OnClick()
        {
            UnitTreeView.gameObject.SetActive(true);
            SquadMemberView.gameObject.SetActive(true);
            if (!UnitTreeView.Initialized)
            {
                BuildUnitTree(UnitTreeView, 
                              GameSettings.Chapter.OrderOfBattle,
                              GameSettings.Chapter.PlayerSoldierMap,
                              GameSettings.Chapter.SquadMap);
                UnitTreeView.Initialized = true;
            }
        }

        public void UnitTreeView_OnUnitSelected(int unitId)
        {
            Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == unitId);
            List<Tuple<int, string, string, Color>> memberList = selectedUnit.HQSquad.Members
                .OrderByDescending(s => s.Template.Rank)
                .Select(s => new Tuple<int, string, string, Color>(s.Id, s.Template.Name, s.Name, DetermineDisplayColor(s)))
                .ToList();
            SquadMemberView.ReplaceSquadMemberContent(memberList);
            SquadMemberView.ReplaceSelectedUnitText(GenerateUnitSummary(selectedUnit));
        }

        public void UnitTreeView_OnSquadSelected(int squadId)
        {
            Squad selectedSquad = GameSettings.Chapter.SquadMap[squadId];
            List<Tuple<int, string, string, Color>> memberList = selectedSquad.Members
                .OrderByDescending(s => s.Template.Rank)
                .Select(s => new Tuple<int, string, string, Color>(s.Id, s.Template.Name, s.Name, DetermineDisplayColor(s)))
                .ToList();
            SquadMemberView.ReplaceSquadMemberContent(memberList);
            SquadMemberView.ReplaceSelectedUnitText(GenerateSquadSummary(selectedSquad));
        }

        public void SquadMemberView_OnSoldierSelected(int soldierId)
        {
            string newText = "";
            _selectedSoldier = GameSettings.Chapter. PlayerSoldierMap[soldierId];
            foreach(string historyLine in _selectedSoldier.SoldierHistory)
            {
                newText += historyLine + "\n";
            }
            SquadMemberView.ReplaceSelectedUnitText(newText);
            var openings = GetOpeningsInUnit(GameSettings.Chapter.OrderOfBattle, 
                                             _selectedSoldier.AssignedSquad,
                                             _selectedSoldier.Template);
            // insert current assignment at top
            openings.Insert(0, new Tuple<int, SoldierTemplate, string>(
                _selectedSoldier.AssignedSquad.Id,
                _selectedSoldier.Template,
                $"{_selectedSoldier.Template.Name}, {_selectedSoldier.AssignedSquad.Name}, {_selectedSoldier.AssignedSquad.ParentUnit.Name}"));
            SquadMemberView.PopulateTransferDropdown(openings);
        }

        public void SquadMemberView_OnSoldierTransferred(Tuple<int, SoldierTemplate, string> newPosition)
        {
            Squad currentSquad = _selectedSoldier.AssignedSquad;
            // move soldier to his new role
            _selectedSoldier.AssignedSquad = null;
            currentSquad.RemoveSquadMember(_selectedSoldier);
            if(_selectedSoldier.Template.IsSquadLeader 
                && (currentSquad.SquadTemplate.SquadType & SquadTypes.HQ) == 0)
            {
                // if soldier is squad leader and its not an HQ Squad, change name
                currentSquad.Name = currentSquad.SquadTemplate.Name;
            }
            Squad newSquad = GameSettings.Chapter.SquadMap[newPosition.Item1];
            _selectedSoldier.AssignedSquad = newSquad;
            newSquad.AddSquadMember(_selectedSoldier);

            UpdateSquadLocations(currentSquad, newSquad);

            if (_selectedSoldier.Template != newPosition.Item2)
            {
                string entry = $"{GameSettings.Date}: promoted to {newPosition.Item2.Name}";
                _selectedSoldier.AddEntryToHistory(entry);
                _selectedSoldier.Template = newPosition.Item2;
            }
            if(_selectedSoldier.Template.IsSquadLeader
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
                              GameSettings.Chapter.PlayerSoldierMap,
                              GameSettings.Chapter.SquadMap);
            if (newSquad.ParentUnit.HQSquad != null
                && newSquad.Id == newSquad.ParentUnit.HQSquad.Id)
            {
                UnitTreeView.UnitButton_OnClick(newSquad.Id);
            }
            else
            {
                UnitTreeView.ExpandUnit(newSquad.ParentUnit.Id);
                UnitTreeView.SquadButton_OnClick(newSquad.Id);
            }
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
            foreach (PlayerSoldier soldier in GameSettings.Chapter.PlayerSoldierMap.Values)
            {
                _trainingHelper.ApplySoldierWorkExperience(soldier, 0.1f);
            }
        }

        private string GenerateUnitSummary(Unit unit)
        {
            string unitReport = unit.Name + " Order of Battle\n\n";
            unitReport += "\nCurrent Company size: " + unit.GetAllMembers().Count().ToString() + "\n\n";
            List<Tuple<SquadTemplateElement, int, int>> toe = 
                new List<Tuple<SquadTemplateElement, int, int>>();
            foreach (Squad squad in unit.Squads)
            {
                toe.AddRange(GetSquadHeadcounts(squad));
            }
            var summedList = toe.GroupBy(tuple => tuple.Item1)
                    .Select(g => new Tuple<SquadTemplateElement, int, int, int>
                        (g.Key, 
                        g.Sum(t => t.Item2), 
                        g.Sum(t => t.Item1.MaximumNumber), 
                        g.Sum(q => q.Item3)))
                    .OrderByDescending(tuple => tuple.Item1.SoldierTemplate.Rank);
            foreach(Tuple<SquadTemplateElement, int, int, int> tuple in summedList)
            {
                unitReport += $"{tuple.Item1.SoldierTemplate.Name}: {tuple.Item2}/{tuple.Item3} ({tuple.Item4} healthy)\n";
            }
            return unitReport;
        }

        private string GenerateSquadSummary(Squad squad)
        {
            string location;
            string alerts = "";
            string popReport = "";

            if(squad.Location != null)
            {
                location = $"Location: {squad.Location.Name}\n\n";
            }
            else if(squad.BoardedLocation != null)
            {
                location = $"Location: On board {squad.BoardedLocation.Name}\n\n";
            }
            else
            {
                location = "Currently Unformed\n\n";
            }
            List<Tuple<SquadTemplateElement, int, int>> headcounts = GetSquadHeadcounts(squad);
            
            foreach (Tuple<SquadTemplateElement, int, int> tuple in headcounts)
            {
                if(tuple.Item3 < tuple.Item1.MinimumNumber)
                {
                    alerts += $"Insufficient healthy {tuple.Item1.SoldierTemplate.Name} to field this squad.\n";
                }
                popReport += $"{tuple.Item1.SoldierTemplate.Name}: {tuple.Item2}/{tuple.Item1.MaximumNumber}";
                if(tuple.Item3 != tuple.Item2)
                {
                    popReport += $" ({tuple.Item2 - tuple.Item3} unfit for duty)";
                }
                popReport += "\n";
            }
            
            return location + alerts + popReport;
        }

        private List<Tuple<SquadTemplateElement, int, int>> GetSquadHeadcounts(Squad squad)
        {
            List<Tuple<SquadTemplateElement, int, int>> entryList = 
                new List<Tuple<SquadTemplateElement, int, int>>();
            IEnumerable<ISoldier> soldiers = squad.Members;
            foreach (SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                // get the members of the squad that match this element
                var matches = soldiers?.Where(s => element.SoldierTemplate == s.Template);
                var healthyMatches = matches?.Where(s => GameSettings.Chapter.PlayerSoldierMap[s.Id].IsDeployable);
                int count = matches == null ? 0 : matches.Count();
                int healthyCount = healthyMatches == null ? 0 : healthyMatches.Count();
                entryList.Add(new Tuple<SquadTemplateElement, int, int>(element, count, healthyCount));
            }
            return entryList;
        }

        private List<Tuple<int, SoldierTemplate, string>> GetOpeningsInUnit(Unit unit, Squad currentSquad, 
                                                                            SoldierTemplate soldierTemplate)
        {
            List<Tuple<int, SoldierTemplate, string>> openSlots = 
                new List<Tuple<int, SoldierTemplate, string>>();
            IEnumerable<SoldierTemplate> squadSlots;
            foreach(Squad squad in unit.Squads)
            {
                squadSlots = GetOpeningsInSquad(squad, currentSquad, soldierTemplate);
                if (squadSlots.Count() > 0)
                {
                    foreach (SoldierTemplate template in squadSlots)
                    {
                        openSlots.Add(new Tuple<int, SoldierTemplate, string>(squad.Id, template,
                            $"{template.Name}, {squad.Name}, {unit.Name}"));
                    }
                }
            }
            foreach(Unit childUnit in unit.ChildUnits ?? Enumerable.Empty<Unit>())
            {
                openSlots.AddRange(GetOpeningsInUnit(childUnit, currentSquad, soldierTemplate));
            }
            return openSlots;
        }

        private IEnumerable<SoldierTemplate> GetOpeningsInSquad(Squad squad, Squad currentSquad, 
                                                                SoldierTemplate soldierTemplate)
        {
            List<SoldierTemplate> openSpots = new List<SoldierTemplate>();
            bool hasSquadLeader = squad.SquadLeader != null;
            // get the count of each soldier type in the squad
            // compare to the max count of each type
            Dictionary<SoldierTemplate, int> typeCountMap = 
                squad.Members.GroupBy(s => s.Template)
                             .ToDictionary(g => g.Key, g => g.Count());
            foreach(SquadTemplateElement element in squad.SquadTemplate.Elements)
            {
                // if the squad has no squad leader, only squad leader elements can be added now
                if(!hasSquadLeader && !element.SoldierTemplate.IsSquadLeader)
                {
                    continue;
                }
                if(currentSquad == squad && element.SoldierTemplate == soldierTemplate)
                {
                    continue;
                }
                if(element.SoldierTemplate.Rank < soldierTemplate.Rank 
                    || element.SoldierTemplate.Rank > soldierTemplate.Rank + 1)
                {
                    continue;
                }
                int existingHeadcount = 0;
                if(typeCountMap.ContainsKey(element.SoldierTemplate))
                {
                    existingHeadcount += typeCountMap[element.SoldierTemplate];
                }
                if(existingHeadcount < element.MaximumNumber)
                {
                    openSpots.Add(element.SoldierTemplate);
                }
            }
            return openSpots;
        }

        private void UpdateSquadLocations(Squad oldSquad, Squad newSquad)
        {
            if(newSquad.Members.Count == 1)
            {
                // make the location of the new squad the same as the old one
                newSquad.Location = oldSquad.Location;
                newSquad.BoardedLocation = oldSquad.BoardedLocation;
            }
            if(oldSquad.Members.Count == 0)
            {
                oldSquad.Location = null;
                oldSquad.BoardedLocation = null;
            }
        }

        protected Color DetermineDisplayColor(ISoldier soldier)
        {
            PlayerSoldier playerSoldier = GameSettings.Chapter.PlayerSoldierMap[soldier.Id];
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