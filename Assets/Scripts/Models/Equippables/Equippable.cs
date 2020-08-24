using Iam.Scripts.Models.Soldiers;
using System; 

namespace Iam.Scripts.Models.Equippables
{
    public enum EquipLocation
    {
        Body,
        OneHand,
        TwoHand
    }

    [Flags]
    public enum FireModes
    {
        SingleShot = 0x1,

    }
    public class EquippableTemplate
    {
        public int Id;
        public string Name;
        public EquipLocation Location;
    }

    public class ArmorTemplate : EquippableTemplate
    {
        public int ArmorProvided;
    }

    public class WeaponTemplate : EquippableTemplate
    {
        public BaseSkill RelatedSkill;
        public float Accuracy;
        public int ArmorPiercing;
        public float PenetrationMultiplier;
    }

    public class RangedWeaponTemplate: WeaponTemplate
    {
        public float BaseStrength;
        public float MaximumDistance;
        public int RateOfFire;
        public ushort AmmoCapacity;
        public ushort Recoil;
    }

    public class MeleeWeaponTemplate: WeaponTemplate
    {
        public float StrengthMultiplier;
        public float ExtraDamage;
        public float ParryModifier;
        //public float Reach;
        public float ExtraAttacks;
    }

    public class Equippable
    {
        public EquippableTemplate Template;
    }

    public class Armor
    {
        public ArmorTemplate Template;
    }

    public class Weapon
    {
        public RangedWeaponTemplate Template;
    }
}