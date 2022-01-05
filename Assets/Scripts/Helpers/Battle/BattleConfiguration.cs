using OnlyWar.Builders;
using OnlyWar.Helpers.Battle.Placers;
using OnlyWar.Models;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using System.Collections.Generic;
using System.Linq;

namespace OnlyWar.Helpers.Battle
{
    public class BattleConfiguration
    {
        public IReadOnlyList<BattleSquad> PlayerSquads;
        public IReadOnlyList<BattleSquad> OpposingSquads;
        public Planet Planet;
        public BattleGrid Grid;
    }

    public static class BattleConfigurationBuilder
    {
        public static IReadOnlyList<BattleConfiguration> BuildBattleConfigurations(Planet planet, int playerFactionId, int alliedFactionId)
        {
            
            bool playerForcePresent = planet.FactionSquadListMap.ContainsKey(playerFactionId) &&
                planet.FactionSquadListMap[playerFactionId].Count > 0;
            bool opForPresent = planet.PlanetFactionMap.Values
                .Any(f => !f.Faction.IsDefaultFaction && !f.Faction.IsPlayerFaction);
            if (!playerForcePresent || !opForPresent) return null;
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

            if (!planet.FactionSquadListMap.ContainsKey(factionId))
            {
                planet.FactionSquadListMap[factionId] = new List<Squad>();
            }

            // add unit to faction
            planetFaction.Faction.Units.Add(newArmy);
            
            // add unit to planet
            foreach(Squad squad in newArmy.Squads)
            {
                squad.IsInReserve = false;
                squad.Location = planet;
                planet.FactionSquadListMap[factionId].Add(squad);
            }

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
            foreach(List<Squad> squadList in planet.FactionSquadListMap.Values)
            {
                if(squadList[0].ParentUnit.UnitTemplate.Faction.IsPlayerFaction)
                {
                    foreach(Squad squad in squadList)
                    {
                        if(!squad.IsInReserve)
                        {
                            playerSquads.Add(squad);
                        }
                    }
                }
                else if(!squadList[0].ParentUnit.UnitTemplate.Faction.IsDefaultFaction)
                {
                    foreach(Squad squad in squadList)
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
            config.Grid = new BattleGrid(100, 500);
            AnnihilationPlacer placer = new AnnihilationPlacer(config.Grid);
            placer.PlaceSquads(config.PlayerSquads, config.OpposingSquads);
            return config;
        }

        private static BattleConfiguration ConstructOpposingAmbushConfiguration(Planet planet)
        {
            List<Squad> playerSquads = new List<Squad>();
            List<Squad> opposingSquads = new List<Squad>();
            foreach (List<Squad> squadList in planet.FactionSquadListMap.Values)
            {
                if (squadList[0].ParentUnit.UnitTemplate.Faction.IsPlayerFaction)
                {
                    foreach (Squad squad in squadList)
                    {
                        if (!squad.IsInReserve)
                        {
                            playerSquads.Add(squad);
                        }
                    }
                }
                else if (!squadList[0].ParentUnit.UnitTemplate.Faction.IsDefaultFaction)
                {
                    foreach (Squad squad in squadList)
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
            config.Grid = new BattleGrid(200, 200);
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
