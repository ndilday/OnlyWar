using System;
using System.Collections.Generic;
using System.Linq;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class ShootAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly RangedWeapon _weapon;
        private readonly BattleSoldier _target;

        public ShootAction(BattleSoldier shooter, RangedWeapon weapon, BattleSoldier target)
        {
            _soldier = shooter;
            _weapon = weapon;
            _target = target;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
