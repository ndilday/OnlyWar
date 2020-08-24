using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Equippables
{
    class ImperialEquippables
    {
        private static ImperialEquippables _instance;
        public static ImperialEquippables Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ImperialEquippables();
                }
                return _instance;
            }
        }

        private ImperialEquippables() { }

        public RangedWeaponTemplate Boltgun = new RangedWeaponTemplate
        {
            Id = 0,
            Name = "Boltgun",
            Accuracy = 0,
            ArmorPiercing = 0,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 1,
            BaseStrength = 6,
            MaximumDistance = 1000,
            AmmoCapacity = 30,
            RelatedSkill = TempBaseSkillList.Instance.Bolter
        };

        public RangedWeaponTemplate BoltPistol = new RangedWeaponTemplate
        {
            Id = 1,
            Name = "Bolt Pistol",
            Accuracy = 0,
            ArmorPiercing = 0,
            Location = EquipLocation.OneHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 1,
            BaseStrength = 6,
            MaximumDistance = 500,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Bolter
        };

        // TODO: figure out how to model flamers
        public RangedWeaponTemplate Flamer = new RangedWeaponTemplate
        {
            Id = 2,
            Name = "Flamer",
            Accuracy = 0,
            ArmorPiercing = 0,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1.0f,
            RateOfFire = 1,
            BaseStrength = 6,
            MaximumDistance = 10,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Flamer
        };

        // TODO: handle self-danger of plasma weapons in supercharge mode
        public RangedWeaponTemplate PlasmaGun = new RangedWeaponTemplate
        {
            Id = 3,
            Name = "Plasma Gun",
            Accuracy = 0,
            ArmorPiercing = 15,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1.0f,
            RateOfFire = 1,
            BaseStrength = 10.5f,
            MaximumDistance = 1000,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        // TODO: handle self-danger of plasma weapons in supercharge mode
        public RangedWeaponTemplate MeltaGun = new RangedWeaponTemplate
        {
            Id = 4,
            Name = "Melta Gun",
            Accuracy = 0,
            ArmorPiercing = 20,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1.0f,
            RateOfFire = 1,
            BaseStrength = 12,
            MaximumDistance = 200,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        // missile launcher, heavy bolter, multi-melta, lascannon, plasma cannon
        public RangedWeaponTemplate HeavyBolter = new RangedWeaponTemplate
        {
            Id = 5,
            Name = "Heavy Bolter",
            Accuracy = 0,
            ArmorPiercing = 5,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 3,
            BaseStrength = 7.5f,
            MaximumDistance = 1600,
            AmmoCapacity = 100,
            RelatedSkill = TempBaseSkillList.Instance.GunneryBolter
        };

        public RangedWeaponTemplate Lascannon = new RangedWeaponTemplate
        {
            Id = 6,
            Name = "Lascannon",
            Accuracy = 0,
            ArmorPiercing = 15,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1,
            RateOfFire = 1,
            BaseStrength = 13.5f,
            MaximumDistance = 2000,
            AmmoCapacity = 100,
            RelatedSkill = TempBaseSkillList.Instance.Lascannon
        };

        public MeleeWeaponTemplate Chainsword = new MeleeWeaponTemplate
        {
            Id = 1,
            Name = "Chainsword",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 0,
            PenetrationMultiplier = 1.5f,
            ExtraAttacks = 1,
            ParryModifier = 0,
            Accuracy = 0,
            ArmorPiercing = 0,
            RelatedSkill = TempBaseSkillList.Instance.Sword
        };

        public MeleeWeaponTemplate ChainFist = new MeleeWeaponTemplate
        {
            Id = 2,
            Name = "Chainfist",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.5f,
            ExtraDamage = 0,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = -1,
            ArmorPiercing = 20,
            RelatedSkill = TempBaseSkillList.Instance.Fist
        };

        public MeleeWeaponTemplate Eviscerator = new MeleeWeaponTemplate
        {
            Id = 3,
            Name = "Eviscerator",
            Location = EquipLocation.TwoHand,
            StrengthMultiplier = 0.5f,
            ExtraDamage = 0,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = -1,
            ArmorPiercing = 20,
            RelatedSkill = TempBaseSkillList.Instance.Sword
        };

        public MeleeWeaponTemplate Crozius = new MeleeWeaponTemplate
        {
            Id = 4,
            Name = "Crozius Arcanum",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 1,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = 0,
            Accuracy = 0,
            ArmorPiercing = 5,
            RelatedSkill = TempBaseSkillList.Instance.Axe
        };

        public MeleeWeaponTemplate ForceAxe = new MeleeWeaponTemplate
        {
            Id = 5,
            Name = "Force axe",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 1,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = 0,
            ArmorPiercing = 10,
            RelatedSkill = TempBaseSkillList.Instance.Axe
        };

        public MeleeWeaponTemplate ForceSword = new MeleeWeaponTemplate
        {
            Id = 6,
            Name = "Force sword",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 0,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = 0,
            Accuracy = 0,
            ArmorPiercing = 15,
            RelatedSkill = TempBaseSkillList.Instance.Sword
        };

        public MeleeWeaponTemplate PowerAxe = new MeleeWeaponTemplate
        {
            Id = 7,
            Name = "Power axe",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 1,
            PenetrationMultiplier = 1f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = 0,
            ArmorPiercing = 10,
            RelatedSkill = TempBaseSkillList.Instance.Axe
        };

        public MeleeWeaponTemplate PowerSword = new MeleeWeaponTemplate
        {
            Id = 8,
            Name = "Power sword",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 0,
            PenetrationMultiplier = 1f,
            ExtraAttacks = 0,
            ParryModifier = 0,
            Accuracy = 0,
            ArmorPiercing = 15,
            RelatedSkill = TempBaseSkillList.Instance.Sword
        };

        public MeleeWeaponTemplate PowerFist = new MeleeWeaponTemplate
        {
            Id = 9,
            Name = "Power fist",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.5f,
            ExtraDamage = 0,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = -1,
            ArmorPiercing = 15,
            RelatedSkill = TempBaseSkillList.Instance.Fist
        };

        public MeleeWeaponTemplate ServoArm = new MeleeWeaponTemplate
        {
            Id = 10,
            Name = "Servo-arm",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.5f,
            ExtraDamage = 0,
            PenetrationMultiplier = 3f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = -1,
            ArmorPiercing = 10,
            RelatedSkill = TempBaseSkillList.Instance.ServoArm
        };

        public MeleeWeaponTemplate MasterPowerSword = new MeleeWeaponTemplate
        {
            Id = 11,
            Name = "Master crafted power sword",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 0,
            PenetrationMultiplier = 2f,
            ExtraAttacks = 0,
            ParryModifier = 0,
            Accuracy = 0,
            ArmorPiercing = 15,
            RelatedSkill = TempBaseSkillList.Instance.Sword
        };

        public MeleeWeaponTemplate ThunderHammer = new MeleeWeaponTemplate
        {
            Id = 12,
            Name = "Thunder hammer",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.5f,
            ExtraDamage = 0,
            PenetrationMultiplier = 3f,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = -1,
            ArmorPiercing = 15,
            RelatedSkill = TempBaseSkillList.Instance.Axe
        };

        public ArmorTemplate PowerArmor = new ArmorTemplate
        {
            Id = 1,
            Name = "Power Armor Mk VII",
            ArmorProvided = 20,
            Location = EquipLocation.Body
        };
    }
}
