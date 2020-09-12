using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Equippables
{
    public class TempEquipment
    {
        private static TempEquipment _instance = null;
        public static TempEquipment Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempEquipment();
                }
                return _instance;
            }
        }

        public RangedWeaponTemplate Deathspitter = new RangedWeaponTemplate
        {
            Id = 1,
            Name = "Deathspitter",
            Accuracy = 0,
            ArmorMultiplier = 0.75f,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1,
            RateOfFire = 15,
            Recoil = 2,
            BaseStrength = 7.5f,
            MaximumDistance = 750,
            DoesDamageDegradeWithRange = true,
            Bulk = 3,
            RequiredStrength = 12,
            AmmoCapacity = 100,
            RelatedSkill = TempBaseSkillList.Instance.OpponentRanged
        };

        public RangedWeaponTemplate Devourer = new RangedWeaponTemplate
        {
            Id = 2,
            Name = "Devourer",
            Accuracy = 0,
            ArmorMultiplier = 1,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1,
            RateOfFire = 15,
            Recoil = 0,
            BaseStrength = 6f,
            MaximumDistance = 750,
            DoesDamageDegradeWithRange = true,
            Bulk = 2,
            RequiredStrength = 8,
            AmmoCapacity = 100,
            RelatedSkill = TempBaseSkillList.Instance.OpponentRanged
        };

        public MeleeWeaponTemplate ScythingTalons = new MeleeWeaponTemplate
        {
            Id = 3,
            Name = "Scything Talons",
            Accuracy = 1,
            ArmorMultiplier = 1,
            ExtraAttacks = 0,
            ExtraDamage = 0,
            Location = EquipLocation.OneHand,
            ParryModifier = 0,
            PenetrationMultiplier = 1,
            StrengthMultiplier = 0.25f,
            RequiredStrength = 8,
            RelatedSkill = TempBaseSkillList.Instance.OpponentMelee
        };

        public MeleeWeaponTemplate RendingClaws = new MeleeWeaponTemplate
        {
            Id = 4,
            Name = "Rending Claws",
            Accuracy = 1,
            ArmorMultiplier = 0.75f,
            ExtraAttacks = 0,
            ExtraDamage = 0,
            Location = EquipLocation.OneHand,
            ParryModifier = 0,
            PenetrationMultiplier = 3,
            StrengthMultiplier = 0.25f,
            RequiredStrength = 8,
            RelatedSkill = TempBaseSkillList.Instance.OpponentMelee
        };
        public ArmorTemplate LightChitin = new ArmorTemplate
        {
            Id = 2,
            Name = "Tyranid 0.5cm Chitin",
            ArmorProvided = 5,
            Location = EquipLocation.Body
        };

        public ArmorTemplate MediumChitin = new ArmorTemplate
        {
            Id = 3,
            Name = "Tyranid 1cm Chitin",
            ArmorProvided = 10,
            Location = EquipLocation.Body
        };

        public ArmorTemplate HeavyChitin = new ArmorTemplate
        {
            Id = 4,
            Name = "Tyranid 1.5cm Chitin",
            ArmorProvided = 15,
            Location = EquipLocation.Body
        };
    }
}
