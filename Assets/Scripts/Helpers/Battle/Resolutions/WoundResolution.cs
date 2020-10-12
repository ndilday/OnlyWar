using OnlyWar.Scripts.Models.Equippables;
using OnlyWar.Scripts.Models.Soldiers;

namespace OnlyWar.Scripts.Helpers.Battle.Resolutions
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
