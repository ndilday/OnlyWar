using System.Collections.Generic;
using System.Linq;

namespace OnlyWar.Models.Soldiers
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
        public uint WeeksOfHealing { get; set; }
        
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

        public Wounds(uint woundTotal, uint weeksOfHealing)
        {
            WoundTotal = woundTotal;
            WeeksOfHealing = weeksOfHealing;
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

        public void ApplyWeekOfHealing()
        {
            WeeksOfHealing += 0x11111100;
            // negligible and minor wounds heal automatically
            WoundTotal &= 0xffffff00;
            if(UnsurvivableWounds > 0 && (WeeksOfHealing & 0xf0000000) > 0x60000000)
            {
                byte newMortalWounds = UnsurvivableWounds;
                WeeksOfHealing &= 0x0fffffff;
                WoundTotal &= 0x0fffffff;
                WoundTotal += (uint)(newMortalWounds * 0x01000000);
            }
            if (MortalWounds > 0 && (WeeksOfHealing & 0x0f000000) > 0x05000000)
            {
                byte newMassiveWounds = MortalWounds;
                WeeksOfHealing &= 0xf0ffffff;
                WoundTotal &= 0xf0ffffff;
                WoundTotal += (uint)(newMassiveWounds * 0x00100000);
            }
            if (MassiveWounds > 0 && (WeeksOfHealing & 0x00f00000) > 0x00400000)
            {
                byte newCriticalWounds = MassiveWounds;
                WeeksOfHealing &= 0xff0fffff;
                WoundTotal &= 0xff0fffff;
                WoundTotal += (uint)(newCriticalWounds * 0x00010000);
            }
            if (CriticalWounds > 0 && (WeeksOfHealing & 0x000f0000) > 0x00030000)
            {
                byte newMajorWounds = CriticalWounds;
                WeeksOfHealing &= 0xfff0ffff;
                WoundTotal &= 0xfff0ffff;
                WoundTotal += (uint)(newMajorWounds * 0x00001000);
            }
            if (MajorWounds > 0 && (WeeksOfHealing & 0x0000f000) > 0x00002000)
            {
                byte newModerateWounds = MajorWounds;
                WeeksOfHealing &= 0xffff0fff;
                WoundTotal &= 0xffff0fff;
                WoundTotal += (uint)(newModerateWounds * 0x00000100);
            }
            if (ModerateWounds > 0 && (WeeksOfHealing & 0x00000f00) > 0x00000100)
            {
                byte newMinorWounds = ModerateWounds;
                WeeksOfHealing &= 0xfffff0ff;
                WoundTotal &= 0xfffff0ff;
                WoundTotal += (uint)(newMinorWounds * 0x00000010);
            }
        }

        public byte RecoveryTimeLeft()
        {
            if (UnsurvivableWounds > 0)
            {
                return (byte)(28 - (WeeksOfHealing & 0xf0000000) / 0x10000000);
            }
            if (MortalWounds > 0)
            {
                return (byte)(21 - (WeeksOfHealing & 0x0f000000) / 0x01000000);
            }
            if (MassiveWounds > 0)
            {
                return (byte)(15 - (WeeksOfHealing & 0x00f00000) / 0x00100000);
            }
            if (CriticalWounds > 0)
            {
                return (byte)(10 - (WeeksOfHealing & 0x000f0000) / 0x00010000);
            }
            if (MajorWounds > 0)
            {
                return (byte)(6 - (WeeksOfHealing & 0x0000f000) / 0x00001000);
            }
            if (ModerateWounds > 0)
            {
                return (byte)(3 - (WeeksOfHealing & 0x00000f00) / 0x00000100);
            }
            if (MinorWounds > 0)
            {
                return 1;
            }
            return 0;
        }
    
        public void HealWounds()
        {
            WoundTotal = 0;
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
        public float NaturalArmor;
        public float WoundMultiplier;
        public uint CrippleWound;
        public uint SeverWound;
        public bool IsMotive;
        public bool IsRangedWeaponHolder;
        public bool IsMeleeWeaponHolder;
        public bool IsVital;
        public int[] HitProbabilityMap;
    }

    public class HitLocation
    {
        public Wounds Wounds;
        public bool IsCybernetic;
        public float Armor;
        
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
            Wounds = new Wounds(0, 0);
            IsCybernetic = false;
            Armor = 0;
            Template = template;
        }

        public HitLocation(HitLocationTemplate template, bool isCybernetic, float armor, 
            uint woundTotal, uint weeksOfHealing)
        {
            Wounds = new Wounds(woundTotal, weeksOfHealing);
            IsCybernetic = isCybernetic;
            Armor = armor;
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

        public BodyTemplate(IEnumerable<HitLocationTemplate> hitLocations)
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

        static readonly List<HitLocationTemplate> list = new List<HitLocationTemplate>
            {
                new HitLocationTemplate
                {
                    Id = 0,
                    Name = "Brain",
                    NaturalArmor = 2,
                    WoundMultiplier = 4,
                    HitProbabilityMap = new int[3] { 30, 30, 30 },
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
                    WoundMultiplier = 4,
                    HitProbabilityMap = new int[3] { 1, 1, 1 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 75, 75, 75 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 480, 480, 30 },
                    CrippleWound = (uint)WoundLevel.Massive,
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 96, 96, 15 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 96, 96, 15 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 20, 20, 20 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 20, 20, 20 },
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
                    WoundMultiplier = 1.5f,
                    HitProbabilityMap = new int[3] { 100, 100, 10 },
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
                    WoundMultiplier = 1,
                   HitProbabilityMap = new int[3] { 160, 80, 1 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 160, 80, 1 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 15, 7, 0 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 15, 7, 0 },
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
                }
            };

        private HumanBodyTemplate() : base(list) { }
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

        static readonly List<HitLocationTemplate> list = new List<HitLocationTemplate>
            {
                new HitLocationTemplate
                {
                    Id = 0,
                    Name = "Brain",
                    NaturalArmor = 2,
                    WoundMultiplier = 4,
                    HitProbabilityMap = new int[3] { 30, 30, 30 },
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
                    WoundMultiplier = 4,
                    HitProbabilityMap = new int[3] { 1, 1, 1 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 75, 75, 75 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 480, 480, 30 },
                    CrippleWound = (uint)WoundLevel.Massive,
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 96, 96, 15 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 72, 72, 15 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 96, 96, 15 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 72, 72, 15 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 20, 20, 20 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 20, 20, 20 },
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
                    WoundMultiplier = 1.5f,
                    HitProbabilityMap = new int[3] { 100, 100, 10 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 160, 80, 1 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 160, 80, 1 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 15, 7, 0 },
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
                    WoundMultiplier = 1,
                    HitProbabilityMap = new int[3] { 15, 7, 0 },
                    CrippleWound = (uint)WoundLevel.Major,
                    SeverWound = (uint)WoundLevel.Critical,
                    IsMotive = true,
                    IsRangedWeaponHolder = false,
                    IsMeleeWeaponHolder = false,
                    IsVital = false
                }
            };

        private TyranidWarriorBodyTemplate() : base(list) { }
    }

    public class Body
    {
        public HitLocation[] HitLocations { get; private set; }
        public Dictionary<Stance, int> TotalProbabilityMap { get; private set; }

        public Body(List<HitLocation> hitLocations)
        {
            HitLocations = hitLocations.ToArray();
            TotalProbabilityMap = new Dictionary<Stance, int>
            {
                [Stance.Standing] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[(int)Stance.Standing]),
                [Stance.Kneeling] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[(int)Stance.Kneeling]),
                [Stance.Prone] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[(int)Stance.Prone])
            };
        }

        public Body(BodyTemplate template)
        {
            HitLocations = template.HitLocations.Select(hlt => new HitLocation(hlt)).ToArray();
            TotalProbabilityMap = new Dictionary<Stance, int>
            {
                [Stance.Standing] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[(int)Stance.Standing]),
                [Stance.Kneeling] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[(int)Stance.Kneeling]),
                [Stance.Prone] = HitLocations.Sum(hl => hl.Template.HitProbabilityMap[(int)Stance.Prone])
            };
        }
    }
}
