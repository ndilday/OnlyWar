using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

    public class HitLocationTemplate
    {
        public int Id;
        public string Name;
        public int NaturalArmor;
        public float DamageMultiplier;
        public Wounds WoundLimit;
        public int HitProbability;
    }

    public class HitLocation
    {
        public Wounds Wounds;
        public bool IsCybernetic;
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
            list.Add( new HitLocationTemplate
            {
                Id = 0,
                Name = "Brain",
                NaturalArmor = 2,
                DamageMultiplier = 4,
                HitProbability = 32,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 1,
                Name = "Eyes",
                NaturalArmor = 0,
                DamageMultiplier = 4,
                HitProbability = 6,
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 2,
                Name = "Head",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 52,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 3,
                Name = "Torso",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 500,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 4,
                Name = "Left Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 190,
                WoundLimit = Wounds.Serious
            });

            list.Add( new HitLocationTemplate
            {
                Id = 5,
                Name = "Right Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 190,
                WoundLimit = Wounds.Serious
            });

            list.Add( new HitLocationTemplate
            {
                Id = 6,
                Name = "Left Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 45,
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 7,
                Name = "Right Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 45,
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 8,
                Name = "Vitals",
                NaturalArmor = 2,
                DamageMultiplier = 1.5f,
                HitProbability = 136,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add( new HitLocationTemplate
            {
                Id = 9,
                Name = "Left Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 363,
                WoundLimit = Wounds.Severe
            });

            list.Add( new HitLocationTemplate
            {
                Id = 10,
                Name = "Right Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 363,
                WoundLimit = Wounds.Severe
            });

            list.Add( new HitLocationTemplate
            {
                Id = 11,
                Name = "Left Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 112,
                WoundLimit = Wounds.Major
            });

            list.Add( new HitLocationTemplate
            {
                Id = 12,
                Name = "Right Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 112,
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
                HitProbability = 32,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 1,
                Name = "Eyes",
                NaturalArmor = 0,
                DamageMultiplier = 4,
                HitProbability = 6,
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 2,
                Name = "Head",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 52,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 3,
                Name = "Torso",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 500,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 4,
                Name = "Left Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 95,
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 4,
                Name = "Left Talon",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 95,
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 5,
                Name = "Right Arm",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 190,
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 5,
                Name = "Right Talon",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 95,
                WoundLimit = Wounds.Serious
            });

            list.Add(new HitLocationTemplate
            {
                Id = 6,
                Name = "Left Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 45,
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 7,
                Name = "Right Hand",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 45,
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 8,
                Name = "Vitals",
                NaturalArmor = 2,
                DamageMultiplier = 1.5f,
                HitProbability = 136,
                WoundLimit = Wounds.Unsurvivable
            });

            list.Add(new HitLocationTemplate
            {
                Id = 9,
                Name = "Left Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 363,
                WoundLimit = Wounds.Severe
            });

            list.Add(new HitLocationTemplate
            {
                Id = 10,
                Name = "Right Leg",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 363,
                WoundLimit = Wounds.Severe
            });

            list.Add(new HitLocationTemplate
            {
                Id = 11,
                Name = "Left Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 112,
                WoundLimit = Wounds.Major
            });

            list.Add(new HitLocationTemplate
            {
                Id = 12,
                Name = "Right Foot",
                NaturalArmor = 0,
                DamageMultiplier = 1,
                HitProbability = 112,
                WoundLimit = Wounds.Major
            });

            Initialize(list);
        }
    }

    public class Body
    {
        public HitLocation[] HitLocations { get; private set; }
        public int TotalProbability { get; private set; }

        public Body(BodyTemplate template)
        {
            HitLocations = template.HitLocations.Select(hlt => new HitLocation(hlt)).ToArray();
            TotalProbability = HitLocations.Sum(hl => hl.Template.HitProbability);
        }
    }
}
