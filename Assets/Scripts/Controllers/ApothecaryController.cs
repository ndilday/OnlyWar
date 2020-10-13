using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;
using OnlyWar.Scripts.Views;
using OnlyWar.Scripts.Models.Soldiers;

namespace OnlyWar.Scripts.Controllers
{
    public class ApothecaryController : ChapterUnitTreeController
    {
        [SerializeField]
        private GameSettings GameSettings;

        [SerializeField]
        private ApothecaryView ApothecaryView;

        [SerializeField]
        private UnitTreeView UnitTreeView;

        private Squad _selectedSquad;
        private const string GENESEED_FORMAT = @"Sir! Currently, we have {0} Geneseed stored.
Within the next year, we anticipate {1} implanted Progenoid Glands will mature.";
        private const string SQUAD_FORMAT = @"{0} has {1} wounded members.
Of those, {2} are unfit for field duty under any circumstances; {3} require cybernetic replacements and will be fitted within the next few days.
It will require approximately {4} weeks before all marines in the squad (other than those replacing cybernetic replacements) are fully fit.";

        public void ApothecaryButton_OnClick()
        {
            ApothecaryView.gameObject.SetActive(true);
            InitializeUnitTree();
            ApothecaryView.UpdateGeneSeedText(GenerateGeneseedReport());
        }

        public void UnitTreeView_OnUnitSelected(int squadId)
        {
            // populate view with members of selected squad
            if (!GameSettings.Chapter.SquadMap.ContainsKey(squadId))
            {
                Unit selectedUnit = GameSettings.Chapter.OrderOfBattle.ChildUnits.First(u => u.Id == squadId);
                SquadSelected(selectedUnit.HQSquad);
            }
            else
            {
                SquadSelected(GameSettings.Chapter.SquadMap[squadId]);
            }
        }

        public void ApothecaryView_OnSquadMemberSelected(int soldierId)
        {
            SoldierSelected(soldierId);
        }

        public void UIController_OnTurnEnd()
        {
            // make sure the dialog is closed
            ApothecaryView.gameObject.SetActive(false);

            // heal wounds by one week
            foreach(PlayerSoldier soldier in GameSettings.Chapter.OrderOfBattle.GetAllMembers())
            {
                foreach(HitLocation hitLocation in soldier.Body.HitLocations)
                {
                    if(hitLocation.IsSevered)
                    {
                        // for now, just give it a cybernetic replacement
                        ApplyBionics(hitLocation, soldier);
                    }
                    else if(hitLocation.Wounds.WoundTotal > 0 )
                    {
                        if (hitLocation.IsCybernetic)
                        {
                            // assume replacement, for now
                            hitLocation.Wounds.HealWounds();
                        }
                        else
                        {
                            hitLocation.Wounds.ApplyWeekOfHealing();
                        }
                    }
                }
            }
        }

        private void ApplyBionics(HitLocation hitLocation, PlayerSoldier soldier)
        {
            // NOTE: We don't heal the wound here. 
            // The wound will heal automatically in the next turn.
            // This represents the marine learning how to use the new body part.
            hitLocation.IsCybernetic = true;
            soldier.AddEntryToHistory($"Received bioic {hitLocation.Template.Name} replacement");

            if (hitLocation.Template.Name.Contains("Arm"))
            {
                hitLocation.Armor = 2;
                string otherName = hitLocation.Template.Name.Replace("Arm", "Hand");
                var otherHitLocation = soldier.Body.HitLocations
                    .First(hl => hl.Template.Name == otherName);
                otherHitLocation.Armor = 2;
                otherHitLocation.IsCybernetic = true;
            }
            else if(hitLocation.Template.Name.Contains("Leg"))
            {
                hitLocation.Armor = 3;
                string otherName = hitLocation.Template.Name.Replace("Leg", "Foot");
                var otherHitLocation = soldier.Body.HitLocations
                    .First(hl => hl.Template.Name == otherName);
                otherHitLocation.Armor = 3;
                otherHitLocation.IsCybernetic = true;
            }
        }

        private void InitializeUnitTree()
        {
            if (!UnitTreeView.Initialized)
            {
                BuildUnitTree(UnitTreeView,
                              GameSettings.Chapter.OrderOfBattle,
                              GameSettings.Chapter.PlayerSoldierMap,
                              GameSettings.Chapter.SquadMap);
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
            foreach(PlayerSoldier marine in GameSettings.Chapter.PlayerSoldierMap.Values)
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
            List<Tuple<int, string, string>> memberList = _selectedSquad.Members.Select(s => new Tuple<int, string, string>(s.Id, s.Type.Name, s.ToString())).ToList();
            ApothecaryView.ReplaceSquadMemberContent(memberList);
            ApothecaryView.ReplaceSelectedSoldierText(GenerateSquadSummary(_selectedSquad));
        }

        private void SoldierSelected(int soldierId)
        {
            ISoldier selected = _selectedSquad.Members.First(s => s.Id == soldierId);
            ApothecaryView.ReplaceSelectedSoldierText(GenerateSoldierSummary(selected));
        }

        private string GenerateSquadSummary(Squad selectedSquad)
        {
            byte woundedSoldiers = 0;
            byte soldiersMissingBodyParts = 0;
            byte maxRecoveryTime = 0;
            byte unfitSoldiers = 0;
            foreach(ISoldier soldier in selectedSquad.Members)
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

        private string GenerateSoldierSummary(ISoldier selectedSoldier)
        {
            string summary = selectedSoldier.Name + "\n";
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
                summary += selectedSoldier.Name +
                    " will be unable to perform field duties until receiving cybernetic replacements\n";
            }
            else if(recoveryTime > 0)
            {
                summary += selectedSoldier.Name +
                    " requires " + recoveryTime.ToString() + " weeks to be fully fit for duty\n";
            }
            else
            {
                summary += selectedSoldier.Name +
                    " is fully fit and ready to serve the Emperor\n";
            }
            return summary;
        }
    }
}