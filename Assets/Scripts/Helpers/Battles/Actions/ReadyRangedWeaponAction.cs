using OnlyWar.Models.Equippables;
using System;

namespace OnlyWar.Helpers.Battles.Actions
{
    class ReadyRangedWeaponAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly RangedWeapon _weapon;
        public ReadyRangedWeaponAction(BattleSoldier soldier, RangedWeapon weapon)
        {
            _soldier = soldier;
            _weapon = weapon;
        }

        public void Execute()
        {
            int handsFree = _soldier.HandsFree;
            if(_weapon.Template.Location == EquipLocation.TwoHand && handsFree < 2)
            {
                // unequip any equipped weapons
                _soldier.EquippedRangedWeapons.Clear();
                _soldier.EquippedMeleeWeapons.Clear();
            }
            if(_weapon.Template.Location == EquipLocation.OneHand && handsFree < 1)
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
            _soldier.EquippedRangedWeapons.Add(_weapon);
        }
    }
}
