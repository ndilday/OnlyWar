using OnlyWar.Scripts.Models.Planets;

namespace OnlyWar.Scripts.Helpers.Battle
{
    public class BattleConfiguration
    {
    }

    public static class BattleConfigurationBuilder
    {
        public static BattleConfiguration BuildBattleConfiguration(Planet planet)
        {
            if(!DoesBattleBreakOut(planet))
            {
                return null;
            }
            return null;
        }

        private static bool DoesBattleBreakOut(Planet planet)
        {
            return false;
        }
    }
}
