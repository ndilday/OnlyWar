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
            Accuracy = 2,
            ArmorMultiplier = 1,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 9,
            Recoil = 1,
            BaseStrength = 6,
            MaximumDistance = 1000,
            DoesDamageDegradeWithRange = false,
            Bulk=4,
            RequiredStrength=10,
            AmmoCapacity = 30,
            RelatedSkill = TempBaseSkillList.Instance.Bolter
        };

        public RangedWeaponTemplate BoltPistol = new RangedWeaponTemplate
        {
            Id = 1,
            Name = "Bolt Pistol",
            Accuracy = 1,
            ArmorMultiplier = 1,
            Location = EquipLocation.OneHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 3,
            Recoil = 3,
            BaseStrength = 6,
            MaximumDistance = 500,
            DoesDamageDegradeWithRange = false,
            Bulk=2,
            RequiredStrength=10,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Bolter
        };

        // TODO: figure out how to model flamers
        public RangedWeaponTemplate Flamer = new RangedWeaponTemplate
        {
            Id = 2,
            Name = "Flamer",
            Accuracy = 6,
            ArmorMultiplier = 1,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1.0f,
            RateOfFire = 1,
            Recoil = 1,
            BaseStrength = 6,
            MaximumDistance = 30,
            DoesDamageDegradeWithRange = true,
            Bulk = 3,
            RequiredStrength = 5,
            AmmoCapacity = 50,
            RelatedSkill = TempBaseSkillList.Instance.Flamer
        };

        // TODO: handle self-danger of plasma weapons in supercharge mode
        public RangedWeaponTemplate PlasmaGun = new RangedWeaponTemplate
        {
            Id = 3,
            Name = "Plasma Gun",
            Accuracy = 2,
            ArmorMultiplier = 0.25f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1.0f,
            RateOfFire = 3,
            Recoil = 2,
            BaseStrength = 10.5f,
            MaximumDistance = 1000,
            DoesDamageDegradeWithRange = true,
            Bulk = 3,
            RequiredStrength = 9,
            AmmoCapacity = 30,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        // TODO: handle self-danger of plasma weapons in supercharge mode
        public RangedWeaponTemplate MeltaGun = new RangedWeaponTemplate
        {
            Id = 4,
            Name = "Melta Gun",
            Accuracy = 2,
            ArmorMultiplier = 0.2f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 4.0f,
            RateOfFire = 3,
            Recoil = 1,
            BaseStrength = 12,
            MaximumDistance = 200,
            DoesDamageDegradeWithRange = true,
            Bulk = 4,
            RequiredStrength = 10,
            AmmoCapacity = 20,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        // missile launcher, heavy bolter, multi-melta, lascannon, plasma cannon
        public RangedWeaponTemplate HeavyBolter = new RangedWeaponTemplate
        {
            Id = 5,
            Name = "Heavy Bolter",
            Accuracy = 2,
            ArmorMultiplier = 0.75f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 12,
            Recoil = 1,
            BaseStrength = 7.5f,
            MaximumDistance = 1600,
            DoesDamageDegradeWithRange = false,
            Bulk = 8,
            RequiredStrength = 16,
            AmmoCapacity = 150,
            RelatedSkill = TempBaseSkillList.Instance.GunneryBolter
        };

        public RangedWeaponTemplate Lascannon = new RangedWeaponTemplate
        {
            Id = 6,
            Name = "Lascannon",
            Accuracy = 10,
            ArmorMultiplier = 0.25f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1f,
            RateOfFire = 1,
            Recoil = 1,
            BaseStrength = 13.5f,
            MaximumDistance = 2000,
            DoesDamageDegradeWithRange = true,
            Bulk = 8,
            RequiredStrength = 15,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Lascannon
        };

        public RangedWeaponTemplate PlasmaPistol = new RangedWeaponTemplate
        {
            Id = 7,
            Name = "Plasma Pistol",
            Accuracy = 2,
            ArmorMultiplier = 0.25f,
            Location = EquipLocation.OneHand,
            PenetrationMultiplier = 1.0f,
            RateOfFire = 3,
            Recoil = 2,
            BaseStrength = 10.5f,
            MaximumDistance = 500,
            DoesDamageDegradeWithRange = true,
            Bulk = 2,
            RequiredStrength = 6,
            AmmoCapacity = 40,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        // TODO: handle multiple firing modes of missile launcher
        public RangedWeaponTemplate MissileLauncher = new RangedWeaponTemplate
        {
            Id = 8,
            Name = "Missile Launcher",
            Accuracy = 3,
            ArmorMultiplier = 0.5f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 3f,
            RateOfFire = 1,
            Recoil = 1,
            BaseStrength = 12f,
            MaximumDistance = 2000,
            DoesDamageDegradeWithRange = false,
            Bulk = 8,
            RequiredStrength = 12,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.MissileLauncher
        };

        // TODO: handle weird damage falloff of multi-melta (works out to wound x of 4.467 on average); for now, just using 4
        public RangedWeaponTemplate MultiMelta = new RangedWeaponTemplate
        {
            Id = 9,
            Name = "Multi-melta",
            Accuracy = 3,
            ArmorMultiplier = 0.2f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 4f,
            RateOfFire = 1,
            BaseStrength = 12f,
            MaximumDistance = 1000,
            DoesDamageDegradeWithRange = true,
            Bulk = 6,
            RequiredStrength = 10,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        public RangedWeaponTemplate PlasmaCannon = new RangedWeaponTemplate
        {
            Id = 10,
            Name = "Plasma Cannon",
            Accuracy = 6,
            ArmorMultiplier = 0.25f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1f,
            RateOfFire = 10,
            Recoil = 2,
            BaseStrength = 10.5f,
            MaximumDistance = 1500,
            DoesDamageDegradeWithRange = true,
            Bulk = 6,
            RequiredStrength = 15,
            AmmoCapacity = 10,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
        };

        // TODO: how to model the mortal wound ability?
        public RangedWeaponTemplate SniperRifle = new RangedWeaponTemplate
        {
            Id = 11,
            Name = "Sniper Rifle",
            Accuracy = 9,
            ArmorMultiplier = 0.5f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1f,
            RateOfFire = 1,
            Recoil = 1,
            BaseStrength = 6f,
            MaximumDistance = 1500,
            DoesDamageDegradeWithRange = true,
            Bulk = 8,
            RequiredStrength = 10,
            AmmoCapacity = 20,
            RelatedSkill = TempBaseSkillList.Instance.Sniper
        };

        public RangedWeaponTemplate Shotgun = new RangedWeaponTemplate
        {
            Id = 12,
            Name = "Astartes shotgun",
            Accuracy = 3,
            ArmorMultiplier = 1,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 2f,
            RateOfFire = 2,
            Recoil = 3,
            BaseStrength = 6f,
            MaximumDistance = 100,
            DoesDamageDegradeWithRange = true,
            Bulk = 4,
            RequiredStrength = 11,
            AmmoCapacity = 8,
            RelatedSkill = TempBaseSkillList.Instance.Plasma
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
            ArmorMultiplier = 1,
            RequiredStrength = 3,
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
            ArmorMultiplier = 0.75f,
            RequiredStrength = 3,
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
            ArmorMultiplier = 0.2f,
            RequiredStrength = 12,
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
            ArmorMultiplier = 0.75f,
            RequiredStrength = 12,
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
            ArmorMultiplier = 0.5f,
            RequiredStrength = 11,
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
            ArmorMultiplier = 0.5f,
            RequiredStrength = 11,
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
            ArmorMultiplier = 0.5f,
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
            ArmorMultiplier = 0.25f,
            RequiredStrength = 10,
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
            ArmorMultiplier = 0.25f,
            RequiredStrength = 3,
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
            ArmorMultiplier = 0.5f,
            RequiredStrength = 5,
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
            ArmorMultiplier = 0.25f,
            RequiredStrength = 10,
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
            ArmorMultiplier = 0.25f,
            RequiredStrength = 12,
            RelatedSkill = TempBaseSkillList.Instance.Axe
        };

        public MeleeWeaponTemplate Fist = new MeleeWeaponTemplate
        {
            Id = 13,
            Name = "Fist",
            Location = EquipLocation.OneHand,
            StrengthMultiplier = 0.25f,
            ExtraDamage = 0,
            PenetrationMultiplier = 1,
            ExtraAttacks = 0,
            ParryModifier = -1,
            Accuracy = 0,
            ArmorMultiplier = 1,
            RequiredStrength = 3,
            RelatedSkill = TempBaseSkillList.Instance.Fist
        };

        public ArmorTemplate PowerArmor = new ArmorTemplate
        {
            Id = 1,
            Name = "Power Armor Mk VII",
            ArmorProvided = 20,
            Location = EquipLocation.Body
        };

        public ArmorTemplate CarapaceArmor = new ArmorTemplate
        {
            Id = 2,
            Name = "Carapace Armor",
            ArmorProvided = 15,
            Location = EquipLocation.Body
        };
    }
}
