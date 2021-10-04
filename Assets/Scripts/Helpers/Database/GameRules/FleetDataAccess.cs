using OnlyWar.Models.Fleets;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlyWar.Helpers.Database.GameRules
{
    public class FleetDataBlob
    {
        public Dictionary<int, List<BoatTemplate>> BoatTemplates { get; set; }
        public Dictionary<int, List<ShipTemplate>> ShipTemplates { get; set; }
        public Dictionary<int, List<FleetTemplate>> FleetTemplates { get; set; }
    }

    public class FleetDataAccess
    {
        public FleetDataBlob GetFleetData(IDbConnection connection)
        {
            FleetDataBlob dataBlob = new FleetDataBlob();
            dataBlob.BoatTemplates = GetBoatTemplatesByFactionId(connection);
            dataBlob.ShipTemplates = GetShipTemplatesByFactionId(connection);
            var fleetShipMap = GetFleetShipTemplateLists(connection);
            dataBlob.FleetTemplates = GetFleetTemplatesByFactionId(connection, 
                                                                   dataBlob.ShipTemplates, 
                                                                   fleetShipMap);
            return dataBlob;
        }

        private Dictionary<int, List<BoatTemplate>> GetBoatTemplatesByFactionId(IDbConnection connection)
        {
            Dictionary<int, List<BoatTemplate>> factionTemplateMap =
                new Dictionary<int, List<BoatTemplate>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM BoatTemplate";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);
                    string name = reader[2].ToString();
                    ushort soldierCap = (ushort)reader.GetInt16(3);
                    BoatTemplate boatTemplate = new BoatTemplate(id, name, soldierCap);
                    if (!factionTemplateMap.ContainsKey(factionId))
                    {
                        factionTemplateMap[factionId] = new List<BoatTemplate>();
                    }
                    factionTemplateMap[factionId].Add(boatTemplate);
                }
            }
            return factionTemplateMap;
        }

        private Dictionary<int, List<ShipTemplate>> GetShipTemplatesByFactionId(IDbConnection connection)
        {
            Dictionary<int, List<ShipTemplate>> factionTemplateMap =
                new Dictionary<int, List<ShipTemplate>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM ShipTemplate";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);
                    string name = reader[2].ToString();
                    ushort soldierCap = (ushort)reader.GetInt16(3);
                    ushort boatCap = (ushort)reader.GetInt16(4);
                    ushort landerCap = (ushort)reader.GetInt16(5);
                    ShipTemplate boatTemplate = new ShipTemplate(id, name, soldierCap, boatCap, landerCap);
                    if (!factionTemplateMap.ContainsKey(factionId))
                    {
                        factionTemplateMap[factionId] = new List<ShipTemplate>();
                    }
                    factionTemplateMap[factionId].Add(boatTemplate);
                }
            }
            return factionTemplateMap;
        }

        private Dictionary<int, List<int>> GetFleetShipTemplateLists(IDbConnection connection)
        {
            Dictionary<int, List<int>> fleetToShipMap = new Dictionary<int, List<int>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM FleetTemplateShipTemplate";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int fleetId = reader.GetInt32(1);
                    int shipId = reader.GetInt32(2);
                    if (!fleetToShipMap.ContainsKey(fleetId))
                    {
                        fleetToShipMap[fleetId] = new List<int>();
                    }
                    fleetToShipMap[fleetId].Add(shipId);
                }
            }
            return fleetToShipMap;
        }

        private Dictionary<int, List<FleetTemplate>> GetFleetTemplatesByFactionId(IDbConnection connection,
                                                                                  Dictionary<int, List<ShipTemplate>> factionShipMap,
                                                                                  Dictionary<int, List<int>> fleetShipMap)
        {
            Dictionary<int, List<FleetTemplate>> factionTemplateMap =
                new Dictionary<int, List<FleetTemplate>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM FleetTemplate";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);
                    string name = reader[2].ToString();

                    List<ShipTemplate> baseList = factionShipMap[factionId];
                    List<ShipTemplate> fleetShipTemplateList = new List<ShipTemplate>();
                    foreach (int shipTemplateId in fleetShipMap[id])
                    {
                        fleetShipTemplateList.Add(baseList.First(st => st.Id == shipTemplateId));
                    }

                    FleetTemplate fleetTemplate = new FleetTemplate(id, name, fleetShipTemplateList);
                    if (!factionTemplateMap.ContainsKey(factionId))
                    {
                        factionTemplateMap[factionId] = new List<FleetTemplate>();
                    }
                    factionTemplateMap[factionId].Add(fleetTemplate);
                }
            }
            return factionTemplateMap;
        }
    }
}
