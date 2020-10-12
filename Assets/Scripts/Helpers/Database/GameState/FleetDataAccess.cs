using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Fleets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Database.GameState
{
    public class FleetDataAccess
    {
        public Dictionary<int, List<Ship>> GetShipsByFleetId(IDbConnection connection,
                                                             IReadOnlyDictionary<int, ShipTemplate> shipTemplateMap)
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

        public List<Fleet> GetFleetsByFactionId(IDbConnection connection,
                                                IReadOnlyDictionary<int, List<Ship>> fleetShipMap,
                                                IReadOnlyDictionary<int, Faction> factionMap,
                                                IReadOnlyList<Planet> planetList)
        {
            List<Fleet> fleetList = new List<Fleet>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Fleet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                float x = (float)reader[2];
                float y = (float)reader[3];

                Planet destination;
                if(reader[4].GetType() != typeof(DBNull))
                {
                    destination = planetList.First(p => p.Id == reader.GetInt32(4));
                }
                else
                {
                    destination = null;
                }

                // see if the position is a planet
                Vector2 location = new Vector2(x, y);
                Planet planet = planetList.FirstOrDefault(p => p.Position == location);

                Fleet fleet = new Fleet(id, factionMap[factionId], location, planet,
                                        destination, fleetShipMap[id]);
                fleetList.Add(fleet);
            }
            return fleetList;
        }

        public void SaveFleet(IDbTransaction transaction, Fleet fleet)
        {
            string destination = fleet.Destination == null ? "null" : fleet.Destination.Id.ToString();
            string insert = $@"INSERT INTO Fleet VALUES ({fleet.Id}, {fleet.Faction.Id}, 
                {fleet.Position.x}, {fleet.Position.y}, {destination});";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();
        }

        public void SaveShip(IDbTransaction transaction, Ship ship)
        {
            string insert = $@"INSERT INTO Ship VALUES ({ship.Id}, {ship.Template.Id}, 
                {ship.Fleet.Id}, '{ship.Name}');";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();
        }
    }
}
