
namespace Iam.Scripts.Models
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

        public RangedWeaponTemplate Boltgun = new RangedWeaponTemplate
        {
            Id = 0,
            Name = "Boltgun",
            Accuracy = 0,
            ArmorPiercing = 0,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 2.0f,
            RateOfFire = 3,
            BaseStrength = 6,
            MaximumDistance = 1000
        };

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
            MaximumDistance = 750
        };

        public ArmorTemplate PowerArmor = new ArmorTemplate
        {
            Id = 1,
            Name = "Power Armor Mk VII",
            ArmorProvided = 20,
            Location = EquipLocation.Body
        };

        public ArmorTemplate Chitin = new ArmorTemplate
        {
            Id = 2,
            Name = "Tyranid 1.5cm Chitin",
            ArmorProvided = 15,
            Location = EquipLocation.Body
        };
    }
}
