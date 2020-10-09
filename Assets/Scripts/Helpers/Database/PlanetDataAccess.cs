using Mono.Data.Sqlite;
using OnlyWar.Scripts.Models;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Database
{
    public class PlanetDataAccess
    {
        public List<Planet> GetPlanets(IDbConnection connection,
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

        public void SavePlanet(IDbTransaction transaction, Planet planet)
        {
            string insert = $@"INSERT INTO Planet VALUES ({planet.Id}, 
                {planet.Name}, {planet.Position.x}, {planet.Position.y},
                {planet.PlanetType}, {planet.ControllingFaction.Id});";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();
        }
    }
}
