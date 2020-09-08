using System;
using System.Collections.Generic;

namespace Iam.Scripts.Models.Equippables
{
    public class WeaponSet
    {
        public string Name { get; set; }
        public RangedWeaponTemplate PrimaryRangedWeapon { get; set; }
        public RangedWeaponTemplate SecondaryRangedWeapon { get; set; }
        public MeleeWeaponTemplate PrimaryMeleeWeapon { get; set; }
        public MeleeWeaponTemplate SecondaryMeleeWeapon { get; set; }
        public List<RangedWeapon> GetRangedWeapons()
        {
            List<RangedWeapon> list = new List<RangedWeapon>
            {
                new RangedWeapon(PrimaryRangedWeapon)
            };
            if (SecondaryRangedWeapon != null)
            {
                list.Add(new RangedWeapon(SecondaryRangedWeapon));
            }
            return list;
        }
        public List<MeleeWeapon> GetMeleeWeapons()
        {
            List<MeleeWeapon> list = new List<MeleeWeapon>
            {
                new MeleeWeapon(PrimaryMeleeWeapon)
            };
            if (SecondaryMeleeWeapon != null)
            {
                list.Add(new MeleeWeapon(SecondaryMeleeWeapon));
            }
            return list;
        }
    }
}
