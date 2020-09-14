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
        public uint WoundTotal { get; private set; }
        public const byte WOUND_MAX = 5;
        public byte WeeksOfHealing { get; set; }
        
        public byte NegligibleWounds
        {
            get
            {
                return (byte)(WoundTotal % 0xf);
            }
        }

        public byte MinorWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x10) % 0xf);
            }
        }

        public byte ModerateWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x100) % 0xf);
            }
        }

        public byte MajorWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x1000) % 0xf);
            }
        }

        public byte CriticalWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x10000) % 0xf);
            }
        }

        public byte MassiveWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x100000) % 0xf);
            }
        }

        public byte MortalWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x1000000) % 0xf);
            }
        }

        public byte UnsurvivableWounds
        {
            get
            {
                return (byte)((WoundTotal / 0x10000000) % 0xf);
            }
        }

        public void AddWound(WoundLevel wound)
        {
            WeeksOfHealing = 0;
            WoundTotal += (uint)wound;
            if(NegligibleWounds > WOUND_MAX)
            {
                WoundTotal &= 0xfffffff0;
                WoundTotal += (uint)WoundLevel.Minor;
            }
            if (MinorWounds > WOUND_MAX)
            {
                WoundTotal &= 0xffffff0f;
                WoundTotal += (uint)WoundLevel.Moderate;
            }
            if (ModerateWounds > WOUND_MAX)
            {
                WoundTotal &= 0xfffff0ff;
                WoundTotal += (uint)WoundLevel.Major;
            }
            if (MajorWounds > WOUND_MAX)
            {
                WoundTotal &= 0xffff0fff;
                WoundTotal += (uint)WoundLevel.Critical;
            }
            if (CriticalWounds > WOUND_MAX)
            {
                WoundTotal &= 0xfff0ffff;
                WoundTotal += (uint)WoundLevel.Massive;
            }
            if (MassiveWounds > WOUND_MAX)
            {
                WoundTotal &= 0xff0fffff;
                WoundTotal += (uint)WoundLevel.Mortal;
            }
            if (MortalWounds > WOUND_MAX)
            {
                WoundTotal &= 0xf0ffffff;
                WoundTotal += (uint)WoundLevel.Unsurvivable;
            }
        }
    }

    public enum WoundLevel
    {
        None = 0,
        Negligible = 0x1,
        Minor = 0x10,
        Moderate = 0x100,
        Major = 0x1000,
        Critical = 0x10000,
        Massive = 0x100000,
        Mortal = 0x1000000,
        Unsurvivable = 0x10000000
    }

    public class HitLocationTemplate
    {
        public int Id;
        public string Name;
        public int NaturalArmor;
        public float DamageMultiplier;
        public uint CrippleWound;
        public uint SeverWound;
        public bool IsMotive;
        public bool IsRangedWeaponHolder;
        public bool IsMeleeWeaponHolder;
        public bool IsVital;
        public Dictionary<Stance, int> HitProbabilityMap;
    }

    public class HitLocation
    {
        public Wounds Wounds;
        public bool IsCybernetic;
        
        public bool IsSevered
        {
            get
            {
                return Wounds.WoundTotal >= (uint)Template.SeverWound;
            }
        }

        public bool IsCrippled
        {
            get
            {
                return Wounds.WoundTotal >= (uint)Template.CrippleWound;
            }
        }
        
        public HitLocationTemplate Template { get; private set; }
        public HitLocation(HitLocationTemplate template)
        {
            Wounds = new Wounds();
            IsCybernetic = false;
            Template = template;
        }

        public override string ToString()
        {
            if(IsSevered)
            {
                return Template.Name + ": <color=red>Severed</color>";
            }
            else if(IsCrippled)
            {
                return Template.Name + ": <color=red>Crippled</color>";
            }
            else if(Wounds.WoundTotal >= (uint)WoundLevel.Unsurvivable)
            {
                return Template.Name + ": <color=red>Unsurvivable</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Mortal)
            {
                return Template.Name + ":<color=red> Mortal</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Massive)
            {
                return Template.Name + ": <color=maroon>Massive</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Critical)
            {
                return Template.Name + ": <color=maroon>Critical</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Major)
            {
                return Template.Name + ": <color=orange>Major</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Moderate)
            {
                return Template.Name + ": <color=orange>Moderate</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Minor)
            {
                return Template.Name + ": <color=green>Minor</color>";
            }
            else if (Wounds.WoundTotal >= (uint)WoundLevel.Negligible)
            {
                return Template.Name + ": <color=green>Negligible</color>";
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint) WoundLevel.Massive,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = (uint)WoundLevel.Moderate,
                    SeverWound = (uint)WoundLevel.Major,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = (uint)WoundLevel.Unsurvivable,
                    SeverWound = (uint)WoundLevel.Unsurvivable,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = 3 * (uint)WoundLevel.Major,
                    SeverWound = 3 * (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = true,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = 3 * (uint)WoundLevel.Major,
                    SeverWound = 3 * (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = true,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = true,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = true,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint) WoundLevel.Massive,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = (uint)WoundLevel.Moderate,
                    SeverWound = (uint)WoundLevel.Major,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = (uint)WoundLevel.Unsurvivable,
                    SeverWound = (uint)WoundLevel.Unsurvivable,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = 3 * (uint)WoundLevel.Major,
                    SeverWound = 3 * (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = true,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = 2 * (uint)WoundLevel.Major,
                    SeverWound = 2 * (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = 3 * (uint)WoundLevel.Major,
                    SeverWound = 3 * (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = true,
                    IsVital = false
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
                    CrippleWound = 2 * (uint)WoundLevel.Major,
                    SeverWound = 2 * (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = true,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = true,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = false,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = true
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Critical,
                    SeverWound = (uint)WoundLevel.Massive,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
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
