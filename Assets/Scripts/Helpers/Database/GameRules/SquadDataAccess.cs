using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Equippables;
using OnlyWar.Scripts.Models.Squads;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlyWar.Scripts.Helpers.Database.GameRules
{
    public class SquadDataBlob
    {
        public Dictionary<int, List<ArmorTemplate>> ArmorTemplatesByFactionId { get; set; }
        public Dictionary<int, RangedWeaponTemplate> RangedWeaponTemplateMap { get; set; }
        public Dictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplateMap { get; set; }
        public Dictionary<int, List<SoldierType>> SoldierTypesByFactionId { get; set; }
        public Dictionary<int, SoldierType> SoldierTypesById { get; set; }
        public Dictionary<int, List<SquadTemplate>> SquadTemplatesByFactionId { get; set; }
        public Dictionary<int, SquadTemplate> SquadTemplatesById { get; set; }
    }

    public class SquadDataAccess
    {
        public SquadDataBlob GetSquadDataBlob(IDbConnection connection, 
                                              Dictionary<int, BaseSkill> baseSkillMap)
        {
            var soldierTypeSkills = GetSoldierTypeTrainingBySoldierTypeId(connection, baseSkillMap);
            var soldierTypes = GetSoldierTypesById(connection, soldierTypeSkills);
            var armorTemplates = GetArmorTemplatesByFactionId(connection);
            var meleeWeapons = GetMeleeWeaponTemplates(connection, baseSkillMap);
            var rangedWeapons = GetRangedWeaponTemplates(connection, baseSkillMap);
            var weaponSets = GetWeaponSetMap(connection, meleeWeapons, rangedWeapons);
            var squadTemplateWeaponSetIds = 
                GetSquadTemplateWeaponSetIdsBySquadTemplateWeaponOptionId(connection);
            var squadWeaponOptions = GetSquadWeaponOptionsBySquadTemplateId(connection, 
                                                                            squadTemplateWeaponSetIds, 
                                                                            weaponSets);
            var squadElements = GetSquadTemplateElementsBySquadId(connection, soldierTypes.Item2);
            var squadTemplates = GetSquadTemplatesById(connection, squadElements, weaponSets, 
                                                       squadWeaponOptions, armorTemplates);
            return new SquadDataBlob
            {
                ArmorTemplatesByFactionId = armorTemplates,
                MeleeWeaponTemplateMap= meleeWeapons,
                RangedWeaponTemplateMap = rangedWeapons,
                SquadTemplatesByFactionId = squadTemplates.Item1,
                SquadTemplatesById = squadTemplates.Item2,
                SoldierTypesByFactionId = soldierTypes.Item1,
                SoldierTypesById = soldierTypes.Item2
            };
        }

        private Dictionary<int, List<ArmorTemplate>> GetArmorTemplatesByFactionId(
            IDbConnection connection)
        {
            Dictionary<int, List<ArmorTemplate>> factionArmorTemplateMap =
                new Dictionary<int, List<ArmorTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ArmorTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                string name = reader[2].ToString();
                //int location = reader.GetInt32(3);
                int armorProvided = reader.GetInt32(4);
                ArmorTemplate armorTemplate = new ArmorTemplate(id, name, (byte)armorProvided);
                if (!factionArmorTemplateMap.ContainsKey(factionId))
                {
                    factionArmorTemplateMap[factionId] = new List<ArmorTemplate>();
                }
                factionArmorTemplateMap[factionId].Add(armorTemplate);
            }
            return factionArmorTemplateMap;
        }

        private Dictionary<int, MeleeWeaponTemplate> GetMeleeWeaponTemplates(
            IDbConnection connection,
            Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, MeleeWeaponTemplate> factionWeaponTemplateMap =
                new Dictionary<int, MeleeWeaponTemplate>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM MeleeWeaponTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader[1].ToString();
                int location = reader.GetInt32(2);
                int baseSkillId = reader.GetInt32(3);
                float accuracy = (float)reader[4];
                float armorMultiplier = (float)reader[5];
                float woundMultiplier = (float)reader[6];
                float requiredStrength = (float)reader[7];
                float strengthMultiplier = (float)reader[8];
                float extraDamage = (float)reader[9];
                float parryMod = (float)reader[10];
                float extraAttacks = (float)reader[11];

                BaseSkill baseSkill = baseSkillMap[baseSkillId];

                MeleeWeaponTemplate weaponTemplate =
                    new MeleeWeaponTemplate(id, name, (EquipLocation)location, baseSkill,
                                            accuracy, armorMultiplier, woundMultiplier,
                                            requiredStrength, strengthMultiplier, extraDamage,
                                            parryMod, extraAttacks);
                factionWeaponTemplateMap[id] = weaponTemplate;
            }
            return factionWeaponTemplateMap;
        }

        private Dictionary<int, RangedWeaponTemplate> GetRangedWeaponTemplates(
            IDbConnection connection,
            Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, RangedWeaponTemplate> factionWeaponTemplateMap =
                new Dictionary<int, RangedWeaponTemplate>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM RangedWeaponTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader[1].ToString();
                int location = reader.GetInt32(2);
                int baseSkillId = reader.GetInt32(3);
                float accuracy = (float)reader[4];
                float armorMultiplier = (float)reader[5];
                float woundMultiplier = (float)reader[6];
                float requiredStrength = (float)reader[7];
                float damageMultiplier = (float)reader[8];
                float maxRange = (float)reader[9];
                byte rof = reader.GetByte(10);
                ushort ammo = (ushort)reader.GetInt16(11);
                ushort recoil = (ushort)reader.GetInt16(12);
                ushort bulk = (ushort)reader.GetInt16(13);
                bool doesDamageDegrade = (bool)reader[14];

                BaseSkill baseSkill = baseSkillMap[baseSkillId];

                RangedWeaponTemplate weaponTemplate =
                    new RangedWeaponTemplate(id, name, (EquipLocation)location, baseSkill,
                                            accuracy, armorMultiplier, woundMultiplier,
                                            requiredStrength, damageMultiplier, maxRange,
                                            rof, ammo, recoil, bulk, doesDamageDegrade);
                factionWeaponTemplateMap[id] = weaponTemplate;
            }
            return factionWeaponTemplateMap;
        }

        private Dictionary<int, WeaponSet> GetWeaponSetMap(
            IDbConnection connection,
            Dictionary<int, MeleeWeaponTemplate> meleeWeaponMap,
            Dictionary<int, RangedWeaponTemplate> rangedWeaponMap)
        {
            RangedWeaponTemplate primaryRanged, secondaryRanged;
            MeleeWeaponTemplate primaryMelee, secondaryMelee;
            Dictionary<int, WeaponSet> weaponSetMap = new Dictionary<int, WeaponSet>();

            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM WeaponSet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                string name = reader[2].ToString();

                if (reader[3].GetType() != typeof(DBNull))
                {
                    primaryRanged = rangedWeaponMap[reader.GetInt32(3)];
                }
                else
                {
                    primaryRanged = null;
                }

                if (reader[4].GetType() != typeof(DBNull))
                {
                    secondaryRanged = rangedWeaponMap[reader.GetInt32(4)];
                }
                else
                {
                    secondaryRanged = null;
                }

                if (reader[5].GetType() != typeof(DBNull))
                {
                    primaryMelee = meleeWeaponMap[reader.GetInt32(5)];
                }
                else
                {
                    primaryMelee = null;
                }

                if (reader[6].GetType() != typeof(DBNull))
                {
                    secondaryMelee = meleeWeaponMap[reader.GetInt32(6)];
                }
                else
                {
                    secondaryMelee = null;
                }

                WeaponSet weaponSet = new WeaponSet(id, name, primaryRanged, secondaryRanged,
                                                    primaryMelee, secondaryMelee);
                weaponSetMap[id] = weaponSet;
            }

            return weaponSetMap;
        }

        private Dictionary<int, List<int>> GetSquadTemplateWeaponSetIdsBySquadTemplateWeaponOptionId(IDbConnection connection)
        {
            Dictionary<int, List<int>> weaponOptionToWeaponSetMap = new Dictionary<int, List<int>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SquadTemplateWeaponOptionWeaponSet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int weaponSetId = reader.GetInt32(1);
                int squadTemplateWeaponOption = reader.GetInt32(2);
                if (!weaponOptionToWeaponSetMap.ContainsKey(squadTemplateWeaponOption))
                {
                    weaponOptionToWeaponSetMap[squadTemplateWeaponOption] = new List<int>();
                }
                weaponOptionToWeaponSetMap[squadTemplateWeaponOption].Add(weaponSetId);
            }
            return weaponOptionToWeaponSetMap;
        }

        private Dictionary<int, List<SquadWeaponOption>> GetSquadWeaponOptionsBySquadTemplateId(
            IDbConnection connection,
            Dictionary<int, List<int>> weaponOptionWeaponSetMap,
            Dictionary<int, WeaponSet> weaponSetMap)
        {
            Dictionary<int, List<SquadWeaponOption>> squadWeaponOptionMap =
                new Dictionary<int, List<SquadWeaponOption>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SquadTemplateWeaponOption";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int squadTemplateId = reader.GetInt32(1);
                string name = reader[2].ToString();
                int min = reader.GetInt32(3);
                int max = reader.GetInt32(4);

                List<int> baseList = weaponOptionWeaponSetMap[id];
                List<WeaponSet> weaponSetList = new List<WeaponSet>();
                foreach (int weaponSetId in baseList)
                {
                    weaponSetList.Add(weaponSetMap[weaponSetId]);
                }

                SquadWeaponOption weaponOption = new SquadWeaponOption(name, min, max, weaponSetList);
                if (!squadWeaponOptionMap.ContainsKey(squadTemplateId))
                {
                    squadWeaponOptionMap[squadTemplateId] = new List<SquadWeaponOption>();
                }
                squadWeaponOptionMap[squadTemplateId].Add(weaponOption);
            }
            return squadWeaponOptionMap;
        }

        private Dictionary<int, List<SquadTemplateElement>> GetSquadTemplateElementsBySquadId(IDbConnection connection,
                                                                                              Dictionary<int, SoldierType> soldierTypeMap)
        {
            Dictionary<int, List<SquadTemplateElement>> elementsMap =
                new Dictionary<int, List<SquadTemplateElement>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SquadTemplateElement";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int squadTemplateId = reader.GetInt32(1);
                int soldierTypeId = reader.GetInt32(2);
                int min = reader.GetInt32(3);
                int max = reader.GetInt32(4);

                SoldierType type = soldierTypeMap[soldierTypeId];

                if (!elementsMap.ContainsKey(squadTemplateId))
                {
                    elementsMap[squadTemplateId] = new List<SquadTemplateElement>();
                }
                elementsMap[squadTemplateId].Add(new SquadTemplateElement(type, (byte)min, (byte)max));
            }
            return elementsMap;
        }

        private Tuple<Dictionary<int, List<SquadTemplate>>, Dictionary<int, SquadTemplate>> GetSquadTemplatesById(
            IDbConnection connection,
            Dictionary<int, List<SquadTemplateElement>> elementMap,
            Dictionary<int, WeaponSet> weaponSetMap,
            Dictionary<int, List<SquadWeaponOption>> squadWeaponOptionMap,
            Dictionary<int, List<ArmorTemplate>> armorTemplateMap)
        {
            Dictionary<int, SquadTemplate> squadTemplateMap = new Dictionary<int, SquadTemplate>();
            Dictionary<int, List<SquadTemplate>> squadTemplatesByFactionId = new Dictionary<int, List<SquadTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SquadTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                string name = reader[2].ToString();
                int defaultArmorId = reader.GetInt32(3);
                int defaultWeaponSetId = reader.GetInt32(4);
                int squadType = reader.GetInt32(5);

                List<ArmorTemplate> armorList = armorTemplateMap[factionId];
                ArmorTemplate defaultArmor = armorList.First(at => at.Id == defaultArmorId);
                List<SquadWeaponOption> options = squadWeaponOptionMap.ContainsKey(id) ?
                    squadWeaponOptionMap[id] : null;
                SquadTemplate squadTemplate = new SquadTemplate(id, name, weaponSetMap[defaultWeaponSetId],
                                                                options, defaultArmor,
                                                                elementMap[id], (SquadTypes)squadType);
                squadTemplateMap[id] = squadTemplate;
                if (!squadTemplatesByFactionId.ContainsKey(factionId))
                {
                    squadTemplatesByFactionId[factionId] = new List<SquadTemplate>();
                }
                squadTemplatesByFactionId[factionId].Add(squadTemplate);
            }
            return new Tuple<Dictionary<int, List<SquadTemplate>>, Dictionary<int, SquadTemplate>>(squadTemplatesByFactionId, squadTemplateMap);
        }

        private Dictionary<int, List<Tuple<BaseSkill, float>>> GetSoldierTypeTrainingBySoldierTypeId(
            IDbConnection connection, Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, List<Tuple<BaseSkill, float>>> soldierTypeMap =
                new Dictionary<int, List<Tuple<BaseSkill, float>>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierTypeTraining";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int soldierTypeId = reader.GetInt32(0);
                int baseSkillId = reader.GetInt32(1);
                float points = (float)reader[2];

                BaseSkill baseSkill = baseSkillMap[baseSkillId];

                Tuple<BaseSkill, float> training = new Tuple<BaseSkill, float>(baseSkill, points);

                if (!soldierTypeMap.ContainsKey(soldierTypeId))
                {
                    soldierTypeMap[soldierTypeId] = new List<Tuple<BaseSkill, float>>();
                }
                soldierTypeMap[soldierTypeId].Add(training);
            }
            return soldierTypeMap;
        }

        private Tuple<Dictionary<int, List<SoldierType>>, Dictionary<int, SoldierType>> GetSoldierTypesById(
            IDbConnection connection,
            Dictionary<int, List<Tuple<BaseSkill, float>>> soldierTypeTrainingMap)
        {
            Dictionary<int, SoldierType> soldierTypeMap = new Dictionary<int, SoldierType>();
            Dictionary<int, List<SoldierType>> soldierTypesByFactionId = new Dictionary<int, List<SoldierType>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierType";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                string name = reader[2].ToString();
                int rank = reader.GetInt32(3);
                bool isSquadLeader = (bool)reader[4];
                List<Tuple<BaseSkill, float>> trainingList = null;
                if(soldierTypeTrainingMap.ContainsKey(id))
                {
                    trainingList = soldierTypeTrainingMap[id];
                }
                SoldierType soldierType = 
                    new SoldierType(id, name, isSquadLeader, (byte)rank, trainingList);
                soldierTypeMap[id] = soldierType;

                if (!soldierTypesByFactionId.ContainsKey(factionId))
                {
                    soldierTypesByFactionId[factionId] = new List<SoldierType>();
                }
                soldierTypesByFactionId[factionId].Add(soldierType);
            }
            return new Tuple<Dictionary<int, List<SoldierType>>, Dictionary<int, SoldierType>>(soldierTypesByFactionId, soldierTypeMap);
        }

    }
}
