using OnlyWar.Models.Equippables;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlyWar.Helpers.Database.GameState
{
    public class UnitDataAccess
    {
        public Dictionary<int, List<Squad>> GetSquadsByUnitId(IDbConnection connection,
                                                               IReadOnlyDictionary<int, SquadTemplate> squadTemplateMap,
                                                               IReadOnlyDictionary<int, List<WeaponSet>> squadWeaponSetMap,
                                                               IReadOnlyDictionary<int, Ship> shipMap,
                                                               List<Planet> planetList)
        {
            Dictionary<int, List<Squad>> squadMap = new Dictionary<int, List<Squad>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Squad";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int squadTemplateId = reader.GetInt32(1);
                    int parentUnitId = reader.GetInt32(2);
                    string name = reader[3].ToString();
                    bool isInReserve = reader.GetBoolean(6);

                    SquadTemplate template = squadTemplateMap[squadTemplateId];

                    Squad squad = new Squad(id, name, null, template, isInReserve);


                    if (reader[4].GetType() != typeof(DBNull))
                    {
                        Ship ship = shipMap[reader.GetInt32(4)];
                        squad.BoardedLocation = ship;
                    }

                    if (reader[5].GetType() != typeof(DBNull))
                    {
                        Planet planet = planetList.First(p => p.Id == reader.GetInt32(5));
                        squad.Location = planet;
                    }

                    if (!squadMap.ContainsKey(parentUnitId))
                    {
                        squadMap[parentUnitId] = new List<Squad>();
                    }

                    if (squadWeaponSetMap.ContainsKey(id))
                    {
                        squad.Loadout = squadWeaponSetMap[id];
                    }

                    squadMap[parentUnitId].Add(squad);
                }
            }
            return squadMap;
        }

        public List<Unit> GetUnits(IDbConnection connection,
                                   IReadOnlyDictionary<int, UnitTemplate> unitTemplateMap,
                                   IReadOnlyDictionary<int, List<Squad>> unitSquadMap)
        {
            List<Unit> unitList = new List<Unit>();
            Dictionary<int, Unit> unitMap = new Dictionary<int, Unit>();
            Dictionary<int, List<Unit>> parentUnitMap = new Dictionary<int, List<Unit>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Unit";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int unitTemplateId = reader.GetInt32(2);
                    string name = reader[4].ToString();

                    Squad hqSquad = null;
                    int parentUnitId;

                    List<Squad> squadList = null;
                    if (unitSquadMap.ContainsKey(id))
                    {
                        squadList = unitSquadMap[id];
                    }

                    Unit unit = new Unit(id, name, unitTemplateMap[unitTemplateId], squadList);
                    if (hqSquad != null)
                    {
                        hqSquad.ParentUnit = unit;
                    }
                    foreach (Squad squad in squadList)
                    {
                        squad.ParentUnit = unit;
                    }

                    unitMap[id] = unit;
                    unitList.Add(unit);

                    if (reader[3].GetType() != typeof(DBNull))
                    {
                        parentUnitId = reader.GetInt32(3);
                        if (!parentUnitMap.ContainsKey(parentUnitId))
                        {
                            parentUnitMap[parentUnitId] = new List<Unit>();
                        }
                        parentUnitMap[parentUnitId].Add(unit);
                    }
                }
            }

            foreach (KeyValuePair<int, List<Unit>> kvp in parentUnitMap)
            {
                unitMap[kvp.Key].ChildUnits = kvp.Value;
                foreach(Unit unit in kvp.Value)
                {
                    unit.ParentUnit = unitMap[kvp.Key];
                }
            }

            return unitList.Where(u => u.ParentUnit == null).ToList();
        }

        public Dictionary<int, List<WeaponSet>> GetSquadWeaponSets(IDbConnection connection, 
                                                                   IReadOnlyDictionary<int, WeaponSet> weaponSets)
        {
            Dictionary<int, List<WeaponSet>> squadWeaponSetMap = 
                new Dictionary<int, List<WeaponSet>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM SquadWeaponSet";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int squadId = reader.GetInt32(0);
                    int weaponSetId = reader.GetInt32(1);

                    WeaponSet weaponSet = weaponSets[weaponSetId];

                    if (!squadWeaponSetMap.ContainsKey(squadId))
                    {
                        squadWeaponSetMap[squadId] = new List<WeaponSet>();
                    }
                    squadWeaponSetMap[squadId].Add(weaponSet);
                }
            }
            return squadWeaponSetMap;
        }

        public void SaveUnit(IDbTransaction transaction, Unit unit)
        {
            string parent = unit.ParentUnit == null ? "null" : unit.ParentUnit.Id.ToString();
            string insert = $@"INSERT INTO Unit VALUES ({unit.Id}, {unit.UnitTemplate.Faction.Id}, 
                {unit.UnitTemplate.Id}, {parent}, '{unit.Name}');";
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
        }

        public void SaveSquad(IDbTransaction transaction, Squad squad)
        {
            string safeName = squad.Name.Replace("\'", "\'\'");
            string ship = squad.BoardedLocation == null ? "null" : squad.BoardedLocation.Id.ToString();
            string planet = squad.Location == null ? "null" : squad.Location.Id.ToString();
            string insert = $@"INSERT INTO Squad VALUES ({squad.Id}, {squad.SquadTemplate.Id}, 
                {squad.ParentUnit.Id}, '{safeName}', {ship}, {planet}, {squad.IsInReserve});";
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }

            if(squad.Loadout != null && squad.Loadout.Count > 0)
            {
                SaveSquadLoadout(transaction, squad);
            }
        }

        private void SaveSquadLoadout(IDbTransaction transaction, Squad squad)
        {
            foreach(WeaponSet weaponSet in squad.Loadout)
            {
                string insert = $@"INSERT INTO SquadWeaponSet VALUES 
                    ({squad.Id}, {weaponSet.Id});";
                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = insert;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
