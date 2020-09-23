using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Factions;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    class PlanetController : MonoBehaviour
    {
        [SerializeField]
        private UnitTreeView UnitTreeView;
        [SerializeField]
        private SquadArmamentView SquadArmamentView;
        [SerializeField]
        private PlanetView PlanetView;
        [SerializeField]
        private GameSettings GameSettings;

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
            UnitTreeView.Initialized = true;
        }

        public void UnitView_OnUnitSelected(int squadId)
        {
            // populate the SquadArmamentView
            _selectedSquad = GameSettings.SquadMap[squadId];
            SquadArmamentView.Clear();
            SquadArmamentView.SetIsFrontLine(!_selectedSquad.IsInReserve);
            if(_selectedSquad.SquadTemplate.DefaultWeapons != null)
            {
                SquadArmamentView.Initialize(!_selectedSquad.IsInReserve,
                                             _selectedSquad.Members.Count, 
                                             _selectedSquad.SquadTemplate.DefaultWeapons.Name, 
                                             GetSquadWeaponSelectionSections(_selectedSquad));
            }
        }

        public void SquadArmamentView_OnIsFrontLineChanged(bool newVal)
        {
            _selectedSquad.IsInReserve = !newVal;
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
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.OrderOfBattle.HQSquad.Id, 
                                         GameSettings.Chapter.OrderOfBattle.HQSquad.Name,
                                         DetermineDisplayColor(GameSettings.Chapter.OrderOfBattle.HQSquad));
                foreach(Squad squad in unitList[0].Squads)
                {
                    UnitTreeView.AddLeafUnit(squad.Id, squad.Name, DetermineDisplayColor(squad));
                }
                PopulateUnitTree(unitList[0].ChildUnits);
            }
            else
            {
                foreach (Unit unit in unitList)
                {
                    if (unit.Squads.Count > 0)
                    {
                        List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
                        foreach (Squad squad in unit.Squads)
                        {

                            squadList.Add(new Tuple<int, string>(squad.Id, squad.Name));
                        }
                        UnitTreeView.AddTreeUnit(unit.HQSquad.Id, unit.Name, squadList);
                    }
                    else if(unit.HQSquad != null)
                    {
                        UnitTreeView.AddLeafUnit(unit.HQSquad.Id, 
                                                 unit.HQSquad.Name,
                                                 DetermineDisplayColor(unit.HQSquad));
                    }
                }
            }
        }

        private Color DetermineDisplayColor(Squad squad)
        {
            var deployables = squad.Members.Select(s => GameSettings.PlayerSoldierMap[s.Id])
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
