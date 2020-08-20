using Iam.Scripts.Models;

namespace Iam.Scripts.Helpers
{
    public class ChosenWeapon
    {
        public RangeBand ActiveRangeBand { get; private set; }
        public Weapon ActiveWeapon { get; private set; }
        public Soldier Soldier { get; private set; }
        public ChosenWeapon(RangeBand band, Weapon weapon, Soldier soldier)
        {
            ActiveRangeBand = band;
            ActiveWeapon = weapon;
            Soldier = soldier;
        }
    }
}
