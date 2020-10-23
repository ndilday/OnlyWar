using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Planets;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Database.GameState
{
    public class PlanetDataAccess
    {
        public List<Planet> GetPlanets(IDbConnection connection,
                                       IReadOnlyDictionary<int, Faction> factionMap,
                                       IReadOnlyDictionary<int, PlanetTemplate> planetTemplateMap)
        {
            List<Planet> planetList = new List<Planet>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Planet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int planetTemplateId = reader.GetInt32(1);
                string name = reader[2].ToString();
                int x = reader.GetInt32(3);
                int y = reader.GetInt32(4);
                int population = reader.GetInt32(6);
                int importance = reader.GetInt32(7);
                int taxLevel = reader.GetInt32(8);
                var template = planetTemplateMap[planetTemplateId];
                Faction controllingFaction;
                if (reader[5].GetType() != typeof(DBNull))
                {
                    controllingFaction = factionMap[reader.GetInt32(5)];
                }
                else
                {
                    controllingFaction = null;
                }
                Planet planet = 
                    new Planet(id, name, new Vector2(x, y), template, importance, taxLevel)
                {
                    ControllingFaction = controllingFaction,
                    ImperialPopulation = population
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
                (Id, PlanetTemplateId, Name, x, y, FactionId, Population, Importance, TaxLevel) VALUES 
                ({planet.Id}, {planet.Template.Id}, '{planet.Name.Replace("\'", "\'\'")}', 
                {planet.Position.x}, {planet.Position.y}, {controllingFactionId},
                {planet.ImperialPopulation}, {planet.Importance}, {planet.TaxLevel});";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();
        }
    }
}
