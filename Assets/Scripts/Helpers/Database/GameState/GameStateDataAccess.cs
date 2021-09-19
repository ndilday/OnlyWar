using Mono.Data.Sqlite;
using OnlyWar.Models;
using OnlyWar.Models.Equippables;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using OnlyWar.Models.Soldiers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Helpers.Database.GameState
{
    public class GameStateDataBlob
    {
        public List<Character> Characters { get; set; }
        public List<Planet> Planets { get; set; }
        public List<Fleet> Fleets { get; set; }
        public List<Unit> Units { get; set; }
        public Date CurrentDate { get; set; }
        public Dictionary<Date, List<EventHistory>> History { get; set; }
    }

    public class GameStateDataAccess
    {
        private readonly PlanetDataAccess _planetDataAccess;
        private readonly FleetDataAccess _fleetDataAccess;
        private readonly UnitDataAccess _unitDataAccess;
        private readonly SoldierDataAccess _soldierDataAccess;
        private readonly PlayerSoldierDataAccess _playerSoldierDataAccess;
        private readonly GlobalDataAccess _globalDataAccess;
        private readonly PlayerFactionEventDataAccess _playerFactionEventDataAccess;
        private readonly string CREATE_TABLE_FILE =
            $"{Application.streamingAssetsPath}/GameData/SaveStructure.sql";
        private static GameStateDataAccess _instance;
        public static GameStateDataAccess Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GameStateDataAccess();
                }
                return _instance;
            }
        }
        
        private GameStateDataAccess()
        {
            _planetDataAccess = new PlanetDataAccess();
            _fleetDataAccess = new FleetDataAccess();
            _unitDataAccess = new UnitDataAccess();
            _soldierDataAccess = new SoldierDataAccess();
            _playerSoldierDataAccess = new PlayerSoldierDataAccess();
            _globalDataAccess = new GlobalDataAccess();
            _playerFactionEventDataAccess = new PlayerFactionEventDataAccess();
        }

        public GameStateDataBlob GetData(string fileName, Dictionary<int, Faction> factionMap,
                            IReadOnlyDictionary<int, PlanetTemplate> planetTemplateMap,
                            IReadOnlyDictionary<int, ShipTemplate> shipTemplateMap,
                            IReadOnlyDictionary<int, UnitTemplate> unitTemplateMap,
                            IReadOnlyDictionary<int, SquadTemplate> squadTemplates,
                            IReadOnlyDictionary<int, WeaponSet> weaponSets,
                            IReadOnlyDictionary<int, HitLocationTemplate> hitLocationTemplates,
                            IReadOnlyDictionary<int, BaseSkill> baseSkillMap, 
                            IReadOnlyDictionary<int, SoldierTemplate> soldierTemplateMap)
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            var characterMap = _planetDataAccess.GetCharacterMap(dbCon, factionMap);
            var planets = _planetDataAccess.GetPlanets(dbCon, factionMap, characterMap, 
                                                       planetTemplateMap);
            var ships = _fleetDataAccess.GetShipsByFleetId(dbCon, shipTemplateMap);
            var shipMap = ships.Values.SelectMany(s => s).ToDictionary(ship => ship.Id);
            var fleets = _fleetDataAccess.GetFleetsByFactionId(dbCon, ships, factionMap, planets);
            var loadouts = _unitDataAccess.GetSquadWeaponSets(dbCon, weaponSets);
            var squads = _unitDataAccess.GetSquadsByUnitId(dbCon, squadTemplates, loadouts, 
                                                           shipMap, planets);
            var units = _unitDataAccess.GetUnits(dbCon, unitTemplateMap, squads);
            var squadMap = squads.Values.SelectMany(s => s).ToDictionary(s => s.Id);
            var soldiers = _soldierDataAccess.GetData(dbCon, hitLocationTemplates, baseSkillMap,
                                                      soldierTemplateMap, squadMap);
            SoldierFactory.Instance.SetCurrentHighestSoldierId(soldiers.Keys.Max());
            var playerSoldiers = _playerSoldierDataAccess.GetData(dbCon, soldiers);
            var date = _globalDataAccess.GetGlobalData(dbCon);
            var history = _playerFactionEventDataAccess.GetHistory(dbCon);
            dbCon.Close();
            return new GameStateDataBlob
            {
                Characters = characterMap.Values.ToList(),
                Planets = planets,
                Fleets = fleets,
                Units = units,
                CurrentDate = date,
                History = history
            };
        }

        public void SaveData(string fileName, 
                             Date currentDate,
                             IEnumerable<Character> characters,
                             IEnumerable<Planet> planets, 
                             IEnumerable<Fleet> fleets,
                             IEnumerable<Unit> units,
                             IEnumerable<PlayerSoldier> playerSoldiers,
                             IReadOnlyDictionary<Date, List<EventHistory>> history)
        {
            string path = $"{Application.streamingAssetsPath}/Saves/{fileName}";
            if(File.Exists(path))
            {
                File.Delete(path);
            }
            GenerateTables(fileName);
            var squads = units.SelectMany(u => u.GetAllSquads());
            var ships = fleets.SelectMany(f => f.Ships);
            string connection = 
                $"URI=file:{path}";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            using (var transaction = dbCon.BeginTransaction())
            {
                try
                {
                    foreach(Character character in characters)
                    {
                        _planetDataAccess.SaveCharacter(transaction, character);
                    }
                    foreach (Planet planet in planets)
                    {
                        _planetDataAccess.SavePlanet(transaction, planet);
                    }

                    foreach(Fleet fleet in fleets)
                    {
                        _fleetDataAccess.SaveFleet(transaction, fleet);
                    }

                    foreach(Ship ship in ships)
                    {
                        _fleetDataAccess.SaveShip(transaction, ship);
                    }

                    foreach(Unit unit in units)
                    {
                        _unitDataAccess.SaveUnit(transaction, unit);
                        foreach(Unit childUnit in unit?.ChildUnits)
                        {
                            _unitDataAccess.SaveUnit(transaction, childUnit);
                        }
                    }

                    foreach(Squad squad in squads)
                    {
                        _unitDataAccess.SaveSquad(transaction, squad);
                        foreach (ISoldier soldier in squad.Members)
                        {
                            _soldierDataAccess.SaveSoldier(transaction, soldier);
                        }
                    }

                    foreach(PlayerSoldier playerSoldier in playerSoldiers)
                    {
                        _playerSoldierDataAccess.SavePlayerSoldier(transaction, playerSoldier);
                    }
                    _globalDataAccess.SaveDate(transaction, currentDate);
                    _playerFactionEventDataAccess.SaveData(transaction, history);
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
                transaction.Commit();
                dbCon.Close();
            }
        }

        private void GenerateTables(string fileName)
        {
            string cmdText = File.ReadAllText(CREATE_TABLE_FILE);
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            using (var command = dbCon.CreateCommand())
            {
                command.CommandText = cmdText;
                command.ExecuteNonQuery();
                dbCon.Close();
            }
        }
    }
}