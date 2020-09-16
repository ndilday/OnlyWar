using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;
using Iam.Scripts.Models.Soldiers;

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
        private Squad _selectedSquad;
        private const string GENESEED_FORMAT = @"Sir! Currently, we have {0} Geneseed stored.
Within the next year, we anticipate {1} implanted Progenoid Glands will mature.";
        private const string SQUAD_FORMAT = @"{0} has {1} wounded members.
Of those, {2} are unfit for field duty under any circumstances; {3} require cybernetic replacements.
It will require approximately {4} weeks before all marines in the squad (other than those replacing cybernetic replacements) are fully fit.";

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
                Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == squadId);
                SquadSelected(selectedUnit.HQSquad);
            }
            else
            {
                SquadSelected(_squadMap[squadId]);
            }
        }

        public void ApothecaryView_OnSquadMemberSelected(int soldierId)
        {
            SoldierSelected(soldierId);
        }

        public void EndTurnButton_OnClick()
        {
            // heal wounds by one week
            foreach(Soldier soldier in GameSettings.Chapter.OrderOfBattle.GetAllMembers())
            {
                foreach(HitLocation hitLocation in soldier.Body.HitLocations)
                {
                    if(hitLocation.Wounds.WoundTotal > 0 && !hitLocation.IsSevered)
                    {
                        hitLocation.Wounds.ApplyWeekOfHealing();
                    }
                }
            }
        }

        private void InitializeUnitTree()
        {
            if (!UnitTreeView.Initialized)
            {
                UnitTreeView.ClearTree();
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
                    if (company.Squads?.Count == 0)
                    {
                        // this is unexpected, currently
                        Debug.Log("We have a company with no squads?");
                        UnitTreeView.AddLeafUnit(company.HQSquad.Id, company.HQSquad.Name);
                    }
                    else
                    {
                        List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
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
            ushort currentGeneseed = GameSettings.Chapter.GeneseedStockpile;
            Date fourYearsAgo = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 4, GameSettings.Date.Week);
            Date fiveYearsAgo = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 5, GameSettings.Date.Week);
            Date nineYearsAgo = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 9, GameSettings.Date.Week);
            Date tenYearsAgo = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 10, GameSettings.Date.Week);
            ushort inAYear = 0;
            foreach(SpaceMarine marine in GameSettings.Chapter.OrderOfBattle.GetAllMembers())
            {
                Date implantDate = marine.ProgenoidImplantDate;
                if(implantDate.IsBetweenInclusive(fiveYearsAgo, fourYearsAgo)
                    || implantDate.IsBetweenInclusive(tenYearsAgo, nineYearsAgo))
                {
                    inAYear++;
                }
            }
            return string.Format(GENESEED_FORMAT, currentGeneseed, inAYear);
        }

        private void SquadSelected(Squad squad)
        {
            _selectedSquad = squad;
            List<Tuple<int, string, string>> memberList = _selectedSquad.GetAllMembers().Select(s => new Tuple<int, string, string>(s.Id, s.JobRole, s.ToString())).ToList();
            ApothecaryView.ReplaceSquadMemberContent(memberList);
            ApothecaryView.ReplaceSelectedSoldierText(GenerateSquadSummary(_selectedSquad));
        }

        private void SoldierSelected(int soldierId)
        {
            Soldier selected = _selectedSquad.GetAllMembers().First(s => s.Id == soldierId);
            ApothecaryView.ReplaceSelectedSoldierText(GenerateSoldierSummary(selected));
        }

        private string GenerateSquadSummary(Squad selectedSquad)
        {
            byte woundedSoldiers = 0;
            byte soldiersMissingBodyParts = 0;
            byte maxRecoveryTime = 0;
            byte unfitSoldiers = 0;
            foreach(Soldier soldier in selectedSquad.GetAllMembers())
            {
                bool isWounded = false;
                bool isMissingParts = false;
                bool isUnfit = false;
                byte greatestWoundHealTime = 0;
                foreach(HitLocation hitLocation in soldier.Body.HitLocations)
                {
                    if(!isMissingParts && hitLocation.IsSevered)
                    {
                        isWounded = true;
                        isMissingParts = true;
                        isUnfit = true;
                    }
                    else if(hitLocation.Wounds.WoundTotal > 0)
                    {
                        isWounded = true;
                        byte healTime = hitLocation.Wounds.RecoveryTimeLeft();
                        if(healTime > greatestWoundHealTime)
                        {
                            greatestWoundHealTime = healTime;
                        }
                        if(hitLocation.IsCrippled)
                        {
                            isUnfit = true;
                        }
                    }
                }
                if (isWounded)
                {
                    woundedSoldiers++;
                }
                if (isMissingParts)
                {
                    soldiersMissingBodyParts++;
                }
                if (greatestWoundHealTime > maxRecoveryTime)
                {
                    maxRecoveryTime = greatestWoundHealTime;
                }
                if (isUnfit)
                {
                    unfitSoldiers++;
                }
            }
            if(woundedSoldiers == 0)
            {
                return selectedSquad.Name + " is entirely fit for duty.";
            }
            return string.Format(SQUAD_FORMAT, 
                                 selectedSquad.Name,
                                 woundedSoldiers,
                                 unfitSoldiers,
                                 soldiersMissingBodyParts,
                                 maxRecoveryTime);
        }

        private string GenerateSoldierSummary(Soldier selectedSoldier)
        {
            string summary = selectedSoldier.ToString() + "\n";
            byte recoveryTime = 0;
            bool isSevered = false;
            foreach (HitLocation hl in selectedSoldier.Body.HitLocations)
            {
                if (hl.Wounds.WoundTotal != 0)
                {
                    if(hl.IsSevered)
                    {
                        isSevered = true;
                    }
                    byte woundTime = hl.Wounds.RecoveryTimeLeft();
                    if(woundTime > recoveryTime)
                    {
                        recoveryTime = woundTime;
                    }
                    summary += hl.ToString() + "\n";
                }
            }
            if(isSevered)
            {
                summary += selectedSoldier.ToString() +
                    " will be unable to perform field duties until receiving cybernetic replacements\n";
            }
            else if(recoveryTime > 0)
            {
                summary += selectedSoldier.ToString() +
                    " requires " + recoveryTime.ToString() + " to be fully fit for duty\n";
            }
            else
            {
                summary += selectedSoldier.ToString() +
                    " is fully fit and ready to serve the Emperor\n";
            }
            return summary;
        }
    }
}