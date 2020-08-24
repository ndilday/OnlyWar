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

        public ArmorTemplate Chitin = new ArmorTemplate
        {
            Id = 2,
            Name = "Tyranid 1.5cm Chitin",
            ArmorProvided = 15,
            Location = EquipLocation.Body
        };
    }
}
