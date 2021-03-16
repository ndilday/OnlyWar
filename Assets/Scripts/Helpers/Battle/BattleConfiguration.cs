using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Planets;
using OnlyWar.Scripts.Models.Squads;
using System.Collections.Generic;
using System.Linq;

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

        private static bool FactionsCanBattle(Planet planet)
        {
            bool containsNonDefaultNonPlayerSquad = false;
            bool containsActivePlayerSquads = false;
            foreach (KeyValuePair<int, List<Squad>> kvp in planet.FactionSquadListMap)
            {
                Faction faction = kvp.Value[0].ParentUnit.UnitTemplate.Faction;
                if (!faction.IsDefaultFaction && !faction.IsPlayerFaction && kvp.Value.Any(s => !s.IsInReserve))
                {
                    containsNonDefaultNonPlayerSquad = true;
                }
                else if (faction.IsPlayerFaction && kvp.Value.Any(s => !s.IsInReserve))
                {
                    containsActivePlayerSquads =
                        kvp.Value.Any(squad => !squad.IsInReserve);
                }
            }
            foreach(PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
            {

            }

            return containsActivePlayerSquads && containsNonDefaultNonPlayerSquad;
        }
    }
}
