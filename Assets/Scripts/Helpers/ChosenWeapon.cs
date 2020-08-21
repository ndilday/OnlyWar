using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers
{
    public class ChosenWeapon
    {
        public Weapon ActiveWeapon { get; private set; }
        public Soldier Soldier { get; private set; }
        public ChosenWeapon(Weapon weapon, Soldier soldier)
        {
            ActiveWeapon = weapon;
            Soldier = soldier;
        }

        public float GetStrengthAtRange(float range)
        {
            return ActiveWeapon.Template.BaseStrength * (1 - (range / ActiveWeapon.Template.MaximumDistance));
        }
    }
}
