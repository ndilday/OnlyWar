using OnlyWar.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Database.GameState
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

                Faction controllingFaction;
                if (reader[5].GetType() != typeof(DBNull))
                {
                    controllingFaction = factionMap[reader.GetInt32(5)];
                }
                else
                {
                    controllingFaction = null;
                }
                Planet planet = new Planet(id, name, new Vector2(x, y), planetType)
                {
                    ControllingFaction = controllingFaction
                };

                planetList.Add(planet);
            }
            return planetList;
        }

        public void SavePlanet(IDbTransaction transaction, Planet planet)
        {
            string controllingFactionId = planet.ControllingFaction == null ?
                "null" : planet.ControllingFaction.Id.ToString();

            string insert = $@"INSERT INTO Planet 
                (Id, Name, x, y, PlanetType, FactionId) VALUES 
                ({planet.Id}, '{planet.Name.Replace("\'", "\'\'")}', {planet.Position.x}, 
                {planet.Position.y}, {(int)planet.PlanetType}, {controllingFactionId});";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();
        }
    }
}
