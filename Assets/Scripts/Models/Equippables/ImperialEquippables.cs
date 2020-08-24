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

        public ArmorTemplate PowerArmor = new ArmorTemplate
        {
            Id = 1,
            Name = "Power Armor Mk VII",
            ArmorProvided = 20,
            Location = EquipLocation.Body
        };
    }
}
