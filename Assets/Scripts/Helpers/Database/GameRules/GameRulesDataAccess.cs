using Mono.Data.Sqlite;
using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Fleets;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Equippables;
using OnlyWar.Scripts.Models.Planets;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Database.GameRules
{
    public class GameRulesBlob
    {
        public List<Faction> Factions { get; set; }
        public Dictionary<int, BaseSkill> BaseSkills { get; set; }
        public Dictionary<int, List<HitLocationTemplate>> BodyTemplates { get; set; }
        public Dictionary<int, PlanetTemplate> PlanetTemplates { get; set; } 
    }

    public class GameRulesDataAccess
    {
        private readonly BaseSkillDataAccess _baseSkillDataAccess;
        private readonly HitLocationTemplateDataAccess _hitLocationDataAccess;
        private readonly FleetDataAccess _fleetDataAccess;
        private readonly PlanetTemplateDataAccess _planetDataAccess;
        private readonly SquadDataAccess _squadDataAccess;

        private static GameRulesDataAccess _instance;

        private GameRulesDataAccess()
        {
            _baseSkillDataAccess = new BaseSkillDataAccess();
            _hitLocationDataAccess = new HitLocationTemplateDataAccess();
            _fleetDataAccess = new FleetDataAccess();
            _planetDataAccess = new PlanetTemplateDataAccess();
            _squadDataAccess = new SquadDataAccess();
        }

        public static GameRulesDataAccess Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GameRulesDataAccess();
                }
                return _instance;
            }
        }
        public GameRulesBlob GetData()
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/GameData/OnlyWar.s3db";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            var baseSkills = _baseSkillDataAccess.GetBaseSkills(dbCon);
            var squadDataBlob = _squadDataAccess.GetSquadDataBlob(dbCon, baseSkills);
            var unitSquadTemplates = GetSquadTemplatesByUnitTemplateId(
                dbCon, squadDataBlob.SquadTemplatesById);
            var unitHierarchy = GetUnitTemplateHierarchy(dbCon);
            var unitTemplates = GetUnitTemplatesByFactionId(dbCon, unitHierarchy, unitSquadTemplates, 
                                                            squadDataBlob.SquadTemplatesById);
            var attributes = GetAttributeTemplates(dbCon);
            var skillTemplates = GetSkillTemplates(dbCon, baseSkills);
            var skillTemplatesBySoldierTemplate = GetSkillTemplatesBySoldierTemplateId(dbCon, skillTemplates);
            var hitLocations = _hitLocationDataAccess.GetHitLocationsByBodyId(dbCon);
            var soldierTemplates = GetSoldierTemplatesByFactionId(
                dbCon, squadDataBlob.SoldierTypesById, attributes, 
                hitLocations, skillTemplatesBySoldierTemplate);

            var planetTemplates = _planetDataAccess.GetData(dbCon);

            var fleetDataBlob = _fleetDataAccess.GetFleetData(dbCon);
            var factions = GetFactionTemplates(dbCon, squadDataBlob.SoldierTypesByFactionId, 
                                               squadDataBlob.RangedWeaponTemplatesByFactionId, 
                                               squadDataBlob.MeleeWeaponTemplatesByFactionId, 
                                               squadDataBlob.ArmorTemplatesByFactionId,
                                               soldierTemplates, 
                                               squadDataBlob.SquadTemplatesByFactionId, 
                                               unitTemplates,
                                               fleetDataBlob.BoatTemplates, 
                                               fleetDataBlob.ShipTemplates, 
                                               fleetDataBlob.FleetTemplates);
            dbCon.Close();
            return new GameRulesBlob
            {
                Factions = factions,
                BaseSkills = baseSkills,
                BodyTemplates = hitLocations,
                PlanetTemplates = planetTemplates
            };
        }

        private List<Faction> GetFactionTemplates(IDbConnection connection,
                                         Dictionary<int, List<SoldierType>> factionSoldierTypeMap,
                                         Dictionary<int, List<RangedWeaponTemplate>> factionRangedWeaponMap,
                                         Dictionary<int, List<MeleeWeaponTemplate>> factionMeleeWeaponMap,
                                         Dictionary<int, List<ArmorTemplate>> factionArmorMap,
                                         Dictionary<int, List<SoldierTemplate>> factionSoldierMap,
                                         Dictionary<int, List<SquadTemplate>> factionSquadMap,
                                         Dictionary<int, List<UnitTemplate>> factionUnitMap,
                                         Dictionary<int, List<BoatTemplate>> factionBoatMap,
                                         Dictionary<int, List<ShipTemplate>> factionShipMap,
                                         Dictionary<int, List<FleetTemplate>> factionFleetMap)
        {
            List<Faction> factionList = new List<Faction>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Faction";
            var reader = command.ExecuteReader();
            while(reader.Read())
            {
                int id = reader.GetInt32(0);
                string name = reader[1].ToString();
                Color color = ConvertDatabaseObjectToColor(reader[2]);
                bool isPlayer = (bool)reader[3];

                var soldierTypeMap = factionSoldierTypeMap[id].ToDictionary(st => st.Id);
                var rangedMap = factionRangedWeaponMap[id].ToDictionary(rw => rw.Id);
                var meleeMap = factionMeleeWeaponMap[id].ToDictionary(mw => mw.Id);
                var armorMap = factionArmorMap[id].ToDictionary(at => at.Id);
                var soldierMap = factionSoldierMap[id].ToDictionary(st => st.Id);
                var squadMap = factionSquadMap[id].ToDictionary(st => st.Id);
                var unitMap = factionUnitMap[id].ToDictionary(ut => ut.Id);
                Dictionary<int, BoatTemplate> boatMap = null;
                Dictionary<int, ShipTemplate> shipMap = null;
                Dictionary<int, FleetTemplate> fleetMap = null;
                if(factionShipMap.ContainsKey(id))
                {
                    boatMap = factionBoatMap[id].ToDictionary(bt => bt.Id);
                    shipMap = factionShipMap[id].ToDictionary(st => st.Id);
                    fleetMap = factionFleetMap[id].ToDictionary(ft => ft.Id);
                }

                Faction factionTemplate = new Faction(id, name, color, isPlayer,
                                                                      soldierTypeMap, rangedMap,
                                                                      meleeMap, armorMap,
                                                                      soldierMap,
                                                                      squadMap, unitMap,
                                                                      boatMap, shipMap, fleetMap);
                factionList.Add(factionTemplate);
            }
            return factionList;
        }

        private Dictionary<int, List<int>> GetUnitTemplateHierarchy(IDbConnection connection)
        {
            Dictionary<int, List<int>> unitTemplateTree = new Dictionary<int, List<int>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM UnitTemplateTree";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int parentUnitId = reader.GetInt32(1);
                int childUnitId = reader.GetInt32(2);

                if (!unitTemplateTree.ContainsKey(parentUnitId))
                {
                    unitTemplateTree[parentUnitId] = new List<int>();
                }
                unitTemplateTree[parentUnitId].Add(childUnitId);
            }
            return unitTemplateTree;
        }

        private Dictionary<int, List<UnitTemplate>> GetUnitTemplatesByFactionId(IDbConnection connection,
                                                                                Dictionary<int, List<int>> unitTemplateTree,
                                                                                Dictionary<int, List<SquadTemplate>> unitSquadMap,
                                                                                Dictionary<int, SquadTemplate> squadTemplateMap)
        {
            Dictionary<int, List<UnitTemplate>> factionUnitTemplateMap = new Dictionary<int, List<UnitTemplate>>();
            Dictionary<int, UnitTemplate> unitTemplateMap = new Dictionary<int, UnitTemplate>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM UnitTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                string name = reader[2].ToString();
                bool isTop = (bool)reader[3];
                int hqSquadTemplateId = reader.GetInt32(4);

                if (!factionUnitTemplateMap.ContainsKey(factionId))
                {
                    factionUnitTemplateMap[factionId] = new List<UnitTemplate>();
                }
                UnitTemplate unitTemplate = new UnitTemplate(id, name, isTop, 
                                                             squadTemplateMap[hqSquadTemplateId], 
                                                             unitSquadMap[id]);
                factionUnitTemplateMap[factionId].Add(unitTemplate);
                unitTemplateMap[id] = unitTemplate;
            }

            // hydrate unit children
            foreach(KeyValuePair<int, List<int>> kvp in unitTemplateTree)
            {
                unitTemplateMap[kvp.Key].SetChildUnits(kvp.Value.Select(i => unitTemplateMap[i]).ToList());
            }

            return factionUnitTemplateMap;
        }

        private Dictionary<int, NormalizedValueTemplate> GetAttributeTemplates(IDbConnection connection)
        {
            Dictionary<int, NormalizedValueTemplate> attributeTemplateMap =
                new Dictionary<int, NormalizedValueTemplate>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM AttributeTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                float baseValue = (float)reader[1];
                float stdDev = (float)reader[2];
                NormalizedValueTemplate attributeTemplate = new NormalizedValueTemplate
                {
                    BaseValue = baseValue,
                    StandardDeviation = stdDev
                };

                attributeTemplateMap[id] = attributeTemplate;
            }
            return attributeTemplateMap;
        }

        private Dictionary<int, SkillTemplate> GetSkillTemplates(IDbConnection connection,
                                                                    Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, SkillTemplate> skillTemplateMap =
                new Dictionary<int, SkillTemplate>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SkillTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int baseSkillId = reader.GetInt32(1);
                float baseValue = (float)reader[2];
                float stdDev = (float)reader[3];
                SkillTemplate skillTemplate = new SkillTemplate
                {
                    BaseSkill = baseSkillMap[baseSkillId],
                    BaseValue = baseValue,
                    StandardDeviation = stdDev
                };

                skillTemplateMap[id] = skillTemplate;
            }
            return skillTemplateMap;
        }

        private Dictionary<int, List<SkillTemplate>> GetSkillTemplatesBySoldierTemplateId(IDbConnection connection,
                                                                                             Dictionary<int, SkillTemplate> skillTemplateMap)
        {
            Dictionary<int, List<SkillTemplate>> skillTemplateListMap =
                new Dictionary<int, List<SkillTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierTemplateSkillTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int soldierTemplateId = reader.GetInt32(1);
                int skillTemplateId = reader.GetInt32(2);

                if (!skillTemplateListMap.ContainsKey(soldierTemplateId))
                {
                    skillTemplateListMap[soldierTemplateId] = new List<SkillTemplate>();
                }
                skillTemplateListMap[soldierTemplateId].Add(skillTemplateMap[skillTemplateId]);
            }
            return skillTemplateListMap;
        }

        private Dictionary<int, List<SoldierTemplate>> GetSoldierTemplatesByFactionId(IDbConnection connection,
                                                                                         Dictionary<int, SoldierType> soldierTypeMap,
                                                                                         Dictionary<int, NormalizedValueTemplate> attributeMap,
                                                                                         Dictionary<int, List<HitLocationTemplate>> hitLocationTemplateMap,
                                                                                         Dictionary<int, List<SkillTemplate>> skillTemplateMap)
        {
            Dictionary<int, List<SoldierTemplate>> soldierTemplateMap = new Dictionary<int, List<SoldierTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int factionId = reader.GetInt32(1);
                int bodyId = reader.GetInt32(2);
                int soldierTypeId = reader.GetInt32(3);
                string name = reader[4].ToString();
                int strengthTemplateId = reader.GetInt32(5);
                int dexterityTemplateId = reader.GetInt32(6);
                int constitutionTemplateId = reader.GetInt32(7);
                int intelligenceTemplateId = reader.GetInt32(8);
                int perceptionTemplateId = reader.GetInt32(9);
                int egoTemplateId = reader.GetInt32(10);
                int charismaTemplateId = reader.GetInt32(11);
                int psychicTemplateId = reader.GetInt32(12);
                int attackSpeedTemplateId = reader.GetInt32(13);
                int moveSpeedTemplateId = reader.GetInt32(14);
                int sizeTemplateId = reader.GetInt32(15);
                SoldierType type = soldierTypeMap[soldierTypeId];
                SoldierTemplate soldierTemplate = new SoldierTemplate(id, name, type,
                                                                      attributeMap[strengthTemplateId],
                                                                      attributeMap[dexterityTemplateId],
                                                                      attributeMap[constitutionTemplateId],
                                                                      attributeMap[intelligenceTemplateId],
                                                                      attributeMap[perceptionTemplateId],
                                                                      attributeMap[egoTemplateId],
                                                                      attributeMap[charismaTemplateId],
                                                                      attributeMap[psychicTemplateId],
                                                                      attributeMap[attackSpeedTemplateId],
                                                                      attributeMap[moveSpeedTemplateId],
                                                                      attributeMap[sizeTemplateId],
                                                                      skillTemplateMap[id],
                                                                      new BodyTemplate(hitLocationTemplateMap[bodyId]));

                if (!soldierTemplateMap.ContainsKey(factionId))
                {
                    soldierTemplateMap[factionId] = new List<SoldierTemplate>();
                }
                soldierTemplateMap[factionId].Add(soldierTemplate);
            }
            return soldierTemplateMap;
        }

        private Dictionary<int, List<SquadTemplate>> GetSquadTemplatesByUnitTemplateId(IDbConnection connection,
                                                                                       Dictionary<int, SquadTemplate> squadTemplateMap)
        {
            Dictionary<int, List<SquadTemplate>> unitSquadTemplateMap = new Dictionary<int, List<SquadTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM UnitTemplateSquadTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int unitTemplateId = reader.GetInt32(1);
                int squadTemplateId = reader.GetInt32(2);

                if (!unitSquadTemplateMap.ContainsKey(unitTemplateId))
                {
                    unitSquadTemplateMap[unitTemplateId] = new List<SquadTemplate>();
                }
                unitSquadTemplateMap[unitTemplateId].Add(squadTemplateMap[squadTemplateId]);
            }
            return unitSquadTemplateMap;
        }

        private Color ConvertDatabaseObjectToColor(object obj)
        {
            long colorInt = (long)obj;
            long r = colorInt / 0x01000000;
            long g = (colorInt / 0x00010000) & 0x000000ff;
            long b = (colorInt / 0x00000100) & 0x000000ff;
            long a = colorInt & 0x000000ff;
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
    }
}
