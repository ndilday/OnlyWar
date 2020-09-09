using Iam.Scripts.Models.Equippables;
using System;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    class ReadyMeleeWeaponAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly MeleeWeapon _weapon;
        public ReadyMeleeWeaponAction(BattleSoldier soldier, MeleeWeapon weapon)
        {
            _soldier = soldier;
            _weapon = weapon;
        }

        public void Execute()
        {
            if(_weapon.Template.Location == EquipLocation.TwoHand && _soldier.HandsFree < 2)
            {
                // unequip any equipped weapons
                _soldier.EquippedRangedWeapons.Clear();
                _soldier.EquippedMeleeWeapons.Clear();
            }
            if(_weapon.Template.Location == EquipLocation.OneHand && _soldier.HandsFree < 1)
            {
                if(_soldier.EquippedRangedWeapons.Count > 0)
                {
                    _soldier.EquippedRangedWeapons.Clear();
                }
                else
                {
                    _soldier.EquippedMeleeWeapons.Clear();
                }
            }
            _soldier.EquippedMeleeWeapons.Add(_weapon);
        }
    }
}
