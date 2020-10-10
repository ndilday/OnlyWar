using Mono.Data.Sqlite;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Database
{
    public class SoldierDataAccess
    {
        public Dictionary<int, Soldier> GetData(string fileName, 
                                                Dictionary<int, HitLocationTemplate> hitLocationTemplateMap,
                                                Dictionary<int, BaseSkill> baseSkillMap, 
                                                Dictionary<int, SoldierType> soldierTypeMap,
                                                Dictionary<int, Squad> squadMap)
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();

            var hitLocationMap = GetHitLocationsBySoldierId(dbCon, hitLocationTemplateMap);
            var soldierSkillMap = GetSkillsBySoldierId(dbCon, baseSkillMap);
            var soldierMap = GetSoldiers(dbCon, soldierTypeMap, squadMap, soldierSkillMap, hitLocationMap);

            dbCon.Close();
            return soldierMap;
        }

        public void SaveData(string fileName, List<ISoldier> soldiers)
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/Saves/{fileName}";
            IDbConnection dbCon = new SqliteConnection(connection);
            using (var transaction = dbCon.BeginTransaction())
            {
                try
                {
                    foreach (Soldier soldier in soldiers)
                    {
                        SaveSoldier(transaction, soldier);
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
                transaction.Commit();
            }
        }

        private Dictionary<int, List<HitLocation>> GetHitLocationsBySoldierId(IDbConnection connection,
                                                                              Dictionary<int, HitLocationTemplate> hitLocationTemplateMap)
        {
            Dictionary<int, List<HitLocation>> hitLocationMap = new Dictionary<int, List<HitLocation>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM HitLocation";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int soldierId = reader.GetInt32(1);
                int hitLocationTemplateId = reader.GetInt32(2);
                int woundTotal = reader.GetInt32(3);
                int weeksOfHealing = reader.GetInt32(4);
                HitLocation hitLocation = new HitLocation(hitLocationTemplateMap[hitLocationTemplateId],
                                                          (uint)woundTotal, (uint)weeksOfHealing);

                if (!hitLocationMap.ContainsKey(soldierId))
                {
                    hitLocationMap[soldierId] = new List<HitLocation>();
                }
                hitLocationMap[soldierId].Add(hitLocation);
            }
            return hitLocationMap;
        }

        private Dictionary<int, List<Skill>> GetSkillsBySoldierId(IDbConnection connection,
                                                                  Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, List<Skill>> skillMap = new Dictionary<int, List<Skill>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierSkill";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int soldierId = reader.GetInt32(1);
                int baseSkillId = reader.GetInt32(2);
                float points = (float)reader[3];
                BaseSkill baseSkill = baseSkillMap[baseSkillId];

                Skill skill = new Skill(baseSkill, points);

                if (!skillMap.ContainsKey(soldierId))
                {
                    skillMap[soldierId] = new List<Skill>();
                }
                skillMap[soldierId].Add(skill);
            }
            return skillMap;
        }

        private Dictionary<int, Soldier> GetSoldiers(IDbConnection connection, 
                                                     Dictionary<int, SoldierType> soldierTypeMap,
                                                     Dictionary<int, Squad> squadMap,
                                                     Dictionary<int, List<Skill>> skillMap,
                                                     Dictionary<int, List<HitLocation>> hitLocationMap)
        {
            Dictionary<int, Soldier> soldiers = new Dictionary<int, Soldier>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Soldier";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int soldierTypeId = reader.GetInt32(1);
                int squadId = reader.GetInt32(2);
                string name = reader[3].ToString();
                float strength = (float)reader[4];
                float dexterity = (float)reader[5];
                float constitution = (float)reader[6];
                float intelligence = (float)reader[7];
                float perception = (float)reader[8];
                float ego = (float)reader[9];
                float charisma = (float)reader[10];
                float psychic= (float)reader[11];
                float attack = (float)reader[12];
                float size = (float)reader[13];
                float move = (float)reader[14];


                Soldier soldier = new Soldier(hitLocationMap[id], skillMap[id])
                {
                    Strength = strength,
                    Dexterity = dexterity,
                    Constitution = constitution,
                    Intelligence = intelligence,
                    Perception = perception,
                    Ego = ego,
                    Charisma = charisma,
                    PsychicPower = psychic,
                    AttackSpeed = attack,
                    Size = size,
                    MoveSpeed = move,
                    Id = id,
                    Name = name,
                    Type = soldierTypeMap[soldierTypeId]
                };

                // due to how we handle decorating with PlayerSoldier, we may need to adjust this
                squadMap[squadId].AddSquadMember(soldier);

                soldiers[id] = soldier;
            }
            return soldiers;
        }

        private void SaveSoldier(IDbTransaction transaction, Soldier soldier)
        {
            string safeName = soldier.Name.Replace("\'", "\'\'");
            string insert = $@"INSERT INTO Soldier VALUES ({soldier.Id}, 
                {soldier.Type.Id}, {soldier.AssignedSquad.Id}, '{safeName}',
                {soldier.Strength}, {soldier.Dexterity}, {soldier.Constitution},
                {soldier.Intelligence},{soldier.Perception}, {soldier.Ego}, {soldier.Charisma}, 
                {soldier.PsychicPower},{soldier.AttackSpeed}, {soldier.Size}, {soldier.MoveSpeed});";
            IDbCommand command = transaction.Connection.CreateCommand();
            command.CommandText = insert;
            command.ExecuteNonQuery();

            foreach (Skill skill in soldier.Skills)
            {
                insert = $@"INSERT INTO SoldierSkill VALUES ({soldier.Id}, 
                    {skill.BaseSkill.Id}, {skill.PointsInvested});";
                command = transaction.Connection.CreateCommand();
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }

            foreach (HitLocation hitLocation in soldier.Body.HitLocations)
            {
                insert = $@"INSERT INTO HitLocation VALUES ({soldier.Id}, {hitLocation.Template.Id}, 
                    {hitLocation.Wounds.WoundTotal}, {hitLocation.Wounds.WeeksOfHealing});";
                command = transaction.Connection.CreateCommand();
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
        }
    }
}
