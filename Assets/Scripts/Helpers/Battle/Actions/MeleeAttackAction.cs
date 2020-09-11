﻿using System;
using System.Collections.Concurrent;

using Iam.Scripts.Helpers.Battle.Resolutions;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    public class MeleeAttackAction : IAction
    {
        private readonly BattleSoldier _attacker;
        private readonly BattleSoldier _target;
        private readonly MeleeWeapon _weapon;
        private readonly ConcurrentBag<WoundResolution> _resultList;
        private readonly ConcurrentQueue<string> _log;
        private readonly bool _didMove;
        public MeleeAttackAction(BattleSoldier attacker, BattleSoldier target, MeleeWeapon weapon, bool didMove, ConcurrentBag<WoundResolution> resultList, ConcurrentQueue<string> log)
        {
            _attacker = attacker;
            _target = target;
            _weapon = weapon;
            if(_weapon == null)
            {
                _weapon = new MeleeWeapon(ImperialEquippables.Instance.Fist);
            }
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
                    _attacker.Squad.IsInMelee = true;
                    _target.Squad.IsInMelee = true;
                    float modifier = _weapon.Template.Accuracy + (_didMove ? -2 : 0);
                    float skill = BattleHelpers.GetWeaponSkillPlusStat(_attacker.Soldier, _weapon.Template);
                    float roll = 10.5f + (3.0f * (float)Random.NextGaussianDouble());
                    float total = skill + modifier - roll;
                    _log.Enqueue(_attacker.Soldier.ToString() + " swings at " + _target.Soldier.ToString());
                    if (total > 0)
                    {
                        _log.Enqueue(_attacker.Soldier.ToString() + " strikes " + _target.Soldier.ToString());
                        HandleHit();
                    }
                }
            }
            else
            {
                _log.Enqueue("<color=orange>" + _attacker.Soldier.ToString() + " did not get close enough to attack</color>");
            }
        }

        private bool IsAdjacentToTarget()
        {
            int quickDistance = _attacker.Location.Item1 - _target.Location.Item1 + _attacker.Location.Item2 - _target.Location.Item2;
            return quickDistance == 1 || quickDistance == -1;
        }

        private void HandleHit()
        {
            HitLocation hitLocation = DetermineHitLocation(_target);
            // make sure this body part hasn't already been shot off
            if (!hitLocation.IsSevered)
            {
                float damage = _attacker.Soldier.Strength * _weapon.Template.StrengthMultiplier * (3.5f + ((float)Random.NextGaussianDouble() * 1.75f));
                float effectiveArmor = _target.Armor.Template.ArmorProvided * _weapon.Template.ArmorMultiplier;
                float penDamage = damage - effectiveArmor;
                if (penDamage > 0)
                {
                    float totalDamage = penDamage * _weapon.Template.PenetrationMultiplier;
                    _resultList.Add(new WoundResolution(_target, totalDamage, hitLocation));
                }
            }
        }

        private HitLocation DetermineHitLocation(BattleSoldier soldier)
        {
            // we're using the "lottery ball" approach to randomness here, where each point of probability
            // for each available body party defines the size of the random linear distribution
            // TODO: factor in cover/body position
            // 
            int roll = Random.GetIntBelowMax(0, soldier.Soldier.Body.TotalProbabilityMap[soldier.Stance]);
            foreach (HitLocation location in soldier.Soldier.Body.HitLocations)
            {
                int locationChance = location.Template.HitProbabilityMap[soldier.Stance];
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
