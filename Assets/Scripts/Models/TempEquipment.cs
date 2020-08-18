
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

        public WeaponTemplate Boltgun = new WeaponTemplate
        {
            Id = 0,
            Name = "Boltgun",
            ArmorPiercing = 0,
            Location = EquipLocation.TwoHand,
            PenetrationMultiplier = 1.5f,
            RateOfFire = 3,
            RangeBands = new RangeBands
            {
                Close = new RangeBand
                {
                    Accuracy = 0,
                    Range = 100,
                    Strength = 5
                },
                Medium = new RangeBand
                {
                    Accuracy = -1,
                    Range = 200,
                    Strength = 4
                },
                Long = new RangeBand
                {
                    Accuracy = -2,
                    Range = 400,
                    Strength = 2
                }
            }
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
