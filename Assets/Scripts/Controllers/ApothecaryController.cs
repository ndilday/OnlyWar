using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class ApothecaryController : MonoBehaviour
    {
        [SerializeField]
        private GameSettings GameSettings;

        [SerializeField]
        private ApothecaryView ApothecaryView;

        [SerializeField]
        private UnitTreeView UnitTreeView;

        private readonly Dictionary<int, Squad> _squadMap = new Dictionary<int, Squad>();

        public void ApothecaryButton_OnClick()
        {
            InitializeUnitTree();
            ApothecaryView.UpdateGeneSeedText(GenerateGeneseedReport());
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
                List<Tuple<int, string, string>> memberList = selectedSquad.GetAllMembers().Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
                ApothecaryView.ReplaceSquadMemberContent(memberList);
                ApothecaryView.ReplaceSelectedSoldierText(GenerateSquadSummary(selectedSquad));
            }
        }

        private void InitializeUnitTree()
        {
            if (!UnitTreeView.Initialized)
            {
                UnitTreeView.AddLeafUnit(GameSettings.Chapter.OrderOfBattle.HQSquad.Id, GameSettings.Chapter.OrderOfBattle.HQSquad.Name);
                _squadMap[GameSettings.Chapter.OrderOfBattle.HQSquad.Id] = GameSettings.Chapter.OrderOfBattle.HQSquad;

                foreach (Squad squad in GameSettings.Chapter.OrderOfBattle.Squads)
                {
                    _squadMap[squad.Id] = squad;
                    UnitTreeView.AddLeafUnit(squad.Id, squad.Name);
                }
                foreach (Unit company in GameSettings.Chapter.OrderOfBattle.ChildUnits)
                {
                    _squadMap[company.HQSquad.Id] = company.HQSquad;
                    if (company.Squads == null || company.Squads.Count == 0)
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

        private string GenerateGeneseedReport()
        {
            return "";
        }

        private void UnitSelected(int unitId)
        {
            Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == unitId);
            List<Tuple<int, string, string>> memberList = selectedUnit.HQSquad.GetAllMembers().Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
            ApothecaryView.ReplaceSquadMemberContent(memberList);
            ApothecaryView.ReplaceSelectedSoldierText(GenerateUnitSummary(selectedUnit));
        }

        private string GenerateUnitSummary(Unit selectedUnit)
        {
            return selectedUnit.Name;
        }

        private string GenerateSquadSummary(Squad selectedSquad)
        {
            return selectedSquad.Name;
        }

        public void ApothecaryView_OnSquadMemberSelected(int soldierId)
        {

        }
    }
}