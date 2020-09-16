using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Resolutions
{
    public class WoundResolution
    {
        public BattleSoldier Inflicter { get; }
        public WeaponTemplate Weapon { get; }
        public BattleSoldier Suffererer { get; }
        public float Damage { get; }
        public HitLocation HitLocation { get; }

        public WoundResolution(BattleSoldier inflicter, WeaponTemplate weapon, BattleSoldier sufferer, float damage, HitLocation hitLocation)
        {
            Inflicter = inflicter;
            Weapon = weapon;
            Suffererer = sufferer;
            Damage = damage;
            HitLocation = hitLocation;
        }
    }
}
