using Mono.Data.Sqlite;
using OnlyWar.Models;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Equippables;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Helpers.Database.GameRules
{
    public class GameRulesBlob
    {
        public IReadOnlyList<Faction> Factions { get; set; }
        public IReadOnlyDictionary<int, BaseSkill> BaseSkills { get; set; }
        public IReadOnlyList<SkillTemplate> SkillTemplates { get; set; }
        public IReadOnlyDictionary<int, List<HitLocationTemplate>> BodyTemplates { get; set; }
        public IReadOnlyDictionary<int, PlanetTemplate> PlanetTemplates { get; set; }
        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; set; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; set; }
        public IReadOnlyDictionary<int, WeaponSet> WeaponSets { get; set; }

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
            var skillTemplates = GetSkillTemplates(dbCon, baseSkills);
            var hitLocations = _hitLocationDataAccess.GetHitLocationsByBodyId(dbCon);
            var squadDataBlob = _squadDataAccess.GetSquadDataBlob(dbCon, baseSkills, hitLocations);
            var unitSquadTemplates = GetSquadTemplatesByUnitTemplateId(
                dbCon, squadDataBlob.SquadTemplatesById);
            var unitHierarchy = GetUnitTemplateHierarchy(dbCon);
            var unitTemplates = 
                GetUnitTemplatesByFactionId(dbCon, unitHierarchy, unitSquadTemplates, 
                                            squadDataBlob.SquadTemplatesById);
            var planetTemplates = _planetDataAccess.GetData(dbCon);

            var fleetDataBlob = _fleetDataAccess.GetFleetData(dbCon);
            var factions = GetFactionTemplates(dbCon, squadDataBlob.SpeciesByFactionId,  
                                               squadDataBlob.SoldierTemplatesByFactionId, 
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
                SkillTemplates = skillTemplates,
                BodyTemplates = hitLocations,
                PlanetTemplates = planetTemplates,
                RangedWeaponTemplates = squadDataBlob.RangedWeaponTemplateMap,
                MeleeWeaponTemplates = squadDataBlob.MeleeWeaponTemplateMap,
                WeaponSets = squadDataBlob.WeaponSetMap
            };
        }

        private List<Faction> GetFactionTemplates(IDbConnection connection,
                                         Dictionary<int, List<Species>> factionSpeciesMap,
                                         Dictionary<int, List<SoldierTemplate>> factionSoldierTemplateMap,
                                         Dictionary<int, List<SquadTemplate>> factionSquadMap,
                                         Dictionary<int, List<UnitTemplate>> factionUnitMap,
                                         Dictionary<int, List<BoatTemplate>> factionBoatMap,
                                         Dictionary<int, List<ShipTemplate>> factionShipMap,
                                         Dictionary<int, List<FleetTemplate>> factionFleetMap)
        {
            List<Faction> factionList = new List<Faction>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Faction";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader[1].ToString();
                    Color color = ConvertDatabaseObjectToColor(reader[2]);
                    bool isPlayer = (bool)reader[3];
                    bool isDefault = (bool)reader[4];
                    bool canInfiltrate = (bool)reader[5];
                    GrowthType growthType = (GrowthType)reader.GetInt32(6);

                    var speciesMap = factionSpeciesMap.ContainsKey(id) ?
                        factionSpeciesMap[id].ToDictionary(st => st.Id) : null;
                    var soldierMap = factionSoldierTemplateMap.ContainsKey(id) ?
                        factionSoldierTemplateMap[id].ToDictionary(st => st.Id) : null;
                    var squadMap = factionSquadMap.ContainsKey(id) ?
                        factionSquadMap[id].ToDictionary(st => st.Id) : null;
                    var unitMap = factionUnitMap.ContainsKey(id) ?
                        factionUnitMap[id].ToDictionary(ut => ut.Id) : null;
                    Dictionary<int, BoatTemplate> boatMap = null;
                    Dictionary<int, ShipTemplate> shipMap = null;
                    Dictionary<int, FleetTemplate> fleetMap = null;
                    if (factionShipMap.ContainsKey(id))
                    {
                        boatMap = factionBoatMap[id].ToDictionary(bt => bt.Id);
                        shipMap = factionShipMap[id].ToDictionary(st => st.Id);
                        fleetMap = factionFleetMap[id].ToDictionary(ft => ft.Id);
                    }

                    Faction factionTemplate = new Faction(id, name, color, isPlayer, isDefault,
                                                          canInfiltrate, growthType, speciesMap,
                                                          soldierMap, squadMap, unitMap, boatMap,
                                                          shipMap, fleetMap);
                    factionList.Add(factionTemplate);
                }
            }
            return factionList;
        }

        private List<SkillTemplate> GetSkillTemplates(IDbConnection connection,
                                                      Dictionary<int, BaseSkill> baseSkillMap)
        {
            List<SkillTemplate> skillTemplateList = new List<SkillTemplate>();
            using (var command = connection.CreateCommand())
            {
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

                    skillTemplateList.Add(skillTemplate);
                }
            }
            return skillTemplateList;
        }

        private Dictionary<int, List<int>> GetUnitTemplateHierarchy(IDbConnection connection)
        {
            Dictionary<int, List<int>> unitTemplateTree = new Dictionary<int, List<int>>();
            using (var command = connection.CreateCommand())
            {
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
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM UnitTemplate";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);
                    string name = reader[2].ToString();
                    bool isTop = (bool)reader[3];
                    SquadTemplate hqSquad;
                    if (reader[4].GetType() != typeof(DBNull))
                    {
                        hqSquad = squadTemplateMap[reader.GetInt32(4)];
                    }
                    else
                    {
                        hqSquad = null;
                    }

                    if (!factionUnitTemplateMap.ContainsKey(factionId))
                    {
                        factionUnitTemplateMap[factionId] = new List<UnitTemplate>();
                    }
                    UnitTemplate unitTemplate = new UnitTemplate(id, name, isTop, hqSquad, unitSquadMap[id]);
                    factionUnitTemplateMap[factionId].Add(unitTemplate);
                    unitTemplateMap[id] = unitTemplate;
                }

                // hydrate unit children
                foreach (KeyValuePair<int, List<int>> kvp in unitTemplateTree)
                {
                    unitTemplateMap[kvp.Key].SetChildUnits(kvp.Value.Select(i => unitTemplateMap[i]).ToList());
                }
            }
            return factionUnitTemplateMap;
        }

        private Dictionary<int, List<SquadTemplate>> GetSquadTemplatesByUnitTemplateId(IDbConnection connection,
                                                                                       Dictionary<int, SquadTemplate> squadTemplateMap)
        {
            Dictionary<int, List<SquadTemplate>> unitSquadTemplateMap = new Dictionary<int, List<SquadTemplate>>();
            using (var command = connection.CreateCommand())
            {
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
