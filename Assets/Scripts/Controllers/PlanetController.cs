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

        Unit _selectedUnit;

        private Dictionary<int, Unit> _unitMap = new Dictionary<int, Unit>();
        public void GalaxyController_OnPlanetSelected(Planet planet)
        {
            // assume player is Space Marine
            List<Unit> unitList = planet.FactionGroundUnitListMap == null ? null : planet.FactionGroundUnitListMap[TempFactions.Instance.SpaceMarines.Id];
            PlanetView.gameObject.SetActive(true);
            UnitTreeView.ClearTree();
            _unitMap.Clear();
            if(unitList != null && unitList.Count > 0)
            {
                PopulateUnitTree(unitList);
            }
            UnitTreeView.Initialized = true;
        }

        public void UnitView_OnUnitSelected(int unitId)
        {
            // populate the SquadArmamentView
            _selectedUnit = _unitMap[unitId];
            SquadArmamentView.Clear();
            SquadArmamentView.SetIsFrontLine(!_selectedUnit.IsInReserve);
            if(_selectedUnit.UnitTemplate.DefaultWeapons != null)
            {
                SquadArmamentView.Initialize(!_selectedUnit.IsInReserve, _selectedUnit.Members.Count, _selectedUnit.UnitTemplate.DefaultWeapons.Name, GetUnitWeaponSelectionSections(_selectedUnit));
            }
        }

        public void SquadArmamentView_OnIsFrontLineChanged(bool newVal)
        {
            _selectedUnit.IsInReserve = !newVal;
        }

        public void SquadArmamentView_OnArmamentChanged()
        {
            if (_selectedUnit != null)
            {
                List<Tuple<string, int>> loadout = SquadArmamentView.GetSelections();
                _selectedUnit.Loadout.Clear();
                _selectedUnit.Loadout = ConvertToWeaponSetList(loadout);
            }
        }

        private List<WeaponSet> ConvertToWeaponSetList(List<Tuple<string, int>> tuples)
        {
            List<WeaponSet> loadout = new List<WeaponSet>();
            var allowedWeaponSets = _selectedUnit.UnitTemplate.WeaponOptions.SelectMany(wo => wo.Options);
            foreach(Tuple<string, int> tuple in tuples)
            {
                // don't track the default loadouts, just the specialized ones
                // also don't need to do anything with zero-value tuples
                if (tuple.Item2 == 0 || tuple.Item1 == _selectedUnit.UnitTemplate.DefaultWeapons.Name) continue;
                for (int i = 0; i < tuple.Item2; i++)
                {
                    loadout.Add(allowedWeaponSets.Single(aws => aws.Name == tuple.Item1));
                }
            }
            return loadout;
        }

        private List<WeaponSelectionSection> GetUnitWeaponSelectionSections(Unit unit)
        {
            List<WeaponSelectionSection> list = new List<WeaponSelectionSection>();
            foreach(UnitWeaponOption option in unit.UnitTemplate.WeaponOptions)
            {
                WeaponSelectionSection section = new WeaponSelectionSection();
                section.Label = option.Name;
                section.MaxCount = option.MaxNumber;
                section.MinCount = option.MinNumber;
                section.Selections = new List<Tuple<string, int>>();
                foreach(WeaponSet weaponSet in option.Options)
                {
                    int currentCount = unit.Loadout.Where(l => l == weaponSet).Count();
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
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.Id, GameSettings.Chapter.Name + " HQ Squad");
                PopulateUnitTree(unitList[0].ChildUnits);
            }
            else
            {
                foreach (Unit unit in unitList)
                {
                    if (unit.ChildUnits != null && unit.ChildUnits.Count > 0)
                    {
                        List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
                        foreach (Unit squad in unit.ChildUnits)
                        {

                            squadList.Add(new Tuple<int, string>(squad.Id, squad.Name));
                            _unitMap[squad.Id] = squad;
                        }
                        UnitTreeView.AddTreeUnit(unit.Id, unit.Name, squadList);
                    }
                    else
                    {
                        UnitTreeView.AddLeafUnit(unit.Id, unit.Name);
                    }
                }
            }
        }
    }
}
