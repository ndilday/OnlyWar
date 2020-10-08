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
        public void GetData(string fileName, Dictionary<int, Faction> factionMap,
                            Dictionary<int, ShipTemplate> shipTemplateMap,
                            Dictionary<int, UnitTemplate> unitTemplateMap,
                            List<SquadTemplate> squadTemplates,
                            List<Planet> planetList)
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            var planets = GetPlanets(dbCon, factionMap);
            var ships = GetShipsByFleetId(dbCon, shipTemplateMap);
            var shipMap = ships.Values.SelectMany(s => s).ToDictionary(ship => ship.Id);
            var fleets = GetFleetsByFactionId(dbCon, ships, factionMap, planets);
            var squads = GetSquadsByUnitId(dbCon, squadTemplates, shipMap, planetList);
            var units = GetUnits(dbCon, unitTemplateMap, squads);
            dbCon.Close();
        }

        private List<Planet> GetPlanets(IDbConnection connection,
                                        Dictionary<int, Faction> factionMap)
        {
            List<Planet> planetList = new List<Planet>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Planet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader[1].ToString();
                int x = reader.GetInt32(2);
                int y = reader.GetInt32(3);
                PlanetType planetType = (PlanetType)reader.GetInt32(4);
                int factionId = reader.GetInt32(5);
                Planet planet = new Planet(id, name, new Vector2(x, y), planetType)
                {
                    ControllingFaction = factionMap[factionId]
                };

                planetList.Add(planet);
            }
            return planetList;
        }
    
        private Dictionary<int, List<Ship>> GetShipsByFleetId(IDbConnection connection,
                                                              Dictionary<int, ShipTemplate> shipTemplateMap)
        {
            Dictionary<int, List<Ship>> fleetShipMap = new Dictionary<int, List<Ship>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Ship";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int shipTemplateId = reader.GetInt32(1);
                int fleetId = reader.GetInt32(2);
                string name = reader[3].ToString();

                Ship ship = new Ship(id, name, shipTemplateMap[shipTemplateId]);

                if (!fleetShipMap.ContainsKey(fleetId))
                {
                    fleetShipMap[fleetId] = new List<Ship>();
                }
                fleetShipMap[fleetId].Add(ship);
            }
            return fleetShipMap;
        }

        private List<Fleet> GetFleetsByFactionId(IDbConnection connection,
                                                 Dictionary<int, List<Ship>> fleetShipMap,
                                                 Dictionary<int, Faction> factionMap,
                                                 List<Planet> planetList)
        {
            List<Fleet> fleetList = new List<Fleet>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Fleet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                int fleetTemplateId = reader.GetInt32(2);
                int x = reader.GetInt32(3);
                int y = reader.GetInt32(4);
                int destinationPlanetId = reader.GetInt32(5);

                // see if the position is a planet
                Vector2 location = new Vector2(x, y);
                Planet planet = planetList.FirstOrDefault(p => p.Position == location);
                Planet destination = planetList.First(p => p.Id == destinationPlanetId);

                Fleet fleet = new Fleet(id, factionMap[factionId], location, planet,
                                        destination, fleetShipMap[id]);
                fleetList.Add(fleet);
            }
            return fleetList;
        }
    
        private Dictionary<int, List<Squad>> GetSquadsByUnitId(IDbConnection connection,
                                                               List<SquadTemplate> squadTemplates,
                                                               Dictionary<int, Ship> shipMap,
                                                               List<Planet> planetList)
        {
            Dictionary<int, List<Squad>> squadMap = new Dictionary<int, List<Squad>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Squad";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int squadTemplateId = reader.GetInt32(1);
                int parentUnitId = reader.GetInt32(2);
                string name = reader[3].ToString();

                SquadTemplate template = squadTemplates.First(st => st.Id == squadTemplateId);

                Squad squad = new Squad(id, name, null, template);


                if (reader[4].GetType() != typeof(DBNull))
                {
                    Ship ship = shipMap[reader.GetInt32(4)];
                    ship.LoadSquad(squad);
                    squad.BoardedLocation = ship;
                }

                if (reader[5].GetType() != typeof(DBNull))
                {
                    Planet planet = planetList.First(p => p.Id == reader.GetInt32(5));
                    squad.Location = planet;
                }

                if(!squadMap.ContainsKey(parentUnitId))
                {
                    squadMap[parentUnitId] = new List<Squad>();
                }
                squadMap[parentUnitId].Add(squad);
            }
            return squadMap;
        }

        private List<Unit> GetUnits(IDbConnection connection,
                                    Dictionary<int, UnitTemplate> unitTemplateMap,
                                    Dictionary<int, List<Squad>> unitSquadMap)
        {
            List<Unit> unitList = new List<Unit>();
            Dictionary<int, Unit> unitMap = new Dictionary<int, Unit>();
            Dictionary<int, List<Unit>> parentUnitMap = new Dictionary<int, List<Unit>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Unit";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                int unitTemplateId = reader.GetInt32(2);
                string name = reader[5].ToString();

                Squad hqSquad = null;
                int parentUnitId;

                List<Squad> squadList = null;
                if (unitSquadMap.ContainsKey(id))
                {
                    squadList = unitSquadMap[id];
                }

                if (reader[3].GetType() != typeof(DBNull))
                {
                    int hqSquadId = reader.GetInt32(3);
                    hqSquad = squadList.First(s => s.Id == hqSquadId);
                }

                Unit unit = new Unit(id, name, unitTemplateMap[unitTemplateId],
                                     hqSquad, squadList);
                if(hqSquad != null)
                {
                    hqSquad.ParentUnit = unit;
                }
                foreach(Squad squad in squadList)
                {
                    squad.ParentUnit = unit;
                }

                unitMap[id] = unit;
                unitList.Add(unit);

                if (reader[4].GetType() != typeof(DBNull))
                {
                    parentUnitId = reader.GetInt32(4);
                    if (!parentUnitMap.ContainsKey(parentUnitId))
                    {
                        parentUnitMap[parentUnitId] = new List<Unit>();
                    }
                    parentUnitMap[parentUnitId].Add(unit);
                }
            }

            foreach(KeyValuePair<int, List<Unit>> kvp in parentUnitMap)
            {
                unitMap[kvp.Key].ChildUnits = kvp.Value;
            }

            return unitList;
        }
    }
}