using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Fleets;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;

namespace OnlyWar.Scripts.Helpers.Database
{
    public class GameStateDataAccess
    {
        private readonly PlanetDataAccess _planetDataAccess;
        private readonly FleetDataAccess _fleetDataAccess;
        private readonly UnitDataAccess _unitDataAccess;
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
        }

        public void GetData(string fileName, Dictionary<int, Faction> factionMap,
                            Dictionary<int, ShipTemplate> shipTemplateMap,
                            Dictionary<int, UnitTemplate> unitTemplateMap,
                            List<SquadTemplate> squadTemplates,
                            List<Planet> planetList)
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            var planets = _planetDataAccess.GetPlanets(dbCon, factionMap);
            var ships = _fleetDataAccess.GetShipsByFleetId(dbCon, shipTemplateMap);
            var shipMap = ships.Values.SelectMany(s => s).ToDictionary(ship => ship.Id);
            var fleets = _fleetDataAccess.GetFleetsByFactionId(dbCon, ships, factionMap, planets);
            var squads = _unitDataAccess.GetSquadsByUnitId(dbCon, squadTemplates, shipMap, planetList);
            var units = _unitDataAccess.GetUnits(dbCon, unitTemplateMap, squads);
            dbCon.Close();
        }

        public void SaveData(string fileName, List<Planet> planets, List<Fleet> fleets, List<Ship> ships,
                             List<Unit> units, List<Squad> squads)
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            using (var transaction = dbCon.BeginTransaction())
            {
                try
                {
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
                    }

                    foreach(Squad squad in squads)
                    {
                        _unitDataAccess.SaveSquad(transaction, squad);
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
                transaction.Commit();
            }
        }
    }
}