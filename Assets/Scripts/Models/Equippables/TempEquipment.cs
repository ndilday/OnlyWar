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
            Accuracy = -1,
            ArmorPiercing = 5,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1,
            RateOfFire = 5,
            BaseStrength = 7.5f,
            MaximumDistance = 750,
            AmmoCapacity = 100,
            RelatedSkill = TempBaseSkillList.Instance.OpponentRanged
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
            Id = 2,
            Name = "Tyranid 1cm Chitin",
            ArmorProvided = 10,
            Location = EquipLocation.Body
        };

        public ArmorTemplate HeavyChitin = new ArmorTemplate
        {
            Id = 3,
            Name = "Tyranid 1.5cm Chitin",
            ArmorProvided = 15,
            Location = EquipLocation.Body
        };
    }
}
