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
            GameSettings.Chapter = new Chapter(chapterUnit, soldiers);
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
                                if (!squad.Location.FactionSquadListMap.ContainsKey(faction.Id))
                                {
                                    squad.Location.FactionSquadListMap[faction.Id] =
                                        new List<Squad>();
                                }
                                squad.Location.FactionSquadListMap[faction.Id].Add(squad);
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
            Date basicTrainingEndDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 3, 52);
            Date trainingStartDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 4, 1);
            var soldierTemplate = GameSettings.Sector.PlayerFaction.SoldierTemplates[0];
            var soldiers =
                SoldierFactory.Instance.GenerateNewSoldiers(1000, soldierTemplate.Species, GameSettings.Sector.SkillTemplateList)
                .Select(s => new PlayerSoldier(s, $"{TempNameGenerator.GetName()} {TempNameGenerator.GetName()}"))
                .ToList();

            string foo = "";
            SoldierTrainingCalculator trainingHelper =
                new SoldierTrainingCalculator(GameSettings.Sector.BaseSkillMap.Values);
            foreach (PlayerSoldier soldier in soldiers)
            {
                soldier.AddEntryToHistory(trainingStartDate + ": accepted into training");
                if (soldier.PsychicPower > 0)
                {
                    soldier.AddEntryToHistory(trainingStartDate + ": psychic ability detected, acolyte training initiated");
                    // add psychic specific training here
                }
                trainingHelper.EvaluateSoldier(soldier, basicTrainingEndDate);
                soldier.ProgenoidImplantDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 2, RNG.GetIntBelowMax(1, 53));

                foo += $"{(int)soldier.MeleeRating}, {(int)soldier.RangedRating}, {(int)soldier.LeadershipRating}, {(int)soldier.AncientRating}, {(int)soldier.MedicalRating}, {(int)soldier.TechRating}, {(int)soldier.PietyRating}\n";
            }


            System.IO.File.WriteAllText($"{Application.streamingAssetsPath}/ratings.csv", foo);

            GameSettings.Chapter =
                NewChapterBuilder.CreateChapter(soldiers,
                                                GameSettings.Sector.PlayerFaction,
                                                GameSettings.Date.ToString());
            List<string> foundingHistoryEntries = new List<string>
            {
                $"{GameSettings.Chapter.OrderOfBattle.Name} officially forms with its first 1,000 battle brothers."
            };
            GameSettings.Chapter.AddToBattleHistory(GameSettings.Date,
                                                    "Chapter Founding",
                                                    foundingHistoryEntries);
            // post-MOS evaluations
            foreach (PlayerSoldier soldier in soldiers)
            {
                trainingHelper.EvaluateSoldier(soldier, GameSettings.Date);
            }
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
                    planet.FactionSquadListMap[GameSettings.Sector.PlayerFaction.Id] =
                            GameSettings.Chapter.SquadMap.Values.ToList();
                    SetChapterSquadsLocation(planet);
                    foreach(Fleet fleet in GameSettings.Chapter.Fleets)
                    {
                        fleet.Planet = planet;
                        fleet.Position = planet.Position;
                        GameSettings.Sector.AddNewFleet(fleet);
                    }
                }
                else if (planet.ControllingFaction.UnitTemplates != null)
                {
                    int potentialArmies = planet.ControllingFaction
                                                .UnitTemplates
                                                .Values
                                                .Where(ut => ut.IsTopLevelUnit)
                                                .Count();
                    // TODO: generalize this
                    Unit newArmy = TempArmyBuilder.GenerateArmy(
                        RNG.GetIntBelowMax(0, potentialArmies),
                        planet.ControllingFaction);
                    planet.ControllingFaction.Units.Add(newArmy);
                    planet.FactionSquadListMap[planet.ControllingFaction.Id] = newArmy.Squads.ToList();
                    foreach(Squad squad in newArmy.Squads)
                    {
                        squad.Location = planet;
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