using OnlyWar.Models;
using OnlyWar.Models.Planets;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace OnlyWar.Helpers.Database.GameState
{
    public class PlanetDataAccess
    {
        public List<Planet> GetPlanets(IDbConnection connection,
                                       IReadOnlyDictionary<int, Faction> factionMap,
                                       IReadOnlyDictionary<int, Character> characterMap,
                                       IReadOnlyDictionary<int, PlanetTemplate> planetTemplateMap)
        {
            Dictionary<int, List<PlanetFaction>> planetFactions =
                GetPlanetFactions(connection, factionMap, characterMap);
            List<Planet> planetList = new List<Planet>();
            using (var command = connection.CreateCommand())
            {
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
                    bool isUnderAssault = reader.GetBoolean(8);
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
                    // for now, we're hard coding all planets to be size 10
                    Planet planet =
                        new Planet(id, name, new Vector2(x, y), 10, template, importance, taxLevel)
                        {
                            ControllingFaction = controllingFaction,
                            IsUnderAssault = isUnderAssault
                        };
                    foreach (PlanetFaction planetFaction in planetFactions[id])
                    {
                        planet.PlanetFactionMap.Add(planetFaction.Faction.Id, planetFaction);
                    }
                    planetList.Add(planet);
                }
            }
            return planetList;
        }

        private Dictionary<int, List<PlanetFaction>> GetPlanetFactions(IDbConnection connection,
                                                                       IReadOnlyDictionary<int, Faction> factionMap,
                                                                       IReadOnlyDictionary<int, Character> characterMap)
        {
            Dictionary<int, List<PlanetFaction>> planetPlanetFactionMap = new Dictionary<int, List<PlanetFaction>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlanetFaction";
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int? leaderId = null;
                    int planetId = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);
                    bool isPublic = reader.GetBoolean(2);
                    int population = reader.GetInt32(3);
                    int pdfMembers = reader.GetInt32(4);
                    int planetaryControl = reader.GetInt32(5);
                    float playerReputation = (float)reader[6];
                    if (reader[7].GetType() != typeof(DBNull))
                    {
                        leaderId = reader.GetInt32(7);
                    }
                    PlanetFaction planetFaction =
                        new PlanetFaction(factionMap[factionId])
                        {
                            IsPublic = isPublic,
                            Population = population,
                            PDFMembers = pdfMembers,
                            PlanetaryControl = planetaryControl,
                            PlayerReputation = playerReputation,
                            Leader = leaderId == null ? null : characterMap[(int)leaderId]
                        };

                    if (!planetPlanetFactionMap.ContainsKey(planetId))
                    {
                        planetPlanetFactionMap[planetId] = new List<PlanetFaction>();
                    }
                    planetPlanetFactionMap[planetId].Add(planetFaction);
                }
            }
            return planetPlanetFactionMap;
        }

        public Dictionary<int, Character> GetCharacterMap(IDbConnection connection, 
                                                           IReadOnlyDictionary<int, Faction> factionMap)
        {
            Dictionary<int, Character> characterMap = new Dictionary<int, Character>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Character";
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    float investigation = (float)reader[1];
                    float paranoia = (float)reader[2];
                    float neediness = (float)reader[3];
                    float patience = (float)reader[4];
                    float appreciation = (float)reader[5];
                    float influence = (float)reader[6];
                    int factionId = reader.GetInt32(7);
                    float opinionOfPlayer = (float)reader[8];

                    Character character = new Character()
                    {
                        Id = id,
                        Appreciation = appreciation,
                        Influence = influence,
                        Investigation = investigation,
                        Loyalty = factionMap[factionId],
                        Neediness = neediness,
                        OpinionOfPlayerForce = opinionOfPlayer,
                        Paranoia = paranoia,
                        Patience = patience
                    };

                    characterMap[id] = character;
                }
            }
            return characterMap;
        }

        public void SavePlanet(IDbTransaction transaction, Planet planet)
        {
            string controllingFactionId = planet.ControllingFaction == null ?
                "null" : planet.ControllingFaction.Id.ToString();

            string insert = $@"INSERT INTO Planet 
                (Id, PlanetTemplateId, Name, x, y, FactionId, 
                Importance, TaxLevel, IsUnderAssault) VALUES 
                ({planet.Id}, {planet.Template.Id}, '{planet.Name.Replace("\'", "\'\'")}', 
                {planet.Position.x}, {planet.Position.y}, {controllingFactionId},
                {planet.Importance}, {planet.TaxLevel}, {planet.IsUnderAssault});";
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
            SavePlanetFactions(transaction, planet.Id, planet.PlanetFactionMap);
        }

        public void SaveCharacter(IDbTransaction transaction, Character character)
        {
            string insert = $@"INSERT INTO Character 
                (Id, Investigation, Paranoia, Neediness, Patience, Appreciation, 
                Influence, LoyalFactionId, OpinionOfPlayer) VALUES 
                ({character.Id}, {character.Investigation}, '{character.Paranoia}', 
                {character.Neediness}, {character.Patience}, {character.Appreciation},
                {character.Influence}, {character.Loyalty.Id}, {character.OpinionOfPlayerForce});";
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
        }

        private void SavePlanetFactions(IDbTransaction transaction, int planetId, Dictionary<int, PlanetFaction> planetFactions)
        {
            foreach(KeyValuePair<int, PlanetFaction> planetFaction in planetFactions)
            {
                object leaderId = planetFaction.Value.Leader != null ?
                    (object)planetFaction.Value.Leader.Id : 
                    "null";
                string insert = $@"INSERT INTO PlanetFaction 
                    (PlanetId, FactionId, IsPublic, Population, 
                    PDFMembers, PlanetaryControl, PlayerReputation, LeaderId) VALUES 
                    ({planetId}, {planetFaction.Key}, {planetFaction.Value.IsPublic}, 
                    {planetFaction.Value.Population}, {planetFaction.Value.PDFMembers}, 
                    {planetFaction.Value.PlanetaryControl}, 
                    {planetFaction.Value.PlayerReputation}, {leaderId});";
                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = insert;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
