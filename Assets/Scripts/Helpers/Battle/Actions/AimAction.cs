using Iam.Scripts.Models.Equippables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class AimAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly BattleSoldier _target;
        private readonly RangedWeapon _weapon;

        public AimAction(BattleSoldier soldier, BattleSoldier target, RangedWeapon weapon)
        {
            _soldier = soldier;
            _target = target;
            _weapon = weapon;
        }
        public void Execute()
        {
            if(_soldier.Aim == null || _soldier.Aim.Item1 != _target)
            {
                // this is a new aim
                _soldier.Aim = new Tuple<BattleSoldier, RangedWeapon, int>(_target, _weapon, 0);
            }
            else
            {
                // increment the existing aim
                int curAim = _soldier.Aim.Item3;
                _soldier.Aim = new Tuple<BattleSoldier, RangedWeapon, int>(_target, _weapon, curAim + 1);
            }
        }
    }
}
