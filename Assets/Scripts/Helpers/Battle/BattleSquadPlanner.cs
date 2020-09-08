using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Iam.Scripts.Helpers.Battle.Actions;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using System.Xml.Schema;

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
                foreach(BattleSoldier soldier in squad.Soldiers)
                {
                    AddMeleeActionsToBag(soldier);
                }
            }
            else
            {
                foreach (BattleSoldier soldier in squad.Soldiers)
                {
                    int closestSoldierId;
                    float distance = _grid.GetNearestEnemy(soldier.Soldier, out closestSoldierId);
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
                        float targetPreferredDistance = GetOptimalDistance(closestSquad.GetRandomSquadMember(), 
                                                                           soldier.Soldier.Size, 
                                                                           soldier.Armor.Template.ArmorProvided, 
                                                                           soldier.Soldier.Constitution);


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
                        foreach (BattleSoldier soldier in squad.Soldiers)
                        {
                            AddChargeActionsToBag(soldier);
                        }
                    }
                    else
                    {
                        foreach (BattleSoldier soldier in squad.Soldiers)
                        {
                            AddAdvancingActionsToBag(soldier);
                        }
                    }
                }
                else if (retreatVotes > standVotes && retreatVotes > advanceVotes)
                {
                    foreach (BattleSoldier soldier in squad.Soldiers)
                    {
                        AddRetreatingActionsToBag(soldier, squad);
                    }
                }
                else
                {
                    foreach (BattleSoldier soldier in squad.Soldiers)
                    {
                        AddStandingActionsToBag(soldier);
                    }
                }
            }
        }

        private void AddStandingActionsToBag(BattleSoldier soldier)
        {
            if(soldier.RangedWeapons.Count == 0)
            {
                Debug.Log("Soldier with no ranged weapons just standing around");
            }
            else if(soldier.EquippedRangedWeapons.Count == 0 && soldier.RangedWeapons.Count > 0)
            {
                int handsFree = soldier.HandsFree;
                // we're standing here without a readied ranged weapon; we should do something about that
                if(soldier.RangedWeapons.Count == 1 && handsFree >= 1)
                {
                    // the easiest case... ready our one ranged weapon
                    _actionBag.Add(new ReadyRangedWeaponAction(soldier, soldier.RangedWeapons[0]));
                }
                else if(soldier.RangedWeapons.Count > 1 && handsFree >= 1)
                {
                    // ugh, this is a decision with a lot of factors that will only come up rarely
                    // for now, let's go with the longer ranged weapon
                    _actionBag.Add(new ReadyRangedWeaponAction(soldier, soldier.RangedWeapons.OrderByDescending(w => w.Template.MaximumDistance).First()));

                }
            }
            // determine if soldier was already aiming and the target is still around and not in a melee
            else if (soldier.Aim != null && _opposingSoldierIdSquadMap.ContainsKey(soldier.Aim.Item1.Soldier.Id) && !soldier.Aim.Item1.IsInMelee)
            {
                // if the aim cannot be improved, go ahead and shoot
                if (soldier.Aim.Item3 == 2)
                {
                    _actionBag.Add(new ShootAction(soldier, soldier.Aim.Item2, soldier.Aim.Item1));
                }
                else
                {
                    // the aim can be improved
                    // current aim bonus is 1 for all-out attack, plus weapon accuracy, plus aim
                    float range = _grid.GetDistanceBetweenSoldiers(soldier.Soldier.Id, soldier.Aim.Item1.Soldier.Id);
                    float currentModifiers = soldier.Aim.Item2.Template.Accuracy + soldier.Aim.Item3 + 1;
                    // item1 is the pre-roll to-hit total; item2 is the expected ratio of damage to con, so 1 is a potential killshot
                    Tuple<float, float> resultEstimate = EstimateShootingResult(soldier, soldier.Aim.Item1, soldier.Aim.Item2, range, currentModifiers);
                    if (soldier.Aim.Item1.GetMoveSpeed() > range)
                    {
                        // it's about to attack, go ahead and shoot, you may not get another chance
                        _actionBag.Add(new ShootAction(soldier, soldier.Aim.Item2, soldier.Aim.Item1));

                    }
                    else if (resultEstimate.Item2 >= 1 && resultEstimate.Item1 >= -8.7f)
                    {
                        // there's a good chance of both hitting and killing, go ahead and shoot now
                        _actionBag.Add(new ShootAction(soldier, soldier.Aim.Item2, soldier.Aim.Item1));
                    }
                    else
                    {
                        // keep aiming
                        _actionBag.Add(new AimAction(soldier, soldier.Aim.Item1, soldier.Aim.Item2));
                    }
                }
            }
            else
            {
                int closestEnemyId;
                float range = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
                BattleSquad oppSquad = _opposingSoldierIdSquadMap[closestEnemyId];
                // decide whether to shoot or aim
                RangedWeapon weapon = ShouldShootAtRange(soldier, oppSquad, range, false, false, 0);
                if(weapon == null)
                {
                    // aim with longest ranged weapon
                    _actionBag.Add(new AimAction(soldier, oppSquad.GetRandomSquadMember(), soldier.EquippedRangedWeapons.OrderByDescending(w => w.Template.MaximumDistance).First()));

                }
                else
                {
                    _actionBag.Add(new ShootAction(soldier, weapon, oppSquad.GetRandomSquadMember()));
                }
            }
        }

        private void AddAdvancingActionsToBag(BattleSoldier soldier)
        {
            // for now advance toward closest enemy;
            // down the road, we may want to advance toward a rearward enemy, ignoring the closest enemy
            
            int closestEnemyId;
            Tuple<int, int> currentPosition = _grid.GetSoldierPosition(soldier.Soldier.Id);
            float distance = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
            float moveSpeed = soldier.GetMoveSpeed();
            if(distance < moveSpeed)
            {
                AddChargeActionsToBag(soldier);
            }
            else
            {
                Tuple<int, int> enemyPosition = _grid.GetSoldierPosition(closestEnemyId);
                Tuple<int, int> line = new Tuple<int, int>(enemyPosition.Item1 - currentPosition.Item1, enemyPosition.Item2 - currentPosition.Item2);
                // soldier can't get there in one move, advance as far as possible
                Tuple<int, int> realMove = CalculateMovementAlongLine(line, moveSpeed);
                _actionBag.Add(new MoveAction(soldier, _grid, realMove));

                // should the soldier shoot along the way?
                float range = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
                BattleSquad targetSquad = _opposingSoldierIdSquadMap[closestEnemyId];
                RangedWeapon weapon = ShouldShootAtRange(soldier, targetSquad, range, false, true, 0);
                if (weapon != null)
                {
                    _actionBag.Add(new ShootAction(soldier, weapon, targetSquad.GetRandomSquadMember()));
                }
            }
        }

        private void AddRetreatingActionsToBag(BattleSoldier soldier, BattleSquad soldierSquad)
        {
            Tuple<int, int> currentPosition = _grid.GetSoldierPosition(soldier.Soldier.Id);

            float moveSpeed = soldier.GetMoveSpeed();

            int newY = (int)(soldierSquad.IsPlayerSquad ? currentPosition.Item2 - moveSpeed : currentPosition.Item2 + moveSpeed);

            _actionBag.Add(new MoveAction(soldier, _grid, new Tuple<int, int>(0, newY)));

            // determine if soldier will shoot as he falls back
            int closestEnemyId;
            float range = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
            BattleSquad targetSquad = _opposingSoldierIdSquadMap[closestEnemyId];
            RangedWeapon weapon = ShouldShootAtRange(soldier, targetSquad, range, false, true, 0);
            if(weapon != null)
            {
                _actionBag.Add(new ShootAction(soldier, weapon, targetSquad.GetRandomSquadMember()));
            }
        }

        private void AddMeleeActionsToBag(BattleSoldier soldier)
        {
            // for now just attack, don't worry about cooler moves
            int closestEnemyId;
            float distance = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
            if (distance != 1) throw new InvalidOperationException("Attempting to melee with no adjacent enemy");
            BattleSoldier enemy = _opposingSoldierIdSquadMap[closestEnemyId].Soldiers.Single(s => s.Soldier.Id == closestEnemyId);
            _actionBag.Add(new MeleeAttackAction(soldier, enemy, false));
        }

        private void AddChargeActionsToBag(BattleSoldier soldier)
        {
            if(soldier.IsInMelee)
            {
                // determine what sort of manuver to make
                AddMeleeActionsToBag(soldier);
            }
            else
            {
                // get stuck in
                // move adjacent to nearest enemy
                int closestEnemyId;
                Tuple<int, int> currentPosition = _grid.GetSoldierPosition(soldier.Soldier.Id);
                float distance = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
                Tuple<int, int> enemyPosition = _grid.GetSoldierPosition(closestEnemyId);
                BattleSquad oppSquad = _opposingSoldierIdSquadMap[closestEnemyId];
                Tuple<int, int> newPos = _grid.GetClosestOpenAdjacency(currentPosition, enemyPosition);
                if (newPos == null)
                {
                    // find the next closest
                    // okay, this is one of those times where I made something because it made me feel smart,
                    // but it's probably unreadable so I should change it later
                    // basically, foreach soldier in the squad of the closest enemy, except the closest enemy (who we already checked)
                    // get their locations, and then sort it according to distance square
                    // PROTIP: SQRT is a relatively expensive operation, so sort by distance squares when it's about comparative, not absolute, distance
                    var map = oppSquad.Soldiers
                        .Where(s => s.Soldier.Id != closestEnemyId)
                        .Select(s => new Tuple<int, Tuple<int, int>>(s.Soldier.Id, _grid.GetSoldierPosition(s.Soldier.Id)))
                        .Select(t => new Tuple<int, Tuple<int, int>, Tuple<int, int>>(t.Item1, t.Item2, new Tuple<int, int>(t.Item2.Item1 - currentPosition.Item1, t.Item2.Item2 - currentPosition.Item2)))
                        .Select(u => new Tuple<int, Tuple<int, int>, int>(u.Item1, u.Item2, (u.Item3.Item1 * u.Item3.Item1 + u.Item3.Item2 * u.Item3.Item2)))
                        .OrderBy(u => u.Item3);
                    foreach(Tuple<int, Tuple<int, int>, int> soldierData in map)
                    {
                        newPos = _grid.GetClosestOpenAdjacency(currentPosition, soldierData.Item2);
                        if(newPos != null)
                        {
                            AddChargeActionsHelper(soldier, soldierData.Item1, currentPosition, Mathf.Sqrt(soldierData.Item3), oppSquad, newPos);
                            break;
                        }
                    }
                    if(newPos == null)
                    {
                        // we weren't able to find an enemy to get near, guess we try to find someone to shoot, instead?
                        Debug.Log("Soldier in squad engaged in melee couldn't find anyone to attack");
                        AddStandingActionsToBag(soldier);
                    }
                }
                else
                {
                    AddChargeActionsHelper(soldier, closestEnemyId, currentPosition, distance, oppSquad, newPos);
                }
            }
        }

        private void AddChargeActionsHelper(BattleSoldier soldier, int closestEnemyId, Tuple<int, int> currentPosition, float distance, BattleSquad oppSquad, Tuple<int, int> newPos)
        {
            Tuple<int, int> move = new Tuple<int, int>(newPos.Item1 - currentPosition.Item1, newPos.Item2 - currentPosition.Item2);
            float distanceSq = ((move.Item1 * move.Item1) + (move.Item2 + move.Item2));
            float moveSpeed = soldier.GetMoveSpeed();
            if (distanceSq > moveSpeed * moveSpeed)
            {
                // soldier can't get there in one move, advance as far as possible
                Tuple<int, int> realMove = CalculateMovementAlongLine(move, moveSpeed);
                _actionBag.Add(new MoveAction(soldier, _grid, realMove));
                
                // should the soldier shoot along the way?
                float range = _grid.GetNearestEnemy(soldier.Soldier, out closestEnemyId);
                BattleSquad targetSquad = _opposingSoldierIdSquadMap[closestEnemyId];
                RangedWeapon weapon = ShouldShootAtRange(soldier, targetSquad, range, false, true, 0);
                if (weapon != null)
                {
                    _actionBag.Add(new ShootAction(soldier, weapon, targetSquad.GetRandomSquadMember()));
                }
            }
            else
            {
                _actionBag.Add(new MoveAction(soldier, _grid, move));
                BattleSoldier target = oppSquad.Soldiers.Single(s => s.Soldier.Id == closestEnemyId);
                _actionBag.Add(new MeleeAttackAction(soldier, target, distance <= 2));
            }
        }

        private float GetOptimalDistance(BattleSoldier soldier, float targetSize, float targetArmor, float targetCon)
        {
            int freeHands = soldier.Soldier.FunctioningHands;
            if(freeHands == 0)
            {
                // with no hands free, there's not much combat left for this soldier
                return -1;
            }
            float range = 0;
            var weapons = soldier.EquippedRangedWeapons.OrderByDescending(w => w.Template.MaximumDistance);
            foreach(RangedWeapon weapon in weapons)
            {
                float hitRange = GetHitDistance(soldier.Soldier, weapon, targetSize, freeHands);
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

        private RangedWeapon ShouldShootAtRange(BattleSoldier soldier, BattleSquad opposingSquad, float range, bool useAccuracy, bool useBulk, float modifiers)
        {
            float sizeMod = CalculateSizeModifier(opposingSquad.GetAverageSize());
            float armor = opposingSquad.GetAverageArmor();
            float con = opposingSquad.GetAverageConstitution();
            RangedWeapon bestWeapon = null;
            float bestAccuracy = -1000;
            float bestDamage = -1000;
            foreach(RangedWeapon weapon in soldier.EquippedRangedWeapons.OrderByDescending(w => w.Template.BaseStrength))
            {
                float expectedDamage = CalculateExpectedDamage(weapon, range, armor, con);
                // if not likely to break through armor, there's little point
                if (expectedDamage > 0)
                {
                    // see if the hit chance is reasonable
                    float rangeMod = CalculateRangeModifier(range);
                    float rofMod = CalculateRateOfFireModifier(weapon.Template.RateOfFire);
                    float weaponSkill = GetWeaponSkill(soldier.Soldier, weapon.Template);
                    if (useAccuracy) modifiers += weapon.Template.Accuracy;
                    if (useBulk) modifiers -= weapon.Template.Bulk;
                    float total = weaponSkill + rofMod + rangeMod + modifiers;
                    if (total >= -12.5f)
                    {
                        // about a 1/4 chance of hitting
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

        private Tuple<float, float> EstimateShootingResult(BattleSoldier soldier, BattleSoldier target, RangedWeapon weapon, float range, float modifiers)
        {
            float sizeMod = CalculateSizeModifier(target.Soldier.Size);
            float armor = target.Armor.Template.ArmorProvided;
            float con = target.Soldier.Constitution;
            float expectedDamage = CalculateExpectedDamage(weapon, range, armor, con);
            float rangeMod = CalculateRangeModifier(range);
            float rofMod = CalculateRateOfFireModifier(weapon.Template.RateOfFire);
            float weaponSkill = GetWeaponSkill(soldier.Soldier, weapon.Template);
            float total = weaponSkill + rofMod + rangeMod + modifiers;
            return new Tuple<float, float>(total, expectedDamage);

        }

        private Tuple<int, int> CalculateMovementAlongLine(Tuple<int, int> line, float moveSpeed)
        {
            if (moveSpeed <= 0) return new Tuple<int, int>(0, 0);   // this shouldn't happen
            // multiply line by the square root of moveSpeed^2/line^2
            int lineLengthSq = (line.Item1 * line.Item1) + (line.Item2 * line.Item2);
            float speedSq = moveSpeed * moveSpeed;
            float multiplier = Mathf.Sqrt(speedSq / lineLengthSq);

            // if they sent us something that moves faster than the line, just return the line
            if (multiplier >= 1.0f) return line;

            float xDistance = line.Item1 * multiplier;
            float yDistance = line.Item2 * multiplier;

            // should always move a minimum of one space
            if (xDistance == 0 && yDistance == 0)
            {
                if (line.Item1 > line.Item2)
                {
                    return new Tuple<int, int>(1, 0);
                }
                else
                {
                    return new Tuple<int, int>(0, 1);
                }
            }
            else
            {
                // if there's movement in both dimensions and "Wasted" movement in the longer direction
                // determine if the excess is enough to finish the movement along the smaller leg
                float xLeftover = xDistance % 1.0f;
                float yLeftover = yDistance % 1.0f;

                if (line.Item2 > 0 && xDistance > yDistance && xLeftover > 0)
                {
                    int x = (int)xDistance;
                    int y = (int)yDistance + 1;
                    if((x * x) + (y * y) < speedSq)
                    {
                        return new Tuple<int, int>(x, y);
                    }
                }
                else if (line.Item2 > 0 && yLeftover > 0)
                {
                    int x = (int)xDistance + 1;
                    int y = (int)yDistance;
                    if ((x * x) + (y * y) < speedSq)
                    {
                        return new Tuple<int, int>(x, y);
                    }
                }
            }
            return new Tuple<int, int> ((int)xDistance, (int)yDistance);
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
