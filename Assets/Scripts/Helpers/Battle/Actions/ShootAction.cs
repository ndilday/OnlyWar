using System;
using System.Collections.Generic;
using System.Linq;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class ShootAction : IAction
    {
        private readonly Soldier _soldier;
        private readonly RangedWeapon _weapon;
        private readonly BattleSquad _targetSquad;

        public ShootAction(Soldier shooter, RangedWeapon weapon, BattleSquad targetSquad)
        {
            _soldier = shooter;
            _weapon = weapon;
            _targetSquad = targetSquad;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
