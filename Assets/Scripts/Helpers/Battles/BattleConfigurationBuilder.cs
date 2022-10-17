using OnlyWar.Builders;
using OnlyWar.Helpers.Battles.Placers;
using OnlyWar.Models.Battles;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using System.Collections.Generic;
using System.Linq;

namespace OnlyWar.Helpers.Battles
{
    public static class BattleConfigurationBuilder
    {
        public static IReadOnlyList<BattleConfiguration> BuildBattleConfigurations(Planet planet, int playerFactionId, int alliedFactionId)
        {
            
            bool playerForcePresent = planet.PlanetFactionMap.ContainsKey(playerFactionId) &&
                planet.PlanetFactionMap[playerFactionId].LandedSquads.Count > 0;
            var opFactions = planet.PlanetFactionMap.Values
                .Where(f => !f.Faction.IsDefaultFaction && !f.Faction.IsPlayerFaction);
            if (!playerForcePresent || opFactions == null || opFactions.Count() == 0) return null;
            // does it really make sense for faction squads on planet to be
            // independent from the faction map?
            return null;
        }

        private static Unit GenerateNewArmy(PlanetFaction planetFaction, Planet planet)
        {
            int factionId = planetFaction.Faction.Id;
            // if we got here, the assaulting force doesn't have an army generated
            // generate an army (and decrement it from the population
            Unit newArmy = TempArmyBuilder.GenerateArmyFromPlanetFaction(planetFaction);

            // add unit to faction
            planetFaction.Faction.Units.Add(newArmy);

            // add unit to planet
            planetFaction.LandedSquads.AddRange(newArmy.Squads);

            // modify planetFaction based on new unit
            int headcount = newArmy.GetAllMembers().Count();
            float ratio = ((float)planetFaction.PDFMembers) /
                (planetFaction.Population + planetFaction.PDFMembers);
            int pdfHeadcount = (int)(headcount * ratio);
            headcount -= pdfHeadcount;
            planetFaction.PDFMembers -= pdfHeadcount;
            if(planetFaction.PDFMembers < 0)
            {
                headcount -= planetFaction.PDFMembers;
                planetFaction.PDFMembers = 0;
            }
            planetFaction.Population -= headcount;
            if(planetFaction.Population < 0)
            {
                planetFaction.Population = 0;
                // TODO: remove this planetFaction from the planet?
            }
            return newArmy;
        }
    
        private static BattleConfiguration ConstructAnnihilationConfiguration(Planet planet)
        {
            List<Squad> playerSquads = new List<Squad>();
            List<Squad> opposingSquads = new List<Squad>();
            foreach(PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
            {
                if(planetFaction.Faction.IsPlayerFaction)
                {
                    foreach(Squad squad in planetFaction.LandedSquads)
                    {
                        if(!squad.IsInReserve)
                        {
                            playerSquads.Add(squad);
                        }
                    }
                }
                else if(!planetFaction.Faction.IsDefaultFaction)
                {
                    foreach(Squad squad in planetFaction.LandedSquads)
                    {
                        if(!squad.IsInReserve)
                        {
                            opposingSquads.Add(squad);
                        }
                    }
                }
            }

            BattleConfiguration config = new BattleConfiguration();
            config.PlayerSquads = CreateBattleSquadList(playerSquads, true);
            config.OpposingSquads = CreateBattleSquadList(opposingSquads, false);
            config.Planet = planet;
            config.Grid = new BattleGrid();
            AnnihilationPlacer placer = new AnnihilationPlacer(config.Grid);
            placer.PlaceSquads(config.PlayerSquads, config.OpposingSquads);
            return config;
        }

        private static BattleConfiguration ConstructOpposingAmbushConfiguration(Planet planet)
        {
            List<Squad> playerSquads = new List<Squad>();
            List<Squad> opposingSquads = new List<Squad>();
            foreach (PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
            {
                if (planetFaction.Faction.IsPlayerFaction)
                {
                    foreach (Squad squad in planetFaction.LandedSquads)
                    {
                        if (!squad.IsInReserve)
                        {
                            playerSquads.Add(squad);
                        }
                    }
                }
                else if (!planetFaction.Faction.IsDefaultFaction)
                {
                    foreach (Squad squad in planetFaction.LandedSquads)
                    {
                        if (!squad.IsInReserve)
                        {
                            opposingSquads.Add(squad);
                        }
                    }
                }
            }

            BattleConfiguration config = new BattleConfiguration();
            config.PlayerSquads = CreateBattleSquadList(playerSquads, true);
            config.OpposingSquads = CreateBattleSquadList(opposingSquads, false);
            config.Planet = planet;
            config.Grid = new BattleGrid();
            AmbushPlacer placer = new AmbushPlacer(config.Grid);
            placer.PlaceSquads(config.PlayerSquads, config.OpposingSquads);
            return config;
        }

        private static List<BattleSquad> CreateBattleSquadList(IReadOnlyList<Squad> squads,
                                                               bool isPlayerSquad)
        {
            List<BattleSquad> battleSquadList = new List<BattleSquad>();
            foreach (Squad squad in squads)
            {
                BattleSquad bs = new BattleSquad(isPlayerSquad, squad);

                battleSquadList.Add(bs);
            }
            return battleSquadList;
        }
    }
}
