using Iam.Scripts.Models.Equippables;
using System;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class ReadyRangedWeaponAction : IAction
    {
        BattleSoldier _soldier;
        RangedWeapon _weapon;
        public ReadyRangedWeaponAction(BattleSoldier soldier, RangedWeapon weapon)
        {
            _soldier = soldier;
            _weapon = weapon;
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
