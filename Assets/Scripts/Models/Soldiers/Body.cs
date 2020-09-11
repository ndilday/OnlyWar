using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Iam.Scripts.Models.Soldiers
{
    public enum Stance
    {
        Standing,
        Kneeling, 
        Prone
    }

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

    public class HitLocationTemplate
    {
        public int Id;
        public string Name;
        public int NaturalArmor;
        public float DamageMultiplier;
        public Wounds WoundLimit;
        public Dictionary<Stance, int> HitProbabilityMap;
    }

    public class HitLocation
    {
        public Wounds Wounds;
        public bool IsCybernetic;
        public bool IsSevered;
        //public bool IsCrippled;
        public HitLocationTemplate Template { get; private set; }
        public HitLocation(HitLocationTemplate template)
        {
            Wounds = Wounds.None;
            IsCybernetic = false;
            Template = template;
        }

        public override string ToString()
        {
            if(Wounds >= Template.WoundLimit)
            {
                return Template.Name + ": <color=red>Crippled</color>";
            }
            if(Wounds >= Wounds.Unsurvivable)
            {
                return Template.Name + ": <color=red>Unsurvivable</color>";
            }
            if (Wounds >= Wounds.Critical)
            {
                return Template.Name + ":<color=maroon> Critical</color>";
            }
            if (Wounds >= Wounds.Severe)
            {
                return Template.Name + ": <color=maroon>Severe</color>";
            }
            if (Wounds >= Wounds.Serious)
            {
                return Template.Name + ": <color=orange>Serious</color>";
            }
            if (Wounds >= Wounds.Major)
            {
                return Template.Name + ": <color=orange>Major</color>";
            }
            if (Wounds >= Wounds.Moderate)
            {
                return Template.Name + ": <color=olive>Moderate</color>";
            }
            if (Wounds >= Wounds.Minor)
            {
                return Template.Name + ": <color=teal>Minor</color>";
            }
            if (Wounds >= Wounds.Negligible)
            {
                return Template.Name + ": <color=teal>Negligible</color>";
            }
            return Template.Name + ": No wounds";
        }
    }

    public class BodyTemplate
    {
        public HitLocationTemplate[] HitLocations;

        protected void Initialize(IEnumerable<HitLocationTemplate> hitLocations)
        {
            HitLocations = hitLocations.ToArray();
        }
    }

    public class HumanBodyTemplate : BodyTemplate
    {
        private static HumanBodyTemplate _instance;

        public static HumanBodyTemplate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HumanBodyTemplate();
                }
                return _instance;
            }
        }

        private HumanBodyTemplate()
        {
            List<HitLocationTemplate> list = new List<HitLocationTemplate>();
            list.Add(new HitLocationTemplate
            {
                Id = 0,
                Name = "Brain",
                NaturalArmor = 2,
                DamageMultiplier = 4,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 30 },
                    {Stance.Kneeling, 30 },
                    { Stance.Prone, 30 }
                },
                WoundLimit = Wounds.Unsurvivable,

            });

            list.Add( new HitLocationTemplate
            {
                Id = 1,
                Name = "Eyes",
                NaturalArmor = 0,
                DamageMultiplier = 4,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 1 },
                    {Stance.Kneeling, 1 },
                    { Stance.Prone, 1 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 2,
                Name = "Face",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 75 },
                    {Stance.Kneeling, 75 },
                    { Stance.Prone, 75 }
                },
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 3,
                Name = "Torso",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 480 },
                    {Stance.Kneeling, 480 },
                    { Stance.Prone, 30 }
                },
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 4,
                Name = "Left Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 96 },
                    {Stance.Kneeling, 96 },
                    { Stance.Prone, 15 }
                },
                WoundLimit = Wounds.Serious
            });

            list.Add( new HitLocationTemplate
            {
                Id = 5,
                Name = "Right Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 96 },
                    {Stance.Kneeling, 96 },
                    { Stance.Prone, 15 }
                },
                WoundLimit = Wounds.Serious
            });

            list.Add( new HitLocationTemplate
            {
                Id = 6,
                Name = "Left Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 20 },
                    {Stance.Kneeling, 20 },
                    { Stance.Prone, 20 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 7,
                Name = "Right Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 20 },
                    {Stance.Kneeling, 20 },
                    { Stance.Prone, 20 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 8,
                Name = "Vitals",
                NaturalArmor = 2,
                DamageMultiplier = 1.5f,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 100 },
                    {Stance.Kneeling, 100 },
                    { Stance.Prone, 10 }
                },
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 9,
                Name = "Left Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 160 },
                    {Stance.Kneeling, 80 },
                    { Stance.Prone, 1 }
                },
                WoundLimit = Wounds.Severe
            });

            list.Add( new HitLocationTemplate
            {
                Id = 10,
                Name = "Right Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 160 },
                    {Stance.Kneeling, 80 },
                    { Stance.Prone, 1 }
                },
                WoundLimit = Wounds.Severe
            });

            list.Add( new HitLocationTemplate
            {
                Id = 11,
                Name = "Left Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 15 },
                    {Stance.Kneeling, 7 },
                    { Stance.Prone, 0 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 12,
                Name = "Right Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 15 },
                    {Stance.Kneeling, 7 },
                    { Stance.Prone, 0 }
                },
                WoundLimit = Wounds.Major
            });

            Initialize(list);
        }
    }

    public class TyranidWarriorBodyTemplate : BodyTemplate
    {
        private static TyranidWarriorBodyTemplate _instance;

        public static TyranidWarriorBodyTemplate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TyranidWarriorBodyTemplate();
                }
                return _instance;
            }
        }
        private TyranidWarriorBodyTemplate()
        {
            List<HitLocationTemplate> list = new List<HitLocationTemplate>();
            list.Add(new HitLocationTemplate
            {
                Id = 0,
                Name = "Brain",
                NaturalArmor = 2,
                DamageMultiplier = 4,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 30 },
                    {Stance.Kneeling, 30 },
                    { Stance.Prone, 30 }
                },
                WoundLimit = Wounds.Unsurvivable,

            });

            list.Add(new HitLocationTemplate
            {
                Id = 1,
                Name = "Eyes",
                NaturalArmor = 0,
                DamageMultiplier = 4,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 1 },
                    {Stance.Kneeling, 1 },
                    { Stance.Prone, 1 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 2,
                Name = "Face",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 75 },
                    {Stance.Kneeling, 75 },
                    { Stance.Prone, 75 }
                },
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 3,
                Name = "Torso",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 480 },
                    {Stance.Kneeling, 480 },
                    { Stance.Prone, 30 }
                },
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 4,
                Name = "Left Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 96 },
                    {Stance.Kneeling, 96 },
                    { Stance.Prone, 15 }
                },
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 5,
                Name = "Left Talon",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 72 },
                    {Stance.Kneeling, 72 },
                    { Stance.Prone, 15 }
                },
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 6,
                Name = "Right Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 96 },
                    {Stance.Kneeling, 96 },
                    { Stance.Prone, 15 }
                },
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 7,
                Name = "Right Talon",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 72 },
                    {Stance.Kneeling, 72 },
                    { Stance.Prone, 15 }
                },
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 8,
                Name = "Left Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 20 },
                    {Stance.Kneeling, 20 },
                    { Stance.Prone, 20 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 9,
                Name = "Right Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 20 },
                    {Stance.Kneeling, 20 },
                    { Stance.Prone, 20 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 10,
                Name = "Vitals",
                NaturalArmor = 2,
                DamageMultiplier = 1.5f,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 100 },
                    {Stance.Kneeling, 100 },
                    { Stance.Prone, 10 }
                },
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 11,
                Name = "Left Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 160 },
                    {Stance.Kneeling, 80 },
                    { Stance.Prone, 1 }
                },
                WoundLimit = Wounds.Severe
            });

            list.Add(new HitLocationTemplate
            {
                Id = 12,
                Name = "Right Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 160 },
                    {Stance.Kneeling, 80 },
                    { Stance.Prone, 1 }
                },
                WoundLimit = Wounds.Severe
            });

            list.Add(new HitLocationTemplate
            {
                Id = 13,
                Name = "Left Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 15 },
                    {Stance.Kneeling, 7 },
                    { Stance.Prone, 0 }
                },
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 14,
                Name = "Right Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbabilityMap = new Dictionary<Stance, int>()
                {
                    { Stance.Standing, 15 },
                    {Stance.Kneeling, 7 },
                    { Stance.Prone, 0 }
                },
                WoundLimit = Wounds.Major
            });

            Initialize(list);
        }
    }

    public class Body
    {
        public HitLocation[] HitLocations { get; private set; }
        public Dictionary<Stance, int> TotalProbabilityMap { get; private set; }

        public Body(BodyTemplate template)
        {
            HitLocations = template.HitLocations.Select(hlt => new HitLocation(hlt)).ToArray();
            TotalProbabilityMap = new Dictionary<Stance, int>();
            TotalProbabilityMap[Stance.Standing] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[Stance.Standing]);
            TotalProbabilityMap[Stance.Kneeling] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[Stance.Kneeling]);
            TotalProbabilityMap[Stance.Prone] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[Stance.Prone]);
        }
    }
}
