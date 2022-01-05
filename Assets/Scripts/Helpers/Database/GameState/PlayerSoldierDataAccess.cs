using OnlyWar.Models;
using OnlyWar.Models.Soldiers;
using System.Collections.Generic;
using System.Data;

namespace OnlyWar.Helpers.Database.GameState
{
    class PlayerSoldierDataAccess
    {
        public Dictionary<int, PlayerSoldier> GetData(IDbConnection dbCon, 
                                                      Dictionary<int, Soldier> soldierMap)
        {
            var factionCasualtyMap = GetFactionCasualtiesBySoldierId(dbCon);
            var rangedWeaponCasualtyMap = GetRangedWeaponCasualtiesBySoldierId(dbCon);
            var meleeWeaponCasualtyMap = GetMeleeWeaponCasualtiesBySoldierId(dbCon);
            var historyMap = GetHistoryBySoldierId(dbCon);
            var playerSoldiers = GetPlayerSoldiers(dbCon, soldierMap, factionCasualtyMap, 
                                                   rangedWeaponCasualtyMap, meleeWeaponCasualtyMap, historyMap);
            return playerSoldiers;
        }

        public void SavePlayerSoldier(IDbTransaction transaction, PlayerSoldier playerSoldier)
        {
            string insert = $@"INSERT INTO PlayerSoldier VALUES ({playerSoldier.Id}, 
                {playerSoldier.MeleeRating}, {playerSoldier.RangedRating}, {playerSoldier.LeadershipRating},
                {playerSoldier.MedicalRating}, {playerSoldier.TechRating}, {playerSoldier.PietyRating},
                {playerSoldier.AncientRating},{playerSoldier.ProgenoidImplantDate.Millenium},
                {playerSoldier.ProgenoidImplantDate.Year},{playerSoldier.ProgenoidImplantDate.Week});";
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }

            foreach (KeyValuePair<int, ushort> weaponCasualtyCount in playerSoldier.RangedWeaponCasualtyCountMap)
            {
                insert = $@"INSERT INTO PlayerSoldierRamgedWeaponCasualtyCount VALUES ({playerSoldier.Id}, 
                    {weaponCasualtyCount.Key}, {weaponCasualtyCount.Value});";
                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = insert;
                    command.ExecuteNonQuery();
                }
            }

            foreach (KeyValuePair<int, ushort> weaponCasualtyCount in playerSoldier.MeleeWeaponCasualtyCountMap)
            {
                insert = $@"INSERT INTO PlayerSoldierMeleeWeaponCasualtyCount VALUES ({playerSoldier.Id}, 
                    {weaponCasualtyCount.Key}, {weaponCasualtyCount.Value});";
                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = insert;
                    command.ExecuteNonQuery();
                }
            }

            foreach (KeyValuePair<int, ushort> factionCasualtyCount in playerSoldier.FactionCasualtyCountMap)
            {
                insert = $@"INSERT INTO PlayerSoldierFactionCasualtyCount VALUES ({playerSoldier.Id}, 
                    {factionCasualtyCount.Key}, {factionCasualtyCount.Value});";
                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = insert;
                    command.ExecuteNonQuery();
                }
            }

            foreach (string entry in playerSoldier.SoldierHistory)
            {
                string safeEntry = entry.Replace("\'", "\'\'");
                insert = $@"INSERT INTO PlayerSoldierHistory VALUES ({playerSoldier.Id}, '{safeEntry}');";
                using (var command = transaction.Connection.CreateCommand())
                {
                    command.CommandText = insert;
                    command.ExecuteNonQuery();
                }
            }
        }

        private Dictionary<int, Dictionary<int, ushort>> GetFactionCasualtiesBySoldierId(IDbConnection connection)
        {
            Dictionary<int, Dictionary<int, ushort>> soldierFactionCasualtyMap = 
                new Dictionary<int, Dictionary<int, ushort>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerSoldierFactionCasualtyCount";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int soldierId = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);
                    ushort count = (ushort)reader.GetInt32(2);

                    if (!soldierFactionCasualtyMap.ContainsKey(soldierId))
                    {
                        soldierFactionCasualtyMap[soldierId] = new Dictionary<int, ushort>();
                    }
                    soldierFactionCasualtyMap[soldierId][factionId] = count;

                }
            }
            return soldierFactionCasualtyMap;
        }

        private Dictionary<int, Dictionary<int, ushort>> GetRangedWeaponCasualtiesBySoldierId(IDbConnection connection)
        {
            Dictionary<int, Dictionary<int, ushort>> soldierWeaponCasualtyMap =
                new Dictionary<int, Dictionary<int, ushort>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerSoldierRangedWeaponCasualtyCount";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int soldierId = reader.GetInt32(0);
                    int weaponTemplateId = reader.GetInt32(1);
                    ushort count = (ushort)reader.GetInt32(2);

                    if (!soldierWeaponCasualtyMap.ContainsKey(soldierId))
                    {
                        soldierWeaponCasualtyMap[soldierId] = new Dictionary<int, ushort>();
                    }
                    soldierWeaponCasualtyMap[soldierId][weaponTemplateId] = count;

                }
            }
            return soldierWeaponCasualtyMap;
        }

        private Dictionary<int, Dictionary<int, ushort>> GetMeleeWeaponCasualtiesBySoldierId(IDbConnection connection)
        {
            Dictionary<int, Dictionary<int, ushort>> soldierWeaponCasualtyMap =
                new Dictionary<int, Dictionary<int, ushort>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerSoldierMeleeWeaponCasualtyCount";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int soldierId = reader.GetInt32(0);
                    int weaponTemplateId = reader.GetInt32(1);
                    ushort count = (ushort)reader.GetInt32(2);

                    if (!soldierWeaponCasualtyMap.ContainsKey(soldierId))
                    {
                        soldierWeaponCasualtyMap[soldierId] = new Dictionary<int, ushort>();
                    }
                    soldierWeaponCasualtyMap[soldierId][weaponTemplateId] = count;

                }
            }
            return soldierWeaponCasualtyMap;
        }

        private Dictionary<int, List<string>> GetHistoryBySoldierId(IDbConnection connection)
        {
            Dictionary<int, List<string>> soldierEntryListMap = new Dictionary<int, List<string>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerSoldierHistory";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int soldierId = reader.GetInt32(0);
                    string entry = reader[1].ToString();

                    if (!soldierEntryListMap.ContainsKey(soldierId))
                    {
                        soldierEntryListMap[soldierId] = new List<string>();
                    }
                    soldierEntryListMap[soldierId].Add(entry);

                }
            }
            return soldierEntryListMap;
        }

        private Dictionary<int, PlayerSoldier> GetPlayerSoldiers(IDbConnection connection,
                                                                 IReadOnlyDictionary<int, Soldier> baseSoldierMap,
                                                                 IReadOnlyDictionary<int, Dictionary<int, ushort>> factionCasualtyMap,
                                                                 IReadOnlyDictionary<int, Dictionary<int, ushort>> rangedWeaponCasualtyMap,
                                                                 IReadOnlyDictionary<int, Dictionary<int, ushort>> meleeWeaponCasualtyMap,
                                                                 IReadOnlyDictionary<int, List<string>> historyMap)
        {
            Dictionary<int, PlayerSoldier> playerSoldierMap = new Dictionary<int, PlayerSoldier>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerSoldier";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int soldierId = reader.GetInt32(0);
                    float melee = (float)reader[1];
                    float ranged = (float)reader[2];
                    float leadership = (float)reader[3];
                    float medical = (float)reader[4];
                    float tech = (float)reader[5];
                    float piety = (float)reader[6];
                    float ancient = (float)reader[7];
                    int implantMillenium = reader.GetInt32(8);
                    int implantYear = reader.GetInt32(9);
                    int implantWeek = reader.GetInt32(10);

                    Date implantDate = new Date(implantMillenium, implantYear, implantWeek);

                    List<string> history;
                    if (historyMap.ContainsKey(soldierId))
                    {
                        history = historyMap[soldierId];
                    }
                    else
                    {
                        history = new List<string>();
                    }

                    Dictionary<int, ushort> rangedWeaponCasualties;
                    if (rangedWeaponCasualtyMap.ContainsKey(soldierId))
                    {
                        rangedWeaponCasualties = rangedWeaponCasualtyMap[soldierId];
                    }
                    else
                    {
                        rangedWeaponCasualties = new Dictionary<int, ushort>();
                    }

                    Dictionary<int, ushort> meleeWeaponCasualties;
                    if (meleeWeaponCasualtyMap.ContainsKey(soldierId))
                    {
                        meleeWeaponCasualties = meleeWeaponCasualtyMap[soldierId];
                    }
                    else
                    {
                        meleeWeaponCasualties = new Dictionary<int, ushort>();
                    }

                    Dictionary<int, ushort> factionCasualties;
                    if (factionCasualtyMap.ContainsKey(soldierId))
                    {
                        factionCasualties = factionCasualtyMap[soldierId];
                    }
                    else
                    {
                        factionCasualties = new Dictionary<int, ushort>();
                    }

                    PlayerSoldier playerSoldier = new PlayerSoldier(baseSoldierMap[soldierId], melee, ranged,
                                                                    leadership, medical, tech, piety, ancient,
                                                                    implantDate, history, rangedWeaponCasualties,
                                                                    meleeWeaponCasualties, factionCasualties);

                    playerSoldierMap[soldierId] = playerSoldier;

                }
            }
            return playerSoldierMap;
        }
    }
}
