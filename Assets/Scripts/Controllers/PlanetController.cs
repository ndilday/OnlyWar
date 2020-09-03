using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    class PlanetController : MonoBehaviour
    {
        public UnitTreeView UnitTreeView;
        public SquadArmamentView SquadArmamentView;
        public PlanetView PlanetView;
        public GameSettings GameSettings;

        Squad _selectedSquad;

        private readonly Dictionary<int, Squad> _squadMap = new Dictionary<int, Squad>();
        public void GalaxyController_OnPlanetSelected(Planet planet)
        {
            // assume player is Space Marine
            List<Unit> unitList = planet.FactionGroundUnitListMap?[TempFactions.Instance.SpaceMarines.Id];
            PlanetView.gameObject.SetActive(true);
            UnitTreeView.ClearTree();
            _squadMap.Clear();
            if(unitList != null && unitList.Count > 0)
            {
                PopulateUnitTree(unitList);
            }
            UnitTreeView.Initialized = true;
        }

        public void UnitView_OnUnitSelected(int squadId)
        {
            // populate the SquadArmamentView
            _selectedSquad = _squadMap[squadId];
            SquadArmamentView.Clear();
            SquadArmamentView.SetIsFrontLine(!_selectedSquad.IsInReserve);
            if(_selectedSquad.SquadTemplate.DefaultWeapons != null)
            {
                SquadArmamentView.Initialize(!_selectedSquad.IsInReserve, _selectedSquad.GetAllMembers().Length, _selectedSquad.SquadTemplate.DefaultWeapons.Name, GetSquadWeaponSelectionSections(_selectedSquad));
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
            foreach(UnitWeaponOption option in squad.SquadTemplate.WeaponOptions)
            {
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
                    section.Selections.Add(new Tuple<string, int>(weaponSet.Name, currentCount));
                }
                list.Add(section);
            }

            return list;
        }

        private void PopulateUnitTree(List<Unit> unitList)
        {
            if (unitList[0].Id == GameSettings.Chapter.Id)
            {
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.HQSquad.Id, GameSettings.Chapter.HQSquad.Name);
                foreach(Squad squad in unitList[0].Squads)
                {
                    UnitTreeView.AddLeafUnit(squad.Id, squad.Name);
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
                            _squadMap[squad.Id] = squad;
                        }
                        UnitTreeView.AddTreeUnit(unit.HQSquad.Id, unit.Name, squadList);
                    }
                    else if(unit.HQSquad != null)
                    {
                        UnitTreeView.AddLeafUnit(unit.HQSquad.Id, unit.HQSquad.Name);
                    }
                }
            }
        }
    }
}
