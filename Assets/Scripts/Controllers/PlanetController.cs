using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Factions;
using Iam.Scripts.Models.Fleets;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    class PlanetController : ChapterUnitTreeController
    {
        [SerializeField]
        private UnitTreeView UnitTreeView;
        [SerializeField]
        private SquadArmamentView SquadArmamentView;
        [SerializeField]
        private PlanetView PlanetView;
        [SerializeField]
        private GameSettings GameSettings;
        [SerializeField]
        private UnitTreeView FleetView;

        private Squad _selectedSquad;
        private Unit _selectedUnit;
        private Ship _selectedShip;
        private Squad _selectedShipSquad;
        private Planet _selectedPlanet;

        public void GalaxyController_OnPlanetSelected(Planet planet)
        {
            _selectedPlanet = planet;
            // assume player is Space Marine
            List<Unit> unitList = null;
            if (planet.FactionGroundUnitListMap.ContainsKey(TempFactions.Instance.SpaceMarineFaction.Id))
            {
                unitList = planet.FactionGroundUnitListMap?[TempFactions.Instance.SpaceMarineFaction.Id];
            }
            PlanetView.gameObject.SetActive(true);
            CreateScoutingReport(planet);
            if(unitList?.Count > 0)
            {
                PopulateUnitTree();
            }
            else
            {
                UnitTreeView.ClearTree();
            }
            if(planet.Fleets.Count > 0)
            {
                FleetView.gameObject.SetActive(true);
                PopulateFleetTree(planet.Fleets);
            }
            UnitTreeView.Initialized = true;
        }

        public void UnitView_OnUnitSelected(int unitId)
        {
            SquadArmamentView.Clear();
            _selectedSquad = null;

            // populate the SquadArmamentView
            if (unitId == GameSettings.Chapter.OrderOfBattle.Id)
            {
                _selectedUnit = GameSettings.Chapter.OrderOfBattle;
            }
            else
            {
                _selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits
                    .First(company => company.Id == unitId);
            }
            
            bool enableAdd = false;
            if(_selectedShip != null && _selectedShip.AvailableCapacity > 0)
            {
                // see if the planetside unit can fit onto the currently selected ship
                int runningTotal = 0;
                if(_selectedUnit.HQSquad.Location == _selectedPlanet)
                {
                    runningTotal += _selectedUnit.HQSquad.Members.Count;
                }
                
                if(runningTotal <= _selectedShip.AvailableCapacity)
                {
                    foreach(Squad squad in _selectedUnit.Squads)
                    {
                        runningTotal += squad.Members.Count;
                        if(runningTotal > _selectedShip.AvailableCapacity)
                        {
                            break;
                        }
                    }
                    if(runningTotal < _selectedShip.AvailableCapacity)
                    {
                        enableAdd = true;
                    }
                }
            }
            PlanetView.EnableLoadInShipButton(enableAdd);
        }

        public void UnitView_OnSquadSelected(int squadId)
        {
            // populate the SquadArmamentView
            _selectedSquad = GameSettings.SquadMap[squadId];
            _selectedUnit = null;
            SquadArmamentView.Clear();
            Tuple<Color, int> deployData = DetermineSquadDisplayValues(_selectedSquad);
            SquadArmamentView.Initialize(deployData.Item2 < 2,
                                         !_selectedSquad.IsInReserve,
                                         _selectedSquad.Members.Count, 
                                         _selectedSquad.SquadTemplate.DefaultWeapons?.Name, 
                                         GetSquadWeaponSelectionSections(_selectedSquad));
            // determine if this squad can fit on the selected ship
            bool enableLoadToShipButton =
                _selectedShip != null 
                    && _selectedShip.AvailableCapacity >= _selectedSquad.Members.Count;
            PlanetView.EnableLoadInShipButton(enableLoadToShipButton);
        }

        public void FleetView_OnShipSelected(int shipId)
        {
            foreach(Fleet fleet in _selectedPlanet.Fleets)
            {
                Ship ship = fleet.Ships.FirstOrDefault(s => s.Id == shipId);
                if(ship != null)
                {
                    _selectedShip = ship;
                    break;
                }
            }
            _selectedShipSquad = null;
            PlanetView.EnableRemoveFromShipButton(false);
            if(_selectedSquad != null || _selectedUnit != null)
            {
                PlanetView.EnableLoadInShipButton(true);
            }
        }

        public void FleetView_OnSquadSelected(int squadId)
        {
            _selectedShip = null;
            _selectedShipSquad = GameSettings.SquadMap[squadId];
            PlanetView.EnableLoadInShipButton(false);
            PlanetView.EnableRemoveFromShipButton(true);
        }

        public void SquadArmamentView_OnIsFrontLineChanged(bool newVal)
        {
            _selectedSquad.IsInReserve = !newVal;
            Tuple<Color, int> tuple = DetermineSquadDisplayValues(_selectedSquad);
            UnitTreeView.UpdateUnitBadge(_selectedSquad.Id, newVal ? 0 : tuple.Item2);
        }

        public void SquadArmamentView_OnArmamentChanged()
        {
            if (_selectedSquad != null)
            {
                List<Tuple<string, int>> loadout = SquadArmamentView.GetSelections();
                _selectedSquad.Loadout.Clear();
                _selectedSquad.Loadout = ConvertToWeaponSetList(loadout);
            }
        }

        public void LoadInShipButton_OnClick()
        {
            if(_selectedSquad != null)
            {
                MoveSquadToSelectedShip(_selectedSquad);
            }
            else if(_selectedUnit != null)
            {
                if (_selectedUnit.HQSquad != null)
                {
                    MoveSquadToSelectedShip(_selectedUnit.HQSquad);
                }
                foreach(Squad squad in _selectedUnit.Squads)
                {
                    MoveSquadToSelectedShip(squad);
                }
            }
            // rebuild both trees
            PopulateUnitTree();
            PopulateFleetTree(_selectedPlanet.Fleets);
            PlanetView.EnableLoadInShipButton(false);
            _selectedShip = null;
            _selectedSquad = null;
            _selectedUnit = null;
        }

        public void RemoveFromShipButton_OnClick()
        {
            _selectedShipSquad.BoardedLocation.RemoveSquad(_selectedShipSquad);
            _selectedShipSquad.BoardedLocation = null;
            _selectedShipSquad.Location = _selectedPlanet;
            PopulateUnitTree();
            PopulateFleetTree(_selectedPlanet.Fleets);
            PlanetView.EnableRemoveFromShipButton(false);

            var factionUnitMap = _selectedShipSquad.Location.FactionGroundUnitListMap;
            if (!factionUnitMap.ContainsKey(TempFactions.Instance.SpaceMarineFaction.Id))
            {
                factionUnitMap[TempFactions.Instance.SpaceMarineFaction.Id] =
                    new List<Unit>();
            }
            var unitList = factionUnitMap[TempFactions.Instance.SpaceMarineFaction.Id];
            if (!unitList.Contains(GameSettings.Chapter.OrderOfBattle))
            {
                // this feels hacky, but I think it's the safest choice
                unitList.Add(GameSettings.Chapter.OrderOfBattle);
            }
            _selectedShipSquad = null;
            _selectedShip = null;
            _selectedSquad = null;
            _selectedUnit = null;
        }

        private void CreateScoutingReport(Planet planet)
        {
            PlanetView.UpdateScoutingReport("");
            string newReport = "";
            bool hasMarineForces = false;
            if (planet.FactionGroundUnitListMap != null)
            {
                foreach (KeyValuePair<int, List<Unit>> kvp in planet.FactionGroundUnitListMap)
                {
                    string factionName = "Xenos";
                    if (kvp.Key == TempFactions.Instance.TyranidFaction.Id)
                    {
                        factionName = "Tyranid";
                    }
                    int factionSoldierCount = 0;
                    if (kvp.Key == TempFactions.Instance.SpaceMarineFaction.Id)
                    {
                        hasMarineForces = true;
                    }
                    else
                    {
                        foreach (Unit unit in kvp.Value)
                        {
                            factionSoldierCount += unit.GetAllMembers().Count();
                        }
                        newReport = factionName + " forces on the planet number in the ";
                        if (factionSoldierCount >= 2000) newReport += "thousands.";
                        else if (factionSoldierCount >= 200) newReport += "hundreds.";
                        else if (factionSoldierCount >= 24) newReport += "dozens.";
                        else newReport = factionName + " forces on the planet are minimal, and should be easy to deal with.";
                    }
                }
            }
            if (!hasMarineForces && planet.Fleets.Count == 0)
            {
                PlanetView.UpdateScoutingReport("With no forces on planet, we have no sense of what xenos or heretics may exist here.");
            }
            else
            {
                PlanetView.UpdateScoutingReport(newReport);
            }
        }

        private List<WeaponSet> ConvertToWeaponSetList(List<Tuple<string, int>> tuples)
        {
            List<WeaponSet> loadout = new List<WeaponSet>();
            var allowedWeaponSets = _selectedSquad.SquadTemplate.WeaponOptions.SelectMany(wo => wo.Options);
            foreach(Tuple<string, int> tuple in tuples)
            {
                // don't track the default loadouts, just the specialized ones
                // also don't need to do anything with zero-value tuples
                if (tuple.Item2 == 0 || tuple.Item1 == _selectedSquad.SquadTemplate.DefaultWeapons.Name) continue;
                for (int i = 0; i < tuple.Item2; i++)
                {
                    loadout.Add(allowedWeaponSets.Single(aws => aws.Name == tuple.Item1));
                }
            }
            return loadout;
        }

        private List<WeaponSelectionSection> GetSquadWeaponSelectionSections(Squad squad)
        {
            List<WeaponSelectionSection> list = new List<WeaponSelectionSection>();
            if (squad.SquadTemplate.WeaponOptions != null)
            {
                foreach (SquadWeaponOption option in squad.SquadTemplate.WeaponOptions)
                {
                    int optionCount = 0;
                    WeaponSelectionSection section = new WeaponSelectionSection
                    {
                        Label = option.Name,
                        MaxCount = option.MaxNumber,
                        MinCount = option.MinNumber,
                        Selections = new List<Tuple<string, int>>()
                    };
                    foreach (WeaponSet weaponSet in option.Options)
                    {
                        int currentCount = squad.Loadout.Where(l => l == weaponSet).Count();
                        optionCount += currentCount;
                        section.Selections.Add(new Tuple<string, int>(weaponSet.Name, currentCount));
                    }
                    section.CurrentCount = optionCount;
                    list.Add(section);
                }
            }

            return list;
        }

        private void PopulateUnitTree()
        {
            UnitTreeView.ClearTree();
            // go through the Chapter OOB and see which squads are on this planet
            if(GameSettings.Chapter.OrderOfBattle.HQSquad.Location == _selectedPlanet)
            {
                AddSquadToUnitTreeAtRoot(GameSettings.Chapter.OrderOfBattle.HQSquad);
            }
            foreach(Squad squad in GameSettings.Chapter.OrderOfBattle.Squads)
            {
                if(squad.Location == _selectedPlanet)
                {
                    AddSquadToUnitTreeAtRoot(squad);
                }
            }
            foreach(Unit company in GameSettings.Chapter.OrderOfBattle.ChildUnits)
            {
                AddCompanyToUnitTree(company);
            }
        }

        private void AddCompanyToUnitTree(Unit company)
        {
            // see if any squads in the company are stationed on this planet
            // if so, display the unit in the squad view
            Tuple<Color, int> display;
            List<Tuple<int, string, Color, int>> squadList =
                new List<Tuple<int, string, Color, int>>();
            // check HQ Squad
            if (company.HQSquad?.Location == _selectedPlanet)
            {
                display = DetermineSquadDisplayValues(company.HQSquad);
                if (display.Item2 == 2) company.HQSquad.IsInReserve = true;
                squadList.Add(
                    new Tuple<int, string, Color, int>(company.HQSquad.Id,
                                                       company.HQSquad.Name,
                                                       display.Item1, display.Item2));
            }
            foreach (Squad squad in company.Squads)
            {
                if (squad.Location == _selectedPlanet)
                {
                    display = DetermineSquadDisplayValues(squad);
                    if (display.Item2 == 2) squad.IsInReserve = true;
                    squadList.Add(
                        new Tuple<int, string, Color, int>(squad.Id, squad.Name,
                                                           display.Item1, display.Item2));
                }
            }
            if (squadList.Count > 0)
            {
                UnitTreeView.AddTreeUnit(company.Id, company.Name, Color.white,
                                             Badge.NORMAL, squadList);
            }
        }

        private void AddSquadToUnitTreeAtRoot(Squad squad)
        {
            Tuple<Color, int> tuple =
                                DetermineSquadDisplayValues(squad);
            if (tuple.Item2 == Badge.INSUFFICIENT_MEN)
            {
                squad.IsInReserve = true;
            }
            UnitTreeView.AddLeafSquad(squad.Id, squad.Name, tuple.Item1, tuple.Item2);
        }

        private Tuple<Color, int> DetermineSquadDisplayValues(Squad squad)
        {
            var deployables = squad.Members.Select(s => GameSettings.PlayerSoldierMap[s.Id])
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
                        return new Tuple<Color, int>(Color.red, Badge.INSUFFICIENT_MEN);
                    }
                    else if (typeGroups[element.SoldierType].Count() < element.MaximumNumber)
                    {
                        isFull = false;
                    }
                }
                else
                {
                    return new Tuple<Color, int>(Color.red, Badge.INSUFFICIENT_MEN);
                }
            }
            if (deployables.Count() < squad.Members.Count)
            {
                return new Tuple<Color, int>(new Color(255, 200, 50), Badge.INJURED);
            }
            Color color = isFull ? Color.white : Color.yellow;
            int number = squad.IsInReserve ? Badge.NORMAL : Badge.COMBAT;
            return new Tuple<Color, int>(color, number);
        }

        private void PopulateFleetTree(IReadOnlyList<Fleet> fleets)
        {
            // foreach ship in fleet, add a company style node
            // if the fleet has any troops, display those as children
            FleetView.ClearTree();
            foreach (Fleet fleet in fleets)
            {
                foreach (Ship ship in fleet.Ships)
                {

                    Tuple<Color, int> display;
                    List<Tuple<int, string, Color, int>> squadList =
                        new List<Tuple<int, string, Color, int>>();
                    foreach (Squad squad in ship.LoadedSquads)
                    {
                        display = DetermineSquadDisplayValues(squad);
                        if (display.Item2 == 2) squad.IsInReserve = true;
                        squadList.Add(
                            new Tuple<int, string, Color, int>(squad.Id, squad.Name,
                                                                display.Item1, display.Item2));
                    }
                    display = DetermineShipDisplayValues(fleet, ship);
                    FleetView.AddTreeUnit(ship.Id, DetermineShipText(fleet, ship),
                                                display.Item1, display.Item2, squadList);
                }
            }
        }

        private void MoveSquadToSelectedShip(Squad squad)
        {
            _selectedShip.LoadSquad(squad);
            squad.Location = null;
            squad.BoardedLocation = _selectedShip;
        }

        private Tuple<Color, int> DetermineShipDisplayValues(Fleet fleet, Ship ship)
        {
            Color color = ship.LoadedSoldierCount == ship.Template.SoldierCapacity 
                ? Color.yellow : Color.white;
            return new Tuple<Color, int>(color, 
                                         fleet.Destination != null 
                                            ? Badge.TAKEOFF : Badge.NORMAL);
        }

        private string DetermineShipText(Fleet fleet, Ship ship)
        {
            string returnValue = "";
            returnValue += ship.Name + "\n";
            returnValue += ship.LoadedSoldierCount.ToString() + "/"
                + ship.Template.SoldierCapacity.ToString();
            if(fleet.Destination != null)
            {
                returnValue += $", Destination:{fleet.Destination.Name}";
            }
            return returnValue;
        }
    }
}
