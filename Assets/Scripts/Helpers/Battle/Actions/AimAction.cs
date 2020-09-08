using Iam.Scripts.Models.Equippables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class AimAction : IAction
    {
        BattleSoldier _soldier;
        BattleSoldier _target;
        RangedWeapon _weapon;

        public AimAction(BattleSoldier soldier, BattleSoldier target, RangedWeapon weapon)
        {
            _soldier = soldier;
            _target = target;
            _weapon = weapon;
        }
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
