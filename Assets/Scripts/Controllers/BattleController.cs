﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        public UnityEvent OnBattleComplete;
        public BattleView BattleView;
        
        private List<BattleSquad> _playerSquads;
        private List<BattleSquad> _opposingSquads;
        private Dictionary<int, Soldier> _playerSoldiers;
        private Dictionary<int, Soldier> _opposingSoldiers;
        
        private BattleGrid _grid;
        private int _turnNumber;

        private const int MAP_WIDTH = 100;
        private const int MAP_HEIGHT = 500;
        private const bool VERBOSE = true;
        private const int GRID_SCALE = 1;


        public BattleController()
        {
            _playerSquads = new List<BattleSquad>();
            _opposingSquads = new List<BattleSquad>();
            _playerSoldiers = new Dictionary<int, Soldier>();
            _opposingSoldiers = new Dictionary<int, Soldier>();
        }

        public void GalaxyController_OnBattleStarted(Planet planet)
        {
            BattleView.gameObject.SetActive(true);
            BattleView.Clear();
            BattleView.SetMapSize(new Vector2(MAP_WIDTH, MAP_HEIGHT));
            _turnNumber = 0;
            _grid = new BattleGrid(MAP_WIDTH, MAP_HEIGHT);
            // assume, for now, that space marines will be one of the two factions
            int currentBottom = 0;
            int currentLeft = 0;
            foreach (Unit unit in planet.FactionGroundUnitListMap[TempFactions.Instance.SpaceMarines.Id])
            {
                // start in bottom left 
                if(!unit.IsInReserve)
                {
                    PlacePlayerUnit(unit, currentLeft, currentBottom);
                }
            }
            BattleView.NextStepButton.SetActive(true);
        }

        public void RunBattleTurn()
        {
            // don't worry about unit AI yet... just have them each move one and fire
            // TODO: handle initiative
            if (_playerSquads[0].Squad.Length > 0 && _opposingSquads[0].Squad.Length > 0)
            {
                _turnNumber++;
                Log(true, "Turn " + _turnNumber.ToString());
                TakeAction(_playerSquads[0], true, _grid);
                if (_opposingSquads[0].Squad.Length > 0)
                {
                    TakeAction(_opposingSquads[0], false, _grid);
                }
                if (_playerSquads[0].Squad.Length == 0 && _opposingSquads[0].Squad.Length == 0)
                {
                    Log(true, "One side destroyed, battle over");
                    // update View button
                    BattleView.NextStepButton.SetActive(false);
                }
                Log(true, "----------");

                UpdateInjuryTrackers();
            }
        }

        private void UpdateInjuryTrackers()
        {
            BattleView.OverwritePlayerWoundTrack(GetSquadInjuryText(_playerSquads[0]));
            BattleView.OverwriteOpposingWoundTrack(GetSquadInjuryText(_opposingSquads[0]));
        }

        private void PlacePlayerUnit(Unit unit, int left, int bottom)
        {
            BattleSquad squad = new BattleSquad(unit.Id, unit.Name, true, unit.Members.ToArray());
            Tuple<int, int> squadSize = squad.GetSquadBoxSize();
            _grid.PlaceSquad(squad, left, bottom);
            BattleView.AddSquad(unit.Id, unit.Name, new Vector2(left, bottom), new Vector2(squadSize.Item1, squadSize.Item2));
        }

        private string GetSquadInjuryText(BattleSquad squad)
        {
            string report = "";
            foreach(Soldier soldier in squad.Squad)
            {
                report += soldier.ToString() + "\n";
                foreach(HitLocation hl in soldier.Body.HitLocations)
                {
                    report += hl.ToString() + "\n";
                }
                report += "\n";
            }
            return report;
        }

        private void Log(bool isMessageVerbose, string text)
        {
            if (VERBOSE || !isMessageVerbose)
            {
                BattleView.LogToBattleLog(text);
            }
        }

        private void TakeAction(BattleSquad squad, bool isPlayerSquad, BattleGrid grid)
        {
            int enemyId;
            // for now, if anyone in the squad starts shooting, the squad shoots
            float range = grid.GetNearestEnemy(squad.Squad[0], isPlayerSquad, out enemyId) * GRID_SCALE;
            List<ChosenWeapon> bestWeapons = squad.GetWeaponsForRange(range);
            BattleSquad enemySquad = squad.IsPlayerSquad ? _opposingSquads[enemyId] : _playerSquads[enemyId];
            if(ShouldFire(bestWeapons, enemySquad, range))
            {
                Log(true, squad.Name + " at range " + range.ToString() + " elected to shoot");
                Shoot(bestWeapons, enemySquad, range, 0f);
            }
            else
            {
                Log(true, squad.Name + " at range " + range.ToString() + " elected to move");
                MoveSquad(grid, squad, isPlayerSquad);
            }
        }

        private void MoveSquad(BattleGrid grid, BattleSquad squad, bool isPlayerSquad)
        {
            int moveAmount = squad.GetSquadMove() * 3 / GRID_SCALE;
            grid.MoveSquad(squad, 0, isPlayerSquad ? moveAmount : -moveAmount);
        }

        private bool ShouldFire(List<ChosenWeapon> weapons, BattleSquad enemy, float range)
        {
            if(weapons.Count < 1)
            {
                return false;
            }
            else
            {
                return IsWorthShooting(weapons, enemy, range);
            }
        }

        private bool IsWorthShooting(List<ChosenWeapon> bestWeapons, BattleSquad enemy, float range)
        {
            foreach(ChosenWeapon weapon in bestWeapons)
            {
                int effectiveArmor = enemy.GetAverageArmor() - weapon.ActiveWeapon.Template.ArmorPiercing;
                float effectiveStrength = weapon.GetStrengthAtRange(range);
                float effectiveWound = enemy.GetAverageConstitution() / 10;
                //float shootingSkill = 
                // TODO: come up with a better prediction equation
                bool canWound = effectiveStrength * 5 > effectiveArmor + effectiveWound;
                if(canWound)
                {
                    if (weapon.GetAccuracyAtRange(range) > 6)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    
        private void Shoot(List<ChosenWeapon> weapons, BattleSquad target, float range, float coverModifier)
        {
            // figure out to-hit modifiers
            // distance + speed
            float rangeModifier = CalculateRangeModifier(range); // add target speed to 
            // size modifier is built into the target squad
            // weapon accuracy
            // cover
            foreach(ChosenWeapon weapon in weapons)
            {
                Soldier hitSoldier = target.GetRandomSquadMember();
                float totalModifier = weapon.ActiveWeapon.Template.Accuracy + rangeModifier + CalculateSizeModifier(hitSoldier.Size) + coverModifier;
                Log(true, "Total modifier to shot is " + totalModifier.ToString());
                // bracing
                // figure out number of shots fired
                for(int i = 0; i < weapon.ActiveWeapon.Template.RateOfFire; i++)
                {
                    Skill soldierSkill = weapon.Soldier.Skills[weapon.ActiveWeapon.Template.RelatedSkill.Id];

                    float skillTotal = soldierSkill.SkillBonus + GetStatForSkill(weapon.Soldier, soldierSkill);
                    float roll = 10.5f + (3.0f * (float)Gaussian.NextGaussianDouble());
                    float marginOfSuccess = skillTotal + totalModifier - roll;
                    Log(true, "Modified roll total is " + marginOfSuccess.ToString());
                    if(marginOfSuccess > 0)
                    {
                        // a hit!
                        ResolveHit(weapon, hitSoldier, target, range);
                    }
                }
            }
        }

        private float GetStatForSkill(Soldier soldier, Skill skill)
        {
            switch(skill.BaseSkill.BaseAttribute)
            {
                case SkillAttribute.Dexterity:
                    return soldier.Dexterity;
                case SkillAttribute.Intelligence:
                    return soldier.Intelligence;
                case SkillAttribute.Ego:
                    return soldier.Ego;
                case SkillAttribute.Presence:
                    return soldier.Presence;
                default:
                    return soldier.Dexterity;
            }
        }

        private void ResolveHit(ChosenWeapon weapon, Soldier hitSoldier, BattleSquad target, float range)
        {
            // TODO: handle hit location
            HitLocation location = DetermineHitLocation(hitSoldier);
            Log(true, hitSoldier.ToString() + " hit in " + location.Template.Name);
            if ((short)location.Wounds >= (short)location.Template.WoundLimit * 2)
            {
                Log(true, location.Template.Name + " already shot off");
            }
            else
            {
                // TODO: should hit margin of success affect damage? If so, how much?
                float damage = weapon.GetStrengthAtRange(range) * (3.5f + ((float)Gaussian.NextGaussianDouble() * 1.75f));
                Log(true, damage.ToString() + "Damage rolled");
                float effectiveArmor = hitSoldier.Armor.Template.ArmorProvided - weapon.ActiveWeapon.Template.ArmorPiercing;
                if (effectiveArmor < 0) effectiveArmor = 0;
                float penDamage = damage - effectiveArmor;
                if (penDamage > 0)
                {
                    float totalDamage = penDamage * weapon.ActiveWeapon.Template.PenetrationMultiplier;
                    HandleWound(totalDamage, hitSoldier, location, target);
                }
            }
        }

        private HitLocation DetermineHitLocation(Soldier soldier)
        {
            // we're using the "lottery ball" approach to randomness here, where each point of probability
            // for each available body party defines the size of the random linear distribution
            // TODO: factor in cover/body position
            // 
            int roll = UnityEngine.Random.Range(1, soldier.Body.TotalProbability);
            foreach(HitLocation location in soldier.Body.HitLocations)
            {
                if(roll < location.Template.HitProbability)
                {
                    return location;
                }
                else
                {
                    // this is basically an easy iterative way to figure out which body part on the "chart" the roll matches
                    roll -= location.Template.HitProbability;
                }
            }
            // this should never happen
            return null;
        }

        private void HandleWound(float totalDamage, Soldier hitSoldier, HitLocation location, BattleSquad soldierSquad)
        {
            Wounds wound;
            // check location for natural armor
            totalDamage -= location.Template.NaturalArmor;
            // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
            // multiply damage by location modifier
            totalDamage *= location.Template.DamageMultiplier;
            // compare total damage to soldier Constitution
            float ratio = totalDamage / hitSoldier.Constitution;
            if(ratio >= 2.0f)
            {
                wound = Wounds.Unsurvivable;
            }
            else if(ratio >= 1.0f)
            {
                wound = Wounds.Critical;
            }
            else if(ratio >= 0.67f)
            {
                wound = Wounds.Severe;
            }
            else if(ratio >= 0.5f)
            {
                wound = Wounds.Serious;
            }
            else if(ratio >= 0.33f)
            {
                wound = Wounds.Major;
            }
            else if(ratio >= 0.2f)
            {
                wound = Wounds.Moderate;
            }
            else if(ratio >= 0.1f)
            {
                wound = Wounds.Minor;
            }
            else
            {
                wound = Wounds.Negligible;
            }
            Log(true, "The hit causes a " + wound.ToFriendlyString() + " wound");
            location.Wounds = (byte)location.Wounds + wound;
            if((short)location.Wounds >= (short)location.Template.WoundLimit * 2)
            {
                Log(true, location.Template.Name + " is blown off");
                if((short)location.Wounds >= (short)Wounds.Unsurvivable * 2)
                {
                    Log(false, hitSoldier.ToString() + " died");
                    soldierSquad.RemoveSoldier(hitSoldier);
                }
            }
            if(location.Template.Name == "Left Foot" || location.Template.Name == "Right Foot" 
                || location.Template.Name == "Left Leg" || location.Template.Name == "Right Leg")
            {
                if(location.Wounds >= location.Template.WoundLimit)
                {
                    Log(false, hitSoldier.ToString() + " has fallen and can't get up");
                    soldierSquad.RemoveSoldier(hitSoldier);
                }
            }
            if (location.Wounds >= Wounds.Critical)
            {
                // TODO: need to start making consciousness checks
            }
            if(wound >= Wounds.Critical)
            {
                // make death check
                CheckForDeath(hitSoldier, soldierSquad);
            }
            if(wound >= Wounds.Unsurvivable)
            {
                // make additional death check
                CheckForDeath(hitSoldier, soldierSquad);
            }
        }

        private void CheckForDeath(Soldier soldier, BattleSquad squad)
        {
            float roll = 10.5f + (3.0f * (float)Gaussian.NextGaussianDouble());
            if (roll > soldier.Constitution)
            {
                Log(false, soldier.ToString() + " died");
                squad.RemoveSoldier(soldier);
            }
        }

        private float CalculateRangeModifier(float range)
        {
            return 2.4663f * (float)Math.Log(2 / range);
        }

        private float CalculateSizeModifier(float size)
        {
            // this is just the opposite of the range modifier
            return -CalculateRangeModifier(size);
        }
    }
}
