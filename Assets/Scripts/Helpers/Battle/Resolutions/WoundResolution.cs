using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Resolutions
{
    public class WoundResolution
    {
        public BattleSoldier Soldier { get; private set; }
        public float Damage { get; private set; }
        public HitLocation HitLocation { get; private set; }

        public WoundResolution(BattleSoldier soldier, float damage, HitLocation hitLocation)
        {
            Soldier = soldier;
            Damage = damage;
            HitLocation = hitLocation;
        }
    }
}
