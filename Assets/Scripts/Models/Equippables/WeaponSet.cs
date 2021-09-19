using System.Collections.Generic;

namespace OnlyWar.Models.Equippables
{
    public class WeaponSet
    {
        public int Id { get; }
        public string Name { get; }
        public RangedWeaponTemplate PrimaryRangedWeapon { get; }
        public RangedWeaponTemplate SecondaryRangedWeapon { get; }
        public MeleeWeaponTemplate PrimaryMeleeWeapon { get; }
        public MeleeWeaponTemplate SecondaryMeleeWeapon { get; }
        public IReadOnlyCollection<RangedWeapon> GetRangedWeapons()
        {
            if (PrimaryRangedWeapon == null) return null;
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
        public IReadOnlyCollection<MeleeWeapon> GetMeleeWeapons()
        {
            if (PrimaryMeleeWeapon == null) return null;
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
        public WeaponSet(int id, string name, 
                         RangedWeaponTemplate primaryRanged = null, RangedWeaponTemplate secondaryRanged = null,
                         MeleeWeaponTemplate primaryMelee = null, MeleeWeaponTemplate secondaryMelee = null)
        {
            Id = id;
            Name = name;
            PrimaryRangedWeapon = primaryRanged;
            SecondaryRangedWeapon = secondaryRanged;
            PrimaryMeleeWeapon = primaryMelee;
            SecondaryMeleeWeapon = secondaryMelee;
        }
    }
}
