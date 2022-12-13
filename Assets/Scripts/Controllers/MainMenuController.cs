using OnlyWar.Builders;
using OnlyWar.Helpers;
using OnlyWar.Models;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using OnlyWar.Helpers.Database.GameState;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OnlyWar.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        //private const int GENERATION_SEED = 0;

        [SerializeField]
        private GameSettings GameSettings;
        // Start is called before the first frame update

        public void NewGameButton_OnClick()
        {
            GenerateNewGame();
            SceneManager.LoadScene("GalaxyView");
        }

        public void LoadGameButton_OnClick()
        {
            // TODO: open file screen
            // load that file
            GameSettings.Sector = new Sector(GameSettings.SectorSize);
            GameStateDataBlob gameData = LoadGameData();

            GameSettings.Sector.GenerateSector(gameData.Characters, gameData.Planets, gameData.Fleets);
            GameSettings.Date = gameData.CurrentDate;
            Dictionary<Faction, List<Unit>> factionUnits = InitalizeUnits(gameData);
            var chapterUnit = factionUnits[GameSettings.Sector.PlayerFaction].First(u => u.ParentUnit == null);
            var soldiers = chapterUnit.GetAllMembers().Select(s => (PlayerSoldier)s);
            GameSettings.Chapter = new Force(chapterUnit, soldiers);
            GameSettings.Chapter.PopulateSquadMap();
            InitalizeRequests(gameData);

            foreach (KeyValuePair<Date, List<EventHistory>> kvp in gameData.History)
            {
                foreach (EventHistory history in kvp.Value)
                {
                    GameSettings.Chapter.AddToBattleHistory(kvp.Key,
                                                            history.EventTitle,
                                                            history.SubEvents);
                }
            }

            SceneManager.LoadScene("GalaxyView");
        }

        private GameStateDataBlob LoadGameData()
        {
            var shipTemplateMap = GameSettings.Sector.Factions.Where(f => f.ShipTemplates != null)
                                                                          .SelectMany(f => f.ShipTemplates.Values)
                                                                          .ToDictionary(s => s.Id);
            var unitTemplateMap = GameSettings.Sector.Factions.Where(f => f.UnitTemplates != null)
                                                              .SelectMany(f => f.UnitTemplates.Values)
                                                              .ToDictionary(u => u.Id);
            var squadTemplateMap = GameSettings.Sector.Factions.Where(f => f.SquadTemplates != null)
                                                               .SelectMany(f => f.SquadTemplates.Values)
                                                               .ToDictionary(s => s.Id);
            var hitLocations = GameSettings.Sector.BodyHitLocationTemplateMap.Values.SelectMany(hl => hl)
                                                                                    .Distinct()
                                                                                    .ToDictionary(hl => hl.Id);
            var soldierTypeMap = GameSettings.Sector.Factions.Where(f => f.SoldierTemplates != null)
                                                             .SelectMany(f => f.SoldierTemplates.Values)
                                                             .ToDictionary(st => st.Id);
            var gameData =
                GameStateDataAccess.Instance.GetData("default.s3db",
                                                     GameSettings,
                                                     GameSettings.Sector.Factions.ToDictionary(f => f.Id),
                                                     GameSettings.Sector.PlanetTemplateMap,
                                                     shipTemplateMap, unitTemplateMap, squadTemplateMap,
                                                     GameSettings.Sector.WeaponSets,
                                                     hitLocations, GameSettings.Sector.BaseSkillMap,
                                                     soldierTypeMap);
            return gameData;
        }

        private Dictionary<Faction, List<Unit>> InitalizeUnits(GameStateDataBlob gameData)
        {
            var factionUnits = gameData.Units.GroupBy(u => u.UnitTemplate.Faction)
                                                         .ToDictionary(g => g.Key, g => g.ToList());
            foreach (Faction faction in GameSettings.Sector.Factions)
            {
                if (factionUnits.ContainsKey(faction))
                {
                    List<Unit> units = factionUnits[faction];
                    faction.Units.AddRange(units);
                    foreach (Unit unit in units)
                    {
                        foreach (Squad squad in unit.GetAllSquads())
                        {
                            if (squad.Location != null)
                            {
                                PlanetFaction playerPlanetFaction;
                                if (!squad.Location.PlanetFactionMap
                                    .ContainsKey(GameSettings.Sector.PlayerFaction.Id))
                                {
                                    playerPlanetFaction = new PlanetFaction(GameSettings.Sector.PlayerFaction);
                                    squad.Location.PlanetFactionMap[GameSettings.Sector.PlayerFaction.Id] =
                                        playerPlanetFaction;
                                }
                                else
                                {
                                    playerPlanetFaction =
                                        squad.Location.PlanetFactionMap[GameSettings.Sector.PlayerFaction.Id];
                                }
                                playerPlanetFaction.LandedSquads.Add(squad);
                            }
                            else if (squad.BoardedLocation != null)
                            {
                                squad.BoardedLocation.LoadSquad(squad);
                            }
                        }
                    }
                }
            }

            return factionUnits;
        }

        private void InitalizeRequests(GameStateDataBlob gameData)
        {
            int currentHighestRequestId = 0;
            foreach (IRequest request in gameData.Requests)
            {
                if (request.Id > currentHighestRequestId)
                {
                    currentHighestRequestId = request.Id;
                }
                // character requests were already hydrated on load,
                // so we only need to hydrate the chapter
                GameSettings.Chapter.Requests.Add(request);
            }
            RequestFactory.Instance.SetCurrentHighestRequestId(currentHighestRequestId);
        }

        public void QuitGameButton_OnClick()
        {
            Application.Quit();
        }

        private void GenerateNewGame()
        {
            GameSettings.Sector = new Sector(GameSettings.SectorSize);
            GameSettings.Sector.GenerateSector(RNG.GetIntBelowMax(0, int.MaxValue));
            // generate chapter
            CreateChapter();
            PlaceStartingForces();
        }

        private void CreateChapter()
        {
            Date trainingStartDate = new Date(GameSettings.Date.Millenium, 
                                              GameSettings.Date.Year - 4, 
                                              1);
            GameSettings.Chapter =
                NewChapterBuilder.CreateChapter(GameSettings.Sector.PlayerFaction,
                                                trainingStartDate,
                                                GameSettings);
            List<string> foundingHistoryEntries = new List<string>
            {
                $"{GameSettings.Chapter.OrderOfBattle.Name} officially forms with its first 1,000 battle brothers."
            };
            GameSettings.Chapter.AddToBattleHistory(GameSettings.Date,
                                                    "Chapter Founding",
                                                    foundingHistoryEntries);
            
            GameSettings.Sector.PlayerFaction.Units.Add(GameSettings.Chapter.OrderOfBattle);
            FoundChapterPlanet();

        }

        private void FoundChapterPlanet()
        {
            // TODO: replace this with a random assignment of starting planet
            // and then have the sector map screen default to zooming in
            // on the Marine starting planet
            var emptyPlanets = GameSettings.Sector.Planets.Values.Where(p => p.ControllingFaction.IsDefaultFaction);
            int max = emptyPlanets.Count();
            int chapterPlanetIndex = RNG.GetIntBelowMax(0, max);
            Planet chapterPlanet = emptyPlanets.ElementAt(chapterPlanetIndex);
            ReplaceChapterPlanetFaction(chapterPlanet);
        }

        private void ReplaceChapterPlanetFaction(Planet chapterPlanet)
        {
            chapterPlanet.ControllingFaction = GameSettings.Sector.PlayerFaction;
            PlanetFaction existingPlanetFaction = 
                chapterPlanet.PlanetFactionMap[GameSettings.Sector.DefaultFaction.Id];
            PlanetFaction homePlanetFaction = new PlanetFaction(GameSettings.Sector.PlayerFaction);
            homePlanetFaction.IsPublic = true;
            homePlanetFaction.Leader = null;
            homePlanetFaction.PDFMembers = existingPlanetFaction.PDFMembers;
            homePlanetFaction.PlayerReputation = 1;
            homePlanetFaction.Population = existingPlanetFaction.Population;
            chapterPlanet.PlanetFactionMap.Remove(existingPlanetFaction.Faction.Id);
            chapterPlanet.PlanetFactionMap[homePlanetFaction.Faction.Id] = homePlanetFaction;
        }

        private void PlaceStartingForces()
        {
            foreach (Planet planet in GameSettings.Sector.Planets.Values)
            {
                // For now, put the chapter on their home planet
                if (planet.ControllingFaction == GameSettings.Sector.PlayerFaction)
                {
                    planet.PlanetFactionMap[GameSettings.Sector.PlayerFaction.Id].LandedSquads
                        .AddRange(GameSettings.Chapter.SquadMap.Values);
                    SetChapterSquadsLocation(planet);
                    foreach(TaskForce fleet in GameSettings.Chapter.TaskForces)
                    {
                        fleet.Planet = planet;
                        fleet.Position = planet.Position;
                        GameSettings.Sector.AddNewFleet(fleet);
                    }
                }
            }
        }

        private void SetChapterSquadsLocation(Planet planet)
        {
            foreach (Squad squad in GameSettings.Chapter.OrderOfBattle.Squads)
            {
                if (squad.Members.Count > 0)
                {
                    squad.Location = planet;
                }
            }
            foreach (Unit unit in GameSettings.Chapter.OrderOfBattle.ChildUnits)
            {
                foreach (Squad squad in unit.Squads)
                {
                    if (squad.Members.Count > 0)
                    {
                        squad.Location = planet;
                    }
                }
            }
        }
    }
}