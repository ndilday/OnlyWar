using System; 

namespace Iam.Scripts.Models
{
    public enum EquipLocation
    {
        Body,
        OneHand,
        TwoHand
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

    public class RangedWeaponTemplate: EquippableTemplate
    {
        public float Accuracy;
        public int ArmorPiercing;
        public float BaseStrength;
        public float MaximumDistance;
        public int RateOfFire;
        public float PenetrationMultiplier;
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