using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Helpers.Battle.Resolutions;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class ShootAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly RangedWeapon _weapon;
        private readonly BattleSoldier _target;
        private readonly ConcurrentBag<WoundResolution> _resultList;

        public ShootAction(BattleSoldier shooter, RangedWeapon weapon, BattleSoldier target, ConcurrentBag<WoundResolution> resultList)
        {
            _soldier = shooter;
            _weapon = weapon;
            _target = target;
            _resultList = resultList;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
