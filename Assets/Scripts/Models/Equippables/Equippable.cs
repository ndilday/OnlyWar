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
        public float ArmorMultiplier;
        public float PenetrationMultiplier;
        public float RequiredStrength;
    }

    public class RangedWeaponTemplate: WeaponTemplate
    {
        public float BaseStrength;
        public float MaximumDistance;
        public int RateOfFire;
        public ushort AmmoCapacity;
        public ushort Recoil;
        public ushort Bulk;
        public bool DoesDamageDegradeWithRange;
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
        public EquippableTemplate Template { get; private set; }
        public Equippable(EquippableTemplate template) { Template = template; }
    }

    public class Armor
    {
        public ArmorTemplate Template { get; private set; }
        public Armor(ArmorTemplate template) { Template = template; }
    }

    public class RangedWeapon
    {
        public RangedWeaponTemplate Template { get; private set; }
        public RangedWeapon(RangedWeaponTemplate template) { Template = template; }

        public override string ToString()
        {
            return Template.Name;
        }
    }

    public class MeleeWeapon
    {
        public MeleeWeaponTemplate Template { get; private set; }
        public MeleeWeapon(MeleeWeaponTemplate template) { Template = template; }
        public override string ToString()
        {
            return Template.Name;
        }
    }
}