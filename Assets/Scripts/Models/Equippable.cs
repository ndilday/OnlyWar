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

        public Equippable GenerateInstance()
        {
            return new Equippable
            {
                Template = this
            };
        }
    }

    public class ArmorTemplate : EquippableTemplate
    {
        public int ArmorProvided;
    }

    public class WeaponTemplate: EquippableTemplate
    {
        public int ArmorPiercing;
        public RangeBands RangeBands;
        public int RateOfFire;
        public float PenetrationMultiplier;
    }

    public class RangeBand
    {
        public int Range;
        public int Accuracy;
        public int Strength;
    }

    public class RangeBands
    {
        public RangeBand Close;
        public RangeBand Medium;
        public RangeBand Long;
        public RangeBand GetRangeForDistance(float distance)
        {
            if (distance <= Close.Range) return Close;
            else if (distance <= Medium.Range) return Medium;
            else if (distance <= Long.Range) return Long;
            else return null;
        }
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
        public WeaponTemplate Template;
    }
}