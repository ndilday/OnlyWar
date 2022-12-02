using System;
using System.Collections.Concurrent;

using OnlyWar.Helpers.Battles.Resolutions;
using OnlyWar.Models.Equippables;
using OnlyWar.Models.Soldiers;

namespace OnlyWar.Helpers.Battles.Actions
{
    public class MeleeAttackAction : IAction
    {
        private readonly BattleSoldier _attacker;
        private readonly BattleSoldier _target;
        private readonly MeleeWeapon _weapon;
        private readonly ConcurrentBag<WoundResolution> _resultList;
        private readonly ConcurrentQueue<string> _log;
        private readonly bool _didMove;
        public MeleeAttackAction(BattleSoldier attacker, BattleSoldier target, 
                                 MeleeWeapon weapon, bool didMove, 
                                 ConcurrentBag<WoundResolution> resultList, 
                                 ConcurrentQueue<string> log)
        {
            _attacker = attacker;
            _target = target;
            _weapon = weapon;
            _didMove = didMove;
            _resultList = resultList;
            _log = log;
        }
        public void Execute()
        {
            if (IsAdjacentToTarget())
            {
                for (int i = 0; i <= _weapon.Template.ExtraAttacks; i++)
                {
                    _attacker.IsInMelee = true;
                    _target.IsInMelee = true;
                    _attacker.BattleSquad.IsInMelee = true;
                    _target.BattleSquad.IsInMelee = true;
                    float modifier = _weapon.Template.Accuracy + (_didMove ? -2 : 0);
                    float skill = _attacker.Soldier.GetTotalSkillValue(_weapon.Template.RelatedSkill);
                    float roll = 10.5f + (3.0f * (float)RNG.NextGaussianDouble());
                    float total = skill + modifier - roll;
                    _log.Enqueue(_attacker.Soldier.Name + " swings at " + _target.Soldier.ToString());
                    if (total > 0)
                    {
                        _log.Enqueue(_attacker.Soldier.Name + " strikes " + _target.Soldier.ToString());
                        HandleHit();
                    }
                }
                _attacker.TurnsSwinging++;
            }
            else
            {
                _log.Enqueue("<color=orange>" + _attacker.Soldier.Name + " did not get close enough to attack</color>");
            }
        }

        private bool IsAdjacentToTarget()
        {
            int topLimit, bottomLimit, leftLimit, rightLimit;
            topLimit = _attacker.TopLeft.Item2 + 1;
            leftLimit = _attacker.TopLeft.Item1 - 1;
            bottomLimit = _attacker.BottomRight.Item2 - 1;
            rightLimit = _attacker.BottomRight.Item1 + 1;

            // the target is adjacent if it's not any of
            // above, below, left, or right of the limit grid
            bool targetIsAbove = _target.BottomRight.Item2 > topLimit;
            bool targetIsBelow = _target.TopLeft.Item2 < bottomLimit;
            bool targetIsLeft = _target.BottomRight.Item1 < leftLimit;
            bool targetIsRight = _target.TopLeft.Item1 > rightLimit;

            return !targetIsAbove && !targetIsBelow && !targetIsLeft && !targetIsRight;
        }

        private void HandleHit()
        {
            HitLocation hitLocation = DetermineHitLocation(_target);
            // make sure this body part hasn't already been shot off
            if (!hitLocation.IsSevered)
            {
                float damage = _attacker.Soldier.Strength * _weapon.Template.StrengthMultiplier * (3.5f + ((float)RNG.NextGaussianDouble() * 1.75f));
                float effectiveArmor = _target.Armor.Template.ArmorProvided * _weapon.Template.ArmorMultiplier;
                float penDamage = damage - effectiveArmor;
                if (penDamage > 0)
                {
                    float totalDamage = penDamage * _weapon.Template.WoundMultiplier;
                    _resultList.Add(new WoundResolution(_attacker, _weapon.Template, _target, totalDamage, hitLocation));
                }
            }
        }

        private HitLocation DetermineHitLocation(BattleSoldier soldier)
        {
            // we're using the "lottery ball" approach to randomness here, where each point of probability
            // for each available body party defines the size of the random linear distribution
            // TODO: factor in cover/body position
            // 
            int roll = RNG.GetIntBelowMax(0, soldier.Soldier.Body.TotalProbabilityMap[soldier.Stance]);
            foreach (HitLocation location in soldier.Soldier.Body.HitLocations)
            {
                int locationChance = location.Template.HitProbabilityMap[(int)soldier.Stance];
                if (roll < locationChance)
                {
                    return location;
                }
                else
                {
                    // this is basically an easy iterative way to figure out which body part on the "chart" the roll matches
                    roll -= locationChance;
                }
            }
            // this should never happen
            throw new InvalidOperationException("Could not determine a hit location");
        }
    }
}
