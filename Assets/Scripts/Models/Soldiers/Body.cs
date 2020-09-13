using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Soldiers
{
    public enum Stance
    {
        Standing,
        Kneeling, 
        Prone
    }

    public class Wounds
    {
        public uint WoundLevel { get; private set; }
        public const byte WOUND_MAX = 5;
        
        public byte NegligibleWounds
        {
            get
            {
                return (byte)(WoundLevel % 0xf);
            }
        }

        public byte MinorWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x10) % 0xf);
            }
        }

        public byte ModerateWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x100) % 0xf);
            }
        }

        public byte MajorWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x1000) % 0xf);
            }
        }

        public byte SeriousWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x10000) % 0xf);
            }
        }

        public byte SevereWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x100000) % 0xf);
            }
        }

        public byte CriticalWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x1000000) % 0xf);
            }
        }

        public byte UnsurvivableWounds
        {
            get
            {
                return (byte)((WoundLevel / 0x10000000) % 0xf);
            }
        }

        public void AddWound(WoundType wound)
        {
            WoundLevel += (uint)wound;
            if(NegligibleWounds > WOUND_MAX)
            {
                WoundLevel &= 0xfffffff0;
                WoundLevel += (uint)WoundType.Minor;
            }
            if (MinorWounds > WOUND_MAX)
            {
                WoundLevel &= 0xffffff0f;
                WoundLevel += (uint)WoundType.Moderate;
            }
            if (ModerateWounds > WOUND_MAX)
            {
                WoundLevel &= 0xfffff0ff;
                WoundLevel += (uint)WoundType.Major;
            }
            if (MajorWounds > WOUND_MAX)
            {
                WoundLevel &= 0xffff0fff;
                WoundLevel += (uint)WoundType.Serious;
            }
            if (SeriousWounds > WOUND_MAX)
            {
                WoundLevel &= 0xfff0ffff;
                WoundLevel += (uint)WoundType.Severe;
            }
            if (SevereWounds > WOUND_MAX)
            {
                WoundLevel &= 0xff0fffff;
                WoundLevel += (uint)WoundType.Critical;
            }
            if (CriticalWounds > WOUND_MAX)
            {
                WoundLevel &= 0xf0ffffff;
                WoundLevel += (uint)WoundType.Unsurvivable;
            }
        }
    }

    public enum WoundType
    {
        None = 0,
        Negligible = 0x1,
        Minor = 0x10,
        Moderate = 0x100,
        Major = 0x1000,
        Serious = 0x10000,
        Severe = 0x100000,
        Critical = 0x1000000,
        Unsurvivable = 0x10000000
    }

    public class HitLocationTemplate
    {
        public int Id;
        public string Name;
        public int NaturalArmor;
        public float DamageMultiplier;
        public WoundType WoundLimit;
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
            Wounds = new Wounds();
            IsCybernetic = false;
            Template = template;
        }

        public override string ToString()
        {
            if(Wounds.WoundLevel >= (uint)Template.WoundLimit)
            {
                return Template.Name + ": <color=red>Crippled</color>";
            }
            if(Wounds.WoundLevel >= (uint)WoundType.Unsurvivable)
            {
                return Template.Name + ": <color=red>Unsurvivable</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Critical)
            {
                return Template.Name + ":<color=maroon> Critical</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Severe)
            {
                return Template.Name + ": <color=maroon>Severe</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Serious)
            {
                return Template.Name + ": <color=orange>Serious</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Major)
            {
                return Template.Name + ": <color=orange>Major</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Moderate)
            {
                return Template.Name + ": <color=olive>Moderate</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Minor)
            {
                return Template.Name + ": <color=teal>Minor</color>";
            }
            else if (Wounds.WoundLevel >= (uint)WoundType.Negligible)
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
            List<HitLocationTemplate> list = new List<HitLocationTemplate>
            {
                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable,

                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Serious
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Serious
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Severe
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Severe
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                }
            };

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
            List<HitLocationTemplate> list = new List<HitLocationTemplate>
            {
                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable,

                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Serious
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Serious
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Serious
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Serious
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Unsurvivable
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Severe
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Severe
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                },

                new HitLocationTemplate
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
                    WoundLimit = WoundType.Major
                }
            };

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
            TotalProbabilityMap = new Dictionary<Stance, int>
            {
                [Stance.Standing] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[Stance.Standing]),
                [Stance.Kneeling] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[Stance.Kneeling]),
                [Stance.Prone] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[Stance.Prone])
            };
        }
    }
}
