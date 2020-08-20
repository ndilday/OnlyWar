using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using System.IO.Compression;
using Iam.Scripts.Views;
using System.ComponentModel;

namespace Iam.Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        public BattleView BattleView;
        private const bool VERBOSE = true;
        private BattleSquad _playerForce;
        private BattleSquad _opposingForce;
        private BattleGrid _grid;
        private const int GRID_SCALE = 10;
        private int _turnNumber;

        public void Test()
        {
            // we're going to start by assuming grid spaces are 10yd x 10yd and turns are 2 seconds. 
            // A carefully moving unencumbered Marine can cover 10yrds in 2 seconds, or 20 yards if running/charging
            BattleView.ClearBattleLog();
            _turnNumber = 0;
            Log(true, "Initiating Test Battle");
            _grid = new BattleGrid(1, 80);
            // generate a Space Marine armed with a Boltgun in Power Armor on each side.
            _playerForce = TempGenerateSingleMarineSquad(0, "Force Blue");
            _opposingForce = TempGenerateSingleMarineSquad(1, "Force Red");
            // Start them 800 yards apart
            _grid.PlacePlayerSquad(_playerForce, 0, 0);
            _grid.PlaceOpposingSquad(_opposingForce, 0, 79);
            BattleView.NextStepButton.SetActive(true);
        }

        public void RunBattleTurn()
        {
            // don't worry about unit AI yet... just have them each move one and fire
            // TODO: handle initiative
            if (_playerForce.Squad.Length > 0 && _opposingForce.Squad.Length > 0)
            {
                _turnNumber++;
                Log(true, "Turn " + _turnNumber.ToString());
                TakeAction(_playerForce, true, _grid);
                if (_opposingForce.Squad.Length > 0)
                {
                    TakeAction(_opposingForce, false, _grid);
                }
                if (_playerForce.Squad.Length == 0 && _opposingForce.Squad.Length == 0)
                {
                    Log(true, "One side destroyed, battle over");
                    // update View button
                    BattleView.NextStepButton.SetActive(false);
                }
                Log(true, "----------");
            }
        }

        private void Log(bool isMessageVerbose, string text)
        {
            if (VERBOSE || !isMessageVerbose)
            {
                BattleView.LogToBattleLog(text);
            }
        }

        public BattleSquad TempGenerateSingleMarineSquad(int id, string name)
        {
            SoldierFactory factory = new SoldierFactory();
            Soldier[] soldiers = new Soldier[1];
            soldiers[0] = factory.GenerateNewSoldier(1, "666 Test");
            soldiers[0].Armor = new Armor
            {
                Template = TempEquipment.Instance.PowerArmor
            };
            soldiers[0].Weapons.Add(new Weapon
            {
                Template = TempEquipment.Instance.Boltgun
            });
            // space marines are about 2.2m, or a little over 7' tall
            return new BattleSquad(id, name, soldiers, 2.4f);
        }

        private void TakeAction(BattleSquad squad, bool isPlayerSquad, BattleGrid grid)
        {
            KeyValuePair<Tuple<int, int>, BattleSquad> enemy;
            float range = grid.GetNearestEnemy(squad, isPlayerSquad, out enemy) * GRID_SCALE;
            List<ChosenWeapon> bestWeapons = squad.GetWeaponsForRange(range);
            if(isPlayerSquad && range > 100)
            {
                Log(true, squad.Name + " at range " + range.ToString() + " elected to move");
                grid.MoveSquad(squad, isPlayerSquad, 0, isPlayerSquad ? 1 : -1);
            }
            else if(ShouldFire(bestWeapons, enemy.Value, range))
            {
                Log(true, squad.Name + " at range " + range.ToString() + " elected to shoot");
                Shoot(bestWeapons, enemy.Value, range, 0f);
            }
            else
            {
                Log(true, squad.Name + " at range " + range.ToString() + " elected to move");
                grid.MoveSquad(squad, isPlayerSquad, 0, isPlayerSquad ? 1 : -1);
            }
        }

        private bool ShouldFire(List<ChosenWeapon> weapons, BattleSquad enemy, float range)
        {
            if(weapons.Count == 0)
            {
                return false;
            }
            else
            {
                int enemyArmor = enemy.GetAverageArmor();
                return IsWorthShooting(weapons, enemyArmor, range);
            }
        }

        private bool IsWorthShooting(List<ChosenWeapon> bestWeapons, int enemyArmor, float range)
        {
            foreach(ChosenWeapon weapon in bestWeapons)
            {
                int effectiveArmor = enemyArmor - weapon.ActiveWeapon.Template.ArmorPiercing;
                // TODO: come up with a better prediction equation
                bool canPen = weapon.ActiveRangeBand.Strength * 6 > effectiveArmor;
                bool canHit = weapon.ActiveRangeBand.Accuracy + CalculateRangeModifier(range) > -14.0f;
                if (canHit && canPen) return true;
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
            float totalModifier = rangeModifier + CalculateSizeModifier(target.Height) + coverModifier;
            foreach(ChosenWeapon weapon in weapons)
            {
                float modifier = weapon.ActiveRangeBand.Accuracy + totalModifier;
                Log(true, "Total modifier to shot is " + modifier.ToString());
                // bracing
                // figure out number of shots fired
                for(int i = 0; i < weapon.ActiveWeapon.Template.RateOfFire; i++)
                {
                    float roll = 10.5f + (3.0f * (float)Gaussian.NextGaussianDouble());
                    float marginOfSuccess = weapon.Soldier.Ranged + modifier - roll;
                    Log(true, "Modified roll total is " + marginOfSuccess.ToString());
                    if(marginOfSuccess > 0)
                    {
                        // a hit!
                        ResolveHit(weapon, target);
                    }
                }
            }
        }

        private void ResolveHit(ChosenWeapon weapon, BattleSquad target)
        {
            Soldier hitSoldier = target.GetRandomSquadMember();
            // TODO: handle hit location
            HitLocation location = DetermineHitLocation(hitSoldier);
            Log(true, "Hit in " + location.Name);
            if ((short)location.Wounds >= (short)location.WoundLimit * 2)
            {
                Log(true, location.Name + " already shot off");
            }
            else
            {
                // TODO: should hit margin of success affect damage? If so, how much?
                float damage = weapon.ActiveRangeBand.Strength * (3.5f + ((float)Gaussian.NextGaussianDouble() * 1.75f));
                Log(true, damage.ToString() + "Damage rolled");
                float penDamage = damage - hitSoldier.Armor.Template.ArmorProvided;
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
                if(roll < location.HitProbability)
                {
                    return location;
                }
                else
                {
                    // this is basically an easy iterative way to figure out which body part on the "chart" the roll matches
                    roll -= location.HitProbability;
                }
            }
            // this should never happen
            return null;
        }

        private void HandleWound(float totalDamage, Soldier hitSoldier, HitLocation location, BattleSquad soldierSquad)
        {
            Wounds wound;
            // check location for natural armor
            totalDamage -= location.NaturalArmor;
            // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
            // multiply damage by location modifier
            totalDamage *= location.DamageMultiplier;
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
            // TODO: handle
            if((short)location.Wounds >= (short)location.WoundLimit * 2)
            {
                Log(true, location.Name + " is blown off");
                if((short)location.Wounds >= (short)Wounds.Unsurvivable * 2)
                {
                    Log(false, hitSoldier.FirstName + " " + hitSoldier.LastName + " died");
                    soldierSquad.RemoveSoldier(hitSoldier);
                }
            }
            if(location.Wounds >= Wounds.Critical)
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
                Log(false, soldier.FirstName + " " + soldier.LastName + " died");
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
