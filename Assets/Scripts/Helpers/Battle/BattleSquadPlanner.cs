using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Iam.Scripts.Helpers.Battle.Actions;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;


namespace Iam.Scripts.Helpers.Battle
{
    public class BattleSquadPlanner
    {
        private readonly BattleGrid _grid;
        private readonly ConcurrentBag<IAction> _actionBag;
        private readonly Dictionary<int, BattleSquad> _opposingSoldierIdSquadMap;

        public BattleSquadPlanner(BattleGrid grid, Dictionary<int, BattleSquad> opposingSoldierIdSquadMap, ConcurrentBag<IAction> actionBag)
        {
            _grid = grid;
            _opposingSoldierIdSquadMap = opposingSoldierIdSquadMap;
            _actionBag = actionBag;
        }

        public void PrepareActions(BattleSquad squad)
        {
            int retreatVotes = 0;
            int advanceVotes = 0;
            int standVotes = 0;
            int chargeVotes = 0;
            // need some concept of squad disposition... stance, whether they're actively aiming
            // determine closest enemy
            // determine our optimal range
            // determine closest enemy optimal range
            // if the enemy wants to advance, we want to stay put, and vice versa
            // if we both want to get closer or both want to stay put, it's more interesting
            if (squad.IsInMelee)
            {
                // it doesn't really matter what the soldiers want to do, it's time to fight
                foreach(Soldier soldier in squad.Squad)
                {
                    AddMeleeActionsToBag(soldier);
                }
            }
            else
            {
                foreach (Soldier soldier in squad.Squad)
                {
                    int closestSoldierId;
                    float distance = _grid.GetNearestEnemy(soldier, squad.IsPlayerSquad, out closestSoldierId);
                    BattleSquad closestSquad = _opposingSoldierIdSquadMap[closestSoldierId];
                    float targetSize = closestSquad.GetAverageSize();
                    float targetArmor = closestSquad.GetAverageArmor();
                    float targetCon = closestSquad.GetAverageConstitution();
                    float preferredHitDistance = GetOptimalDistance(soldier, targetSize, targetArmor, targetCon);
                    if (preferredHitDistance == -1)
                    {
                        // this soldier wants to run
                        retreatVotes++;
                    }
                    else
                    {
                        float targetPreferredDistance = GetOptimalDistance(closestSquad.GetRandomSquadMember(), soldier.Size, soldier.Armor.Template.ArmorProvided, soldier.Constitution);


                        if (preferredHitDistance == 0)
                        {
                            advanceVotes++;
                            chargeVotes++;
                        }
                        else if(preferredHitDistance < targetPreferredDistance)
                        {
                            // advance
                            advanceVotes++;
                        }
                        else
                        {
                            // don't advance
                            standVotes++;
                        }
                    }
                }

                if (advanceVotes > standVotes && advanceVotes > retreatVotes)
                {
                    if (chargeVotes >= advanceVotes / 2)
                    {
                        foreach (Soldier soldier in squad.Squad)
                        {
                            AddChargeActionsToBag(soldier);
                        }
                    }
                    else
                    {
                        foreach (Soldier soldier in squad.Squad)
                        {
                            AddAdvancingActionsToBag(soldier);
                        }
                    }
                }
                else if (retreatVotes > standVotes && retreatVotes > advanceVotes)
                {
                    foreach (Soldier soldier in squad.Squad)
                    {
                        AddRetreatingActionsToBag(soldier, squad);
                    }
                }
                else
                {
                    foreach (Soldier soldier in squad.Squad)
                    {
                        AddStandingActionsToBag(soldier);
                    }
                }
            }
        }

        private void AddStandingActionsToBag(Soldier soldier)
        {
        }

        private void AddAdvancingActionsToBag(Soldier soldier)
        {

        }

        private void AddRetreatingActionsToBag(Soldier soldier, BattleSquad soldierSquad)
        {
            Tuple<int, int> currentPosition = _grid.GetSoldierPosition(soldier.Id, soldierSquad.IsPlayerSquad);;

            float moveSpeed = DetermineMoveSpeed(soldier);

            int newY = (int)(soldierSquad.IsPlayerSquad ? currentPosition.Item2 - moveSpeed : currentPosition.Item2 + moveSpeed);

            _actionBag.Add(new MoveAction(soldier, soldierSquad.IsPlayerSquad, _grid, new Tuple<int, int>(0, newY)));

            // determine if soldier will shoot as he falls back
            int closestEnemyId;
            float range = _grid.GetNearestEnemy(soldier, soldierSquad.IsPlayerSquad, out closestEnemyId);
            BattleSquad targetSquad = _opposingSoldierIdSquadMap[closestEnemyId];
            RangedWeapon weapon = ShouldShootAtRange(soldier, targetSquad, range, false, true, 0);
            if(weapon != null)
            {
                _actionBag.Add(new ShootAction(soldier, weapon, targetSquad));
            }
        }

        private void AddChargeActionsToBag(Soldier soldier)
        {

        }

        private void AddMeleeActionsToBag(Soldier soldier)
        {
        }

        private float DetermineMoveSpeed(Soldier soldier)
        {
            // TODO: if leg injuries, slow soldier down
            float baseMoveSpeed = soldier.MoveSpeed;
            //soldier.Body.HitLocations.Where(hl => hl)
            return baseMoveSpeed;
        }

        private float GetOptimalDistance(Soldier soldier, float targetSize, float targetArmor, float targetCon)
        {
            int freeHands = soldier.FreeHands;
            if(freeHands == 0)
            {
                // with no hands free, there's not much combat left for this soldier
                return -1;
            }
            float range = 0;
            var weapons = soldier.RangedWeapons.OrderByDescending(w => w.Template.MaximumDistance);
            foreach(RangedWeapon weapon in weapons)
            {
                float hitRange = GetHitDistance(soldier, weapon, targetSize, freeHands);
                float damRange = GetKillDistance(weapon, targetArmor, targetCon);
                float minVal = Mathf.Min(hitRange, damRange);
                if (minVal > range) range = minVal;
            }
            return range;
        }

        private float GetHitDistance(Soldier soldier, RangedWeapon weapon, float targetSize, int freeHands)
        {
            var skill = soldier.Skills[weapon.Template.RelatedSkill.Id];
            float baseTotal = GetStatForSkill(soldier, skill.BaseSkill) + skill.SkillBonus + CalculateSizeModifier(targetSize);

            if (weapon.Template.Location == EquipLocation.TwoHand && freeHands == 1)
            {
                // unless the soldier is strong enough, the weapon can't be used one-handed
                if (weapon.Template.RequiredStrength * 1.5f > soldier.Strength) return 0;
                if (weapon.Template.RequiredStrength * 2 > soldier.Strength)
                {
                    baseTotal -= (weapon.Template.RequiredStrength * 2) - soldier.Strength;
                }
            }

            // we'd like to get to a range where at least 1 bullet will hit more often than not
            // +1 for all-out attack, - ROF after the first shot
            // z value of 0.43 is 
            baseTotal = baseTotal + 1 + weapon.Template.Accuracy;
            baseTotal = baseTotal + CalculateRateOfFireModifier(weapon.Template.RateOfFire);
            if (baseTotal < 10.5) return 0;
            return GetRangeForModifier(10.5f - baseTotal);
        }

        private float GetKillDistance(RangedWeapon weapon, float targetArmor, float targetCon)
        {
            // if range doesn't matter for damage, we can just limit on hitting 
            if (!weapon.Template.DoesDamageDegradeWithRange) return weapon.Template.MaximumDistance;
            float effectiveArmor = targetArmor * weapon.Template.ArmorMultiplier;
            
            // if there's no chance of doing a wound, maybe we should run?
            if (weapon.Template.BaseStrength * 6 < effectiveArmor) return -1;
            //if we can't kill in one shot at point blank range, we still need to get as close as possible to have the best chance of taking the target down
            if ((weapon.Template.BaseStrength * 6 - effectiveArmor) * weapon.Template.PenetrationMultiplier < targetCon) return 0;
            // find the range with a 1/3 chance of a killshot
            float distanceRatio = 1 - (((targetCon / weapon.Template.PenetrationMultiplier) + effectiveArmor) / (4.25f * weapon.Template.BaseStrength));
            if (distanceRatio < 0) return 0;
            return weapon.Template.MaximumDistance * distanceRatio;
        }

        private RangedWeapon ShouldShootAtRange(Soldier soldier, BattleSquad opposingSquad, float range, bool useAccuracy, bool useBulk, float modifiers)
        {
            float sizeMod = CalculateSizeModifier(opposingSquad.GetAverageSize());
            float armor = opposingSquad.GetAverageArmor();
            float con = opposingSquad.GetAverageConstitution();
            RangedWeapon bestWeapon = null;
            float bestAccuracy = -1000;
            float bestDamage = -1000;
            foreach(RangedWeapon weapon in soldier.RangedWeapons.OrderByDescending(w => w.Template.BaseStrength))
            {
                float expectedDamage = CalculateExpectedDamage(weapon, range, armor, con);
                // if not likely to break through armor, there's little point
                if (expectedDamage > 0)
                {
                    // see if the hit chance is reasonable
                    float rangeMod = CalculateRangeModifier(range);
                    float rofMod = CalculateRateOfFireModifier(weapon.Template.RateOfFire);
                    float weaponSkill = GetWeaponSkill(soldier, weapon.Template);
                    if (useAccuracy) modifiers += weapon.Template.Accuracy;
                    if (useBulk) modifiers -= weapon.Template.Bulk;
                    float total = weaponSkill + rofMod + rangeMod + modifiers;
                    if (total >= 6.66f)
                    {
                        if (expectedDamage > bestDamage)
                        {
                            bestAccuracy = total;
                            bestDamage = expectedDamage;
                            bestWeapon = weapon;
                        }
                    }
                }
            }
            return bestWeapon;
        }

        private float CalculateExpectedDamage(RangedWeapon weapon, float range, float armor, float con)
        {
            float effectiveStrength =
                weapon.Template.DoesDamageDegradeWithRange ?
                    weapon.Template.BaseStrength * (1 - (range / weapon.Template.MaximumDistance)) :
                    weapon.Template.BaseStrength;
            return ((effectiveStrength * 4.25f) - armor) / con; 
        }

        private float GetWeaponSkill(Soldier soldier, WeaponTemplate template)
        {
            Skill skill = soldier.Skills[template.RelatedSkill.Id];
            return GetStatForSkill(soldier, skill.BaseSkill) + skill.SkillBonus;
        }

        private float GetStatForSkill(Soldier soldier, BaseSkill skill)
        {
            switch (skill.BaseAttribute)
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
        
        private float GetRangeForModifier(float modifier)
        {
            return 2 * Mathf.Exp(-modifier / 2.4663f);
        }

        private float CalculateRangeModifier(float range)
        {
            // 
            return 2.4663f * Mathf.Log(2 / range);
        }

        private float CalculateSizeModifier(float size)
        {
            // this is just the opposite of the range modifier
            return -CalculateRangeModifier(size);
        }

        private float CalculateRateOfFireModifier(int rateOfFire)
        {
            if (rateOfFire == 1) return 0;
            return Mathf.Log(rateOfFire, 2);
        }
    }
}
