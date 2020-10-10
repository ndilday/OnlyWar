using OnlyWar.Scripts.Helpers;
using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;
using OnlyWar.Scripts.Helpers.Database;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OnlyWar.Scripts.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        private SoldierTrainingHelper _trainingHelper;

        [SerializeField]
        private GameSettings GameSettings;
        // Start is called before the first frame update

        public void NewGameButton_OnClick()
        {
            _trainingHelper = new SoldierTrainingHelper();
            GenerateNewGame();
            SceneManager.LoadScene("GalaxyView");
        }

        public void LoadGameButton_OnClick()
        {
            // TODO: open file screen
            // load that file
            GameSettings.Galaxy = new Galaxy(GameSettings.GalaxySize);
            var shipTemplateMap = GameSettings.Galaxy.Factions.SelectMany(f => f.ShipTemplates.Values)
                                                              .ToDictionary(s => s.Id);
            var unitTemplateMap = GameSettings.Galaxy.Factions.SelectMany(f => f.UnitTemplates.Values)
                                                              .ToDictionary(u => u.Id);
            var squadTemplateMap = GameSettings.Galaxy.Factions.SelectMany(f => f.SquadTemplates.Values)
                                                              .ToDictionary(s => s.Id);
            var gameData = 
                GameStateDataAccess.Instance.GetData("default.s3db", 
                                                     GameSettings.Galaxy.Factions.ToDictionary(f => f.Id), 
                                                     shipTemplateMap, unitTemplateMap, squadTemplateMap);

            GameSettings.Galaxy.GenerateGalaxy(gameData.Planets, gameData.Fleets);
            var factionUnits = gameData.Units.GroupBy(u => u.UnitTemplate.Faction)
                                             .ToDictionary(g => g.Key, g => g.ToList());
            foreach(Faction faction in GameSettings.Galaxy.Factions)
            {
                if(factionUnits.ContainsKey(faction))
                {
                    faction.Units.AddRange(factionUnits[faction]);
                }
            }
            SceneManager.LoadScene("GalaxyView");
        }

        public void QuitGameButton_OnClick()
        {
            Application.Quit();
        }

        private void GenerateNewGame()
        {
            GameSettings.Galaxy = new Galaxy(GameSettings.GalaxySize);
            GameSettings.Galaxy.GenerateGalaxy(1);
            // generate chapter
            CreateChapter();
            PlacePlayerForces();
        }

        private void CreateChapter()
        {
            Date basicTrainingEndDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 3, 52);
            Date trainingStartDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 4, 1);
            var soldierTemplate = GameSettings.Galaxy.PlayerFaction.SoldierTemplates[0];
            var soldiers = 
                SoldierFactory.Instance.GenerateNewSoldiers(1000, soldierTemplate)
                .Select(s => new PlayerSoldier(s, $"{TempNameGenerator.GetName()} {TempNameGenerator.GetName()}"));
            foreach (PlayerSoldier soldier in GameSettings.Chapter.PlayerSoldierMap.Values)
            {
                soldier.AddEntryToHistory(trainingStartDate + ": accepted into training");
                if (soldier.PsychicPower > 0)
                {
                    soldier.AddEntryToHistory(trainingStartDate + ": psychic ability detected, acolyte training initiated");
                    // add psychic specific training here
                }
                _trainingHelper.EvaluateSoldier(soldier, basicTrainingEndDate);
                //soldier.ProgenoidImplantDate = new Date(GameSettings.Date.Millenium, GameSettings.Date.Year - 1, RNG.GetIntBelowMax(1, 53));
            }
            GameSettings.Chapter =
                NewChapterBuilder.CreateChapter(soldiers,
                                                GameSettings.Galaxy.PlayerFaction,
                                                new Date(GameSettings.Date.Millenium,
                                                    (GameSettings.Date.Year), 1).ToString());

            // TODO: replace this with a random assignment of starting planet
            // and then have the galaxy map screen default to zooming in
            // on the Marine starting planet
            GameSettings.ChapterPlanetId = 11;
        }

        private void PlacePlayerForces()
        {
            // For now, put the chapter on their home planet
            foreach (Planet planet in GameSettings.Galaxy.Planets)
            {
                if (planet.Id == GameSettings.ChapterPlanetId)
                {
                    planet.FactionSquadListMap = new Dictionary<int, List<Squad>>
                    {
                        [GameSettings.Galaxy.PlayerFaction.Id] =
                            GameSettings.Chapter.SquadMap.Values.ToList()
                    };
                    SetChapterSquadsLocation(planet);
                    GameSettings.Chapter.Fleets[0].Planet = planet;
                    GameSettings.Chapter.Fleets[0].Position = planet.Position;
                    GameSettings.Galaxy.AddNewFleet(GameSettings.Chapter.Fleets[0]);
                }
                else if (planet.ControllingFaction != null)
                {
                    int potentialArmies = planet.ControllingFaction.UnitTemplates
                                                .Values
                                                .Where(ut => ut.IsTopLevelUnit)
                                                .Count();
                    planet.FactionSquadListMap = new Dictionary<int, List<Squad>>
                    {
                        // TODO: generalize this
                        [planet.ControllingFaction.Id] = 
                            TempTyranidArmyGenerator.GenerateTyranidArmy(
                                RNG.GetIntBelowMax(0,potentialArmies),
                                planet.ControllingFaction)
                            .GetAllSquads().ToList()
                    };
                }
            }
        }

        private void SetChapterSquadsLocation(Planet planet)
        {
            if (GameSettings.Chapter.OrderOfBattle.HQSquad != null)
            {
                GameSettings.Chapter.OrderOfBattle.HQSquad.Location = planet;
            }
            foreach (Squad squad in GameSettings.Chapter.OrderOfBattle.Squads)
            {
                squad.Location = planet;
            }
            foreach (Unit unit in GameSettings.Chapter.OrderOfBattle.ChildUnits)
            {
                if (unit.HQSquad != null)
                {
                    unit.HQSquad.Location = planet;
                }
                foreach (Squad squad in unit.Squads)
                {
                    squad.Location = planet;
                }
            }
        }
    }
}