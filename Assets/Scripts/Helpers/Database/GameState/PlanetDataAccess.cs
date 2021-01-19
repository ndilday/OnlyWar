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
            Dictionary<int, List<PlanetFaction>> planetFactions =
                GetPlanetFactions(connection, factionMap);
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
                int importance = reader.GetInt32(6);
                int taxLevel = reader.GetInt32(7);
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
                        ControllingFaction = controllingFaction
                    };
                foreach (PlanetFaction planetFaction in planetFactions[id])
                {
                    planet.PlanetFactionMap.Add(planetFaction.Faction.Id, planetFaction);
                }
                planetList.Add(planet);
            }
            return planetList;
        }

        private Dictionary<int, List<PlanetFaction>> GetPlanetFactions(IDbConnection connection,
                                                                 IReadOnlyDictionary<int, Faction> factionMap)
        {
            Dictionary<int, List<PlanetFaction>> planetPlanetFactionMap = new Dictionary<int, List<PlanetFaction>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM PlanetFaction";
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int planetId = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                bool isPublic = reader.GetBoolean(2);
                int population = reader.GetInt32(3);
                int pdfMembers = reader.GetInt32(4);
                float playerReputation = (float)reader[5];

                PlanetFaction planetFaction =
                    new PlanetFaction(factionMap[factionId])
                    {
                        IsPublic = isPublic,
                        Population = population,
                        PDFMembers = pdfMembers,
                        PlayerReputation = playerReputation
                    };

                if (!planetPlanetFactionMap.ContainsKey(planetId))
                {
                    planetPlanetFactionMap[planetId] = new List<PlanetFaction>();
                }
                planetPlanetFactionMap[planetId].Add(planetFaction);
            }
            return planetPlanetFactionMap;
        }

        public void SavePlanet(IDbTransaction transaction, Planet planet)
        {
            string controllingFactionId = planet.ControllingFaction == null ?
                "null" : planet.ControllingFaction.Id.ToString();

            string insert = $@"INSERT INTO Planet 
                (Id, PlanetTemplateId, Name, x, y, FactionId, Importance, TaxLevel) VALUES 
                ({planet.Id}, {planet.Template.Id}, '{planet.Name.Replace("\'", "\'\'")}', 
                {planet.Position.x}, {planet.Position.y}, {controllingFactionId},
                {planet.Importance}, {planet.TaxLevel});";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();
            SavePlanetFactions(transaction, planet.Id, planet.PlanetFactionMap);
        }

        private void SavePlanetFactions(IDbTransaction transaction, int planetId, Dictionary<int, PlanetFaction> planetFactions)
        {
            foreach(KeyValuePair<int, PlanetFaction> planetFaction in planetFactions)
            {
                string insert = $@"INSERT INTO PlanetFaction 
                    (PlanetId, FactionId, IsPublic, Population, PDFMembers, PlayerReputation) VALUES 
                    ({planetId}, {planetFaction.Key}, {planetFaction.Value.IsPublic}, {planetFaction.Value.Population}, 
                    {planetFaction.Value.PDFMembers}, {planetFaction.Value.PlayerReputation});";
                IDbCommand command = transaction.Connection.CreateCommand();
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
        }
    }
}
