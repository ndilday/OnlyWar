using System;
using System.Collections.Generic;
using UnityEngine;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        public BattleView BattleView;
        private const bool VERBOSE = true;
        private BattleSquad _playerForce;
        private BattleSquad _opposingForce;
        private BattleGrid _grid;
        private const int GRID_SCALE = 2;
        private int _turnNumber;

        public void Test()
        {
            // we're going to assume for now that squads fit into 2yd x 2yd spaces and turns are 2 seconds
            // A carefully moving unencumbered Marine can cover 6yrds in 2 seconds, or 18 yards if running/charging
            BattleView.ClearBattleLog();
            _turnNumber = 0;
            Log(true, "Initiating Test Battle");
            _grid = new BattleGrid(1, 250);
            // generate a Space Marine armed with a Boltgun in Power Armor on each side.
            _playerForce = TempGenerateSmallMarineSquad(0, "Force Blue");
            _opposingForce = TempGenerateSmallTyranidWarriorSquad(1, "Force Red");
            // Start them 800 yards apart
            _grid.PlacePlayerSquad(_playerForce, 0, 0);
            _grid.PlaceOpposingSquad(_opposingForce, 0, 249);
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

                UpdateInjuryTrackers();
            }
        }

        private void UpdateInjuryTrackers()
        {
            BattleView.OverwritePlayerWoundTrack(GetSquadInjuryText(_playerForce));
            BattleView.OverwriteOpposingWoundTrack(GetSquadInjuryText(_opposingForce));
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

        private BattleSquad TempGenerateSingleMarineSquad(int id, string name)
        {
            SpaceMarine[] soldiers = new SpaceMarine[1];
            soldiers[0] = SoldierFactory.Instance.GenerateNewSoldier<SpaceMarine>(TempSpaceMarineTemplate.Instance);
            soldiers[0].Armor = new Armor
            {
                Template = ImperialEquippables.Instance.PowerArmor
            };
            soldiers[0].Weapons.Add(new Weapon
            {
                Template = ImperialEquippables.Instance.Boltgun
            });
            // space marines are about 2.2m, or a little over 7' tall
            return new BattleSquad(id, name, soldiers);
        }

        private BattleSquad TempGenerateSmallMarineSquad(int id, string name)
        {
            SpaceMarine[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers<SpaceMarine>(6, TempSpaceMarineTemplate.Instance);
            foreach(SpaceMarine marine in soldiers)
            {
                marine.FirstName = TempNameGenerator.GetName();
                marine.LastName = TempNameGenerator.GetName();
                marine.Armor = new Armor
                {
                    Template = ImperialEquippables.Instance.PowerArmor
                };
                marine.Weapons.Add(new Weapon
                {
                    Template = ImperialEquippables.Instance.Boltgun
                });
            }
            return new BattleSquad(id, name, soldiers);
        }

        private BattleSquad TempGenerateSmallTyranidWarriorSquad(int id, string name)
        {
            Tyranid[] soldiers = SoldierFactory.Instance.GenerateNewSoldiers<Tyranid>(3, TempTyranidWarriorTemplate.Instance);
            foreach(Tyranid warrior in soldiers)
            {
                warrior.Armor = new Armor
                {
                    Template = TempEquipment.Instance.Chitin
                };
                warrior.Weapons.Add(new Weapon
                {
                    Template = TempEquipment.Instance.Deathspitter
                });
            }
            return new BattleSquad(id, name, soldiers);
        }

        private void TakeAction(BattleSquad squad, bool isPlayerSquad, BattleGrid grid)
        {
            KeyValuePair<Tuple<int, int>, BattleSquad> enemy;
            float range = grid.GetNearestEnemy(squad, isPlayerSquad, out enemy) * GRID_SCALE;
            List<ChosenWeapon> bestWeapons = squad.GetWeaponsForRange(range);
            if(ShouldFire(bestWeapons, enemy.Value, range))
            {
                Log(true, squad.Name + " at range " + range.ToString() + " elected to shoot");
                Shoot(bestWeapons, enemy.Value, range, 0f);
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
            grid.MoveSquad(squad, isPlayerSquad, 0, isPlayerSquad ? moveAmount : -moveAmount);
        }

        private bool ShouldFire(List<ChosenWeapon> weapons, BattleSquad enemy, float range)
        {
            if(weapons.Count == 0)
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
