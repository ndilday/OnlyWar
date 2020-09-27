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

        public void GalaxyController_OnPlanetSelected(Planet planet)
        {
            // assume player is Space Marine
            List<Unit> unitList = planet.FactionGroundUnitListMap?[TempFactions.Instance.SpaceMarineFaction.Id];
            PlanetView.gameObject.SetActive(true);
            UnitTreeView.ClearTree();
            CreateScoutingReport(planet);
            if(unitList?.Count > 0)
            {
                PopulateUnitTree(unitList);
            }
            if(planet.LocalFleet != null)
            {
                FleetView.gameObject.SetActive(true);
                PopulateFleetTree(planet.LocalFleet);
            }
            UnitTreeView.Initialized = true;
        }

        public void UnitView_OnUnitSelected(int squadId)
        {
            // populate the SquadArmamentView
            _selectedSquad = GameSettings.SquadMap[squadId];
            SquadArmamentView.Clear();
            Tuple<Color, int> deployData = DetermineSquadDisplayValues(_selectedSquad);
            SquadArmamentView.Initialize(deployData.Item2 < 2,
                                         !_selectedSquad.IsInReserve,
                                         _selectedSquad.Members.Count, 
                                         _selectedSquad.SquadTemplate.DefaultWeapons?.Name, 
                                         GetSquadWeaponSelectionSections(_selectedSquad));
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
                    if (kvp.Key == TempFactions.Instance.SpaceMarineFaction.Id) hasMarineForces = true;
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
            if (!hasMarineForces) PlanetView.UpdateScoutingReport("With no forces on planet, we have no sense of what xenos or heretics may exist here.");
            else PlanetView.UpdateScoutingReport(newReport);
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

        private void PopulateUnitTree(List<Unit> unitList)
        {
            if (unitList[0].Id == GameSettings.Chapter.OrderOfBattle.Id)
            {
                Tuple<Color, int> tuple = 
                    DetermineSquadDisplayValues(GameSettings.Chapter.OrderOfBattle.HQSquad);
                if (tuple.Item2 == 2) GameSettings.Chapter.OrderOfBattle.HQSquad.IsInReserve = true;
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.OrderOfBattle.HQSquad.Id, 
                                         GameSettings.Chapter.OrderOfBattle.HQSquad.Name,
                                         tuple.Item1, tuple.Item2);
                foreach(Squad squad in unitList[0].Squads)
                {
                    tuple = DetermineSquadDisplayValues(squad);
                    if (tuple.Item2 == 2) squad.IsInReserve = true;
                    UnitTreeView.AddLeafUnit(squad.Id, squad.Name, tuple.Item1, tuple.Item2);
                }
                PopulateUnitTree(unitList[0].ChildUnits);
            }
            else
            {
                foreach (Unit unit in unitList)
                {
                    if (unit.Squads.Count > 0)
                    {
                        Tuple<Color, int> display;
                        List<Tuple<int, string, Color, int>> squadList = 
                            new List<Tuple<int, string, Color, int>>();
                        foreach (Squad squad in unit.Squads)
                        {
                            display = DetermineSquadDisplayValues(squad);
                            if (display.Item2 == 2) squad.IsInReserve = true;
                            squadList.Add(
                                new Tuple<int, string, Color, int>(squad.Id, squad.Name, 
                                                                   display.Item1, display.Item2));
                        }
                        display = DetermineSquadDisplayValues(unit.HQSquad);
                        if (display.Item2 == 2) unit.HQSquad.IsInReserve = true;
                        UnitTreeView.AddTreeUnit(unit.HQSquad.Id, unit.Name, display.Item1, 
                                                 display.Item2, squadList);
                    }
                    else if(unit.HQSquad != null)
                    {
                        UnitTreeView.AddLeafUnit(unit.HQSquad.Id, 
                                                 unit.HQSquad.Name,
                                                 DetermineDisplayColor(unit.HQSquad, 
                                                                       GameSettings.PlayerSoldierMap));
                    }
                }
            }
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
                        return new Tuple<Color, int>(Color.red, 2);
                    }
                    else if (typeGroups[element.SoldierType].Count() < element.MaximumNumber)
                    {
                        isFull = false;
                    }
                }
                else
                {
                    return new Tuple<Color, int>(Color.red, 2);
                }
            }
            if (deployables.Count() < squad.Members.Count)
            {
                return new Tuple<Color, int>(new Color(255, 200, 50), 1);
            }
            Color color = isFull ? Color.white : Color.yellow;
            int number = squad.IsInReserve ? -1 : 0;
            return new Tuple<Color, int>(color, number);
        }

        private void PopulateFleetTree(Fleet fleet)
        {
            // foreach ship in fleet, add a company style node
            // if the fleet has any troops, display those as children
            foreach (Ship ship in fleet.Ships)
            {
                if (ship.LoadedSquads.Count > 0)
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
                else
                {
                    var tuple = DetermineShipDisplayValues(fleet, ship);
                    FleetView.AddLeafUnit(ship.Id,
                                             DetermineShipText(fleet, ship),
                                             tuple.Item1, 
                                             tuple.Item2);
                }
            }
        }

        private Tuple<Color, int> DetermineShipDisplayValues(Fleet fleet, Ship ship)
        {
            Color color = ship.LoadedSoldierCount == ship.Template.SoldierCapacity 
                ? Color.yellow : Color.white;
            return new Tuple<Color, int>(color, fleet.Destination != null ? 4 : -1);
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
