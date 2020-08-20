using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Iam.Scripts.Models
{
    [Flags]
    public enum Wounds : byte
    {
        None = 0,
        [Description("Negligible")]
        Negligible = 0x1,
        [Description("Minor")]
        Minor = 0x2,
        [Description("Moderate")]
        Moderate = 0x4,
        [Description("Major")]
        Major = 0x8,
        [Description("Serious")]
        Serious = 0x10,
        [Description("Severe")]
        Severe = 0x20,
        [Description("Critical")]
        Critical = 0x40,
        [Description("Unsurvivable")]
        Unsurvivable = 0x80
    }

    public static class WoundsExtensions
    {
        public static string ToFriendlyString(this Wounds me)
        {
            switch (me)
            {
                case Wounds.None:
                    return "No";
                case Wounds.Negligible:
                    return "Negligible";
                case Wounds.Minor:
                    return "Minor";
                case Wounds.Moderate:
                    return "Moderate";
                case Wounds.Major:
                    return "Major";
                case Wounds.Serious:
                    return "Serious";
                case Wounds.Severe:
                    return "Severe";
                case Wounds.Critical:
                    return "Critical";
                case Wounds.Unsurvivable:
                    return "Unsurvivable";
                default:
                    return "What happen?";
            }
        }
    }

    public class HitLocation
    {
        public int Id;
        public string Name;
        public int NaturalArmor;
        public float DamageMultiplier;
        public Wounds WoundLimit;
        public Wounds Wounds;
        public bool IsCybernetic;
        public int HitProbability;
    }

    public class Body
    {
        public HitLocation Brain { get; private set; }
        public HitLocation Eyes { get; private set; }
        public HitLocation Head { get; private set; }
        public HitLocation LeftArm { get; private set; }
        public HitLocation RightArm { get; private set; }
        public HitLocation LeftHand { get; private set; }
        public HitLocation RightHand { get; private set; }
        public HitLocation Torso { get; private set; }
        public HitLocation Groin { get; private set; }
        public HitLocation LeftLeg { get; private set; }
        public HitLocation RightLeg { get; private set; }
        public HitLocation LeftFoot { get; private set; }
        public HitLocation RightFoot { get; private set; }
        public int TotalProbability { get; private set; }


        public Body()
        {
            Brain = new HitLocation
            {
                Id = 0,
                Name = "Brain",
                NaturalArmor = 2,
                DamageMultiplier = 4,
                IsCybernetic = false,
                HitProbability = 32,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Unsurvivable
            };

            Eyes = new HitLocation
            {
                Id = 1,
                Name = "Eyes",
                NaturalArmor = 0,
                DamageMultiplier = 4,
                IsCybernetic = false,
                HitProbability = 6,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Major
            };

            Head = new HitLocation
            {
                Id = 2,
                Name = "Head",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 52,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Unsurvivable
            };

            Torso = new HitLocation
            {
                Id = 3,
                Name = "Torso",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 500,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Unsurvivable
            };

            LeftArm = new HitLocation
            {
                Id = 4,
                Name = "Left Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 190,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Serious
            };

            RightArm = new HitLocation
            {
                Id = 5,
                Name = "Right Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 190,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Serious
            };

            LeftHand = new HitLocation
            {
                Id = 6,
                Name = "Left Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 45,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Major
            };

            RightHand = new HitLocation
            {
                Id = 7,
                Name = "Right Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 45,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Major
            };

            Groin = new HitLocation
            {
                Id = 8,
                Name = "Vitals",
                NaturalArmor = 2,
                DamageMultiplier = 1.5f,
                IsCybernetic = false,
                HitProbability = 136,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Unsurvivable
            };

            LeftLeg = new HitLocation
            {
                Id = 9,
                Name = "Left Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 363,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Severe
            };

            RightLeg = new HitLocation
            {
                Id = 10,
                Name = "Right Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 363,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Severe
            };

            LeftFoot = new HitLocation
            {
                Id = 11,
                Name = "Left Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 112,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Major
            };

            RightFoot = new HitLocation
            {
                Id = 12,
                Name = "Right Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                IsCybernetic = false,
                HitProbability = 112,
                Wounds = Wounds.None,
                WoundLimit = Wounds.Major
            };

            TotalProbability = 2146;
        }

        public IEnumerable<HitLocation> HitLocations
        {
            get
            {
                yield return Brain;
                yield return Eyes;
                yield return Head;
                yield return LeftArm;
                yield return RightArm;
                yield return LeftHand;
                yield return RightHand;
                yield return Torso;
                yield return Groin;
                yield return LeftLeg;
                yield return RightLeg;
                yield return LeftFoot;
                yield return RightFoot;
            }
        }
    }
}
