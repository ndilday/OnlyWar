using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models.Factions;
using Iam.Scripts.Models.Fleets;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Helpers.Database
{
    public class FactionDataAccess
    {
        public List<Faction> GetData()
        {
            string connection = $"URI=file:{Application.streamingAssetsPath}/GameData/OnlyWar.s3db";
            IDbConnection dbCon = new SqliteConnection(connection);
            dbCon.Open();
            var baseSkills = GetBaseSkills(dbCon);
            var soldierTypes = GetSoldierTypesById(dbCon);
            var armorTemplates = GetArmorTemplatesByFactionId(dbCon);
            var meleeWeapons = GetMeleeWeaponTemplatesByFactionId(dbCon, baseSkills);
            var rangedWeapons = GetRangedWeaponTemplatesByFactionId(dbCon, baseSkills);
            var weaponSets = GetWeaponSetsById(dbCon, meleeWeapons, rangedWeapons);
            var squadTemplateWeaponSetIds = GetSquadTemplateWeaponSetIdsBySquadTemplateWeaponOptionId(dbCon);
            var squadWeaponOptions = GetSquadWeaponOptionsBySquadTemplateId(dbCon, squadTemplateWeaponSetIds, weaponSets.Item2);
            var squadElements = GetSquadTemplateElementsBySquadId(dbCon, soldierTypes.Item2);
            var squadTemplates = GetSquadTemplatesById(dbCon, squadElements, weaponSets.Item2, squadWeaponOptions, armorTemplates);
            var unitSquadTemplates = GetSquadTemplatesByUnitTemplateId(dbCon, squadTemplates.Item2);
            var unitHierarchy = GetUnitTemplateHierarchy(dbCon);
            var unitTemplates = GetUnitTemplatesByFactionId(dbCon, unitHierarchy, unitSquadTemplates, squadTemplates.Item2);
            var attributes = GetAttributeTemplates(dbCon);
            var skillTemplates = GetSkillTemplates(dbCon, baseSkills);
            var skillTemplatesBySoldierTemplate = GetSkillTemplatesBySoldierTemplateId(dbCon, skillTemplates);
            var stanceProbabilities = GetStanceHitProbabilitiesByHitLocationId(dbCon);
            var hitLocations = GetHitLocationsByBodyId(dbCon, stanceProbabilities);
            var soldierTemplates = GetSoldierTemplatesByFactionId(dbCon, soldierTypes.Item2, attributes, hitLocations, skillTemplatesBySoldierTemplate);
            var boatTemplates = GetBoatTemplatesByFactionId(dbCon);
            var shipTemplates = GetShipTemplatesByFactionId(dbCon);
            var fleetShips = GetFleetShipTemplateLists(dbCon);
            var fleetTemplates = GetFleetTemplatesByFactionId(dbCon, shipTemplates, fleetShips);
            var factions = GetFactionTemplates(dbCon, soldierTypes.Item1, rangedWeapons, meleeWeapons, armorTemplates,
                                               weaponSets.Item1, soldierTemplates, squadTemplates.Item1, unitTemplates,
                                               boatTemplates, shipTemplates, fleetTemplates);
            return factions;
        }

        private List<Faction> GetFactionTemplates(IDbConnection connection,
                                         Dictionary<int, List<SoldierType>> factionSoldierTypeMap,
                                         Dictionary<int, List<RangedWeaponTemplate>> factionRangedWeaponMap,
                                         Dictionary<int, List<MeleeWeaponTemplate>> factionMeleeWeaponMap,
                                         Dictionary<int, List<ArmorTemplate>> factionArmorMap,
                                         Dictionary<int, List<WeaponSet>> factionWeaponSetMap,
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
                int id = (int)reader[0];
                string name = reader[1].ToString();
                Color color = ConvertDatabaseObjectToColor(reader[2]);
                bool isPlayer = (bool)reader[3];

                var soldierTypeMap = factionSoldierTypeMap[id].ToDictionary(st => st.Id);
                var rangedMap = factionRangedWeaponMap[id].ToDictionary(rw => rw.Id);
                var meleeMap = factionMeleeWeaponMap[id].ToDictionary(mw => mw.Id);
                var armorMap = factionArmorMap[id].ToDictionary(at => at.Id);
                var weaponSetMap = factionWeaponSetMap[id].ToDictionary(ws => ws.Id);
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
                                                                      weaponSetMap, soldierMap,
                                                                      squadMap, unitMap,
                                                                      boatMap, shipMap, fleetMap);
                factionList.Add(factionTemplate);
            }
            return factionList;
        }

        private Dictionary<int, BaseSkill> GetBaseSkills(IDbConnection connection)
        {
            Dictionary<int, BaseSkill> baseSkillMap = new Dictionary<int, BaseSkill>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM BaseSkill";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                string name = reader[1].ToString();
                SkillCategory category = (SkillCategory)reader[2];
                var attribute = (Models.Soldiers.Attribute)reader[3];
                float difficulty = (float)reader[4];
                BaseSkill baseSkill = new BaseSkill(id, category, name, attribute, difficulty);

                baseSkillMap[id] = baseSkill;
            }
            return baseSkillMap;
        }

        private Tuple<Dictionary<int, List<SoldierType>>, Dictionary<int, SoldierType>> GetSoldierTypesById(IDbConnection connection)
        {
            Dictionary<int, SoldierType> soldierTypeMap = new Dictionary<int, SoldierType>();
            Dictionary<int, List<SoldierType>> soldierTypesByFactionId = new Dictionary<int, List<SoldierType>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierType";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                int rank = (int)reader[3];
                bool isSquadLeader = (bool)reader[4];
                SoldierType soldierType = new SoldierType(id, name, isSquadLeader, (byte)rank);
                soldierTypeMap[factionId] = soldierType;

                if (!soldierTypesByFactionId.ContainsKey(factionId))
                {
                    soldierTypesByFactionId[factionId] = new List<SoldierType>();
                }
                soldierTypesByFactionId[factionId].Add(soldierType);
            }
            return new Tuple<Dictionary<int, List<SoldierType>>, Dictionary<int, SoldierType>>(soldierTypesByFactionId, soldierTypeMap);
        }

        private Dictionary<int, List<ArmorTemplate>> GetArmorTemplatesByFactionId(IDbConnection connection)
        {
            Dictionary<int, List<ArmorTemplate>> factionArmorTemplateMap =
                new Dictionary<int, List<ArmorTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ArmorTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                //int location = (int)reader[3];
                int armorProvided = (int)reader[4];
                ArmorTemplate armorTemplate = new ArmorTemplate(id, name, (byte)armorProvided);
                if (!factionArmorTemplateMap.ContainsKey(factionId))
                {
                    factionArmorTemplateMap[factionId] = new List<ArmorTemplate>();
                }
                factionArmorTemplateMap[factionId].Add(armorTemplate);
            }
            return factionArmorTemplateMap;
        }

        private Dictionary<int, List<MeleeWeaponTemplate>> GetMeleeWeaponTemplatesByFactionId(IDbConnection connection, 
                                                                                                 Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, List<MeleeWeaponTemplate>> factionWeaponTemplateMap =
                new Dictionary<int, List<MeleeWeaponTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM MeleeWeaponTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                int location = (int)reader[3];
                int baseSkillId = (int)reader[4];
                float accuracy = (float)reader[5];
                float armorMultiplier = (float)reader[6];
                float woundMultiplier = (float)reader[7];
                float requiredStrength = (float)reader[8];
                float strengthMultiplier = (float)reader[9];
                float extraDamage = (float)reader[10];
                float parryMod = (float)reader[11];
                float extraAttacks = (float)reader[12];

                BaseSkill baseSkill = baseSkillMap[baseSkillId];

                MeleeWeaponTemplate weaponTemplate = 
                    new MeleeWeaponTemplate(id, name, (EquipLocation)location, baseSkill, 
                                            accuracy, armorMultiplier, woundMultiplier,
                                            requiredStrength, strengthMultiplier, extraDamage,
                                            parryMod, extraAttacks);
                if (!factionWeaponTemplateMap.ContainsKey(factionId))
                {
                    factionWeaponTemplateMap[factionId] = new List<MeleeWeaponTemplate>();
                }
                factionWeaponTemplateMap[factionId].Add(weaponTemplate);
            }
            return factionWeaponTemplateMap;
        }

        private Dictionary<int, List<RangedWeaponTemplate>> GetRangedWeaponTemplatesByFactionId(IDbConnection connection, 
                                                                                                   Dictionary<int, BaseSkill> baseSkillMap)
        {
            Dictionary<int, List<RangedWeaponTemplate>> factionWeaponTemplateMap =
                new Dictionary<int, List<RangedWeaponTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM RangedWeaponTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                int location = (int)reader[3];
                int baseSkillId = (int)reader[4];
                float accuracy = (float)reader[5];
                float armorMultiplier = (float)reader[6];
                float woundMultiplier = (float)reader[7];
                float requiredStrength = (float)reader[8];
                float damageMultiplier = (float)reader[9];
                float maxRange = (float)reader[10];
                byte rof = (byte)reader[11];
                ushort ammo = (ushort)reader[12];
                ushort recoil = (ushort)reader[13];
                ushort bulk = (ushort)reader[14];
                bool doesDamageDegrade = (bool)reader[15];

                BaseSkill baseSkill = baseSkillMap[baseSkillId];

                RangedWeaponTemplate weaponTemplate =
                    new RangedWeaponTemplate(id, name, (EquipLocation)location, baseSkill,
                                            accuracy, armorMultiplier, woundMultiplier,
                                            requiredStrength, damageMultiplier, maxRange,
                                            rof, ammo, recoil, bulk, doesDamageDegrade);
                if (!factionWeaponTemplateMap.ContainsKey(factionId))
                {
                    factionWeaponTemplateMap[factionId] = new List<RangedWeaponTemplate>();
                }
                factionWeaponTemplateMap[factionId].Add(weaponTemplate);
            }
            return factionWeaponTemplateMap;
        }
    
        private Tuple<Dictionary<int, List<WeaponSet>>, Dictionary<int, WeaponSet>> GetWeaponSetsById(
            IDbConnection connection, 
            Dictionary<int, List<MeleeWeaponTemplate>> meleeWeaponMap, 
            Dictionary<int, List<RangedWeaponTemplate>> rangedWeaponMap)
        {
            RangedWeaponTemplate primaryRanged, secondaryRanged;
            MeleeWeaponTemplate primaryMelee, secondaryMelee;
            Dictionary<int, WeaponSet> weaponSetMap = new Dictionary<int, WeaponSet>();
            Dictionary<int, List<WeaponSet>> weaponSetsByFactionId = new Dictionary<int, List<WeaponSet>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM WeaponSet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                int? primaryRangedId = (int?)reader[3];
                int? secondaryRangedId = (int?)reader[4];
                int? primaryMeleeId = (int?)reader[5];
                int? secondaryMeleeId = (int?)reader[6];

                if(primaryRangedId != null)
                {
                    primaryRanged = rangedWeaponMap[factionId].First(rw => rw.Id == (int)primaryRangedId);
                }
                else
                {
                    primaryRanged = null;
                }

                if (secondaryRangedId != null)
                {
                    secondaryRanged = rangedWeaponMap[factionId].First(rw => rw.Id == (int)secondaryRangedId);
                }
                else
                {
                    secondaryRanged = null;
                }

                if (primaryMeleeId != null)
                {
                    primaryMelee = meleeWeaponMap[factionId].First(mw => mw.Id == (int)primaryMeleeId);
                }
                else
                {
                    primaryMelee = null;
                }

                if (secondaryMeleeId != null)
                {
                    secondaryMelee = meleeWeaponMap[factionId].First(mw => mw.Id == (int)secondaryMeleeId);
                }
                else
                {
                    secondaryMelee = null;
                }

                WeaponSet weaponSet = new WeaponSet(id, name, primaryRanged, secondaryRanged, 
                                                    primaryMelee, secondaryMelee);
                weaponSetMap[id] = weaponSet;
                if (!weaponSetsByFactionId.ContainsKey(factionId))
                {
                    weaponSetsByFactionId[factionId] = new List<WeaponSet>();
                }
                weaponSetsByFactionId[factionId].Add(weaponSet);
            }
            return new Tuple<Dictionary<int, List<WeaponSet>>, Dictionary<int, WeaponSet>>(weaponSetsByFactionId, weaponSetMap);
        }

        private Dictionary<int, List<int>> GetSquadTemplateWeaponSetIdsBySquadTemplateWeaponOptionId(IDbConnection connection)
        {
            Dictionary<int, List<int>> weaponOptionToWeaponSetMap = new Dictionary<int, List<int>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SquadTemplateWeaponOptionWeaponSet";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int weaponSetId = (int)reader[1];
                int squadTemplateWeaponOption = (int)reader[2];
                if (!weaponOptionToWeaponSetMap.ContainsKey(squadTemplateWeaponOption))
                {
                    weaponOptionToWeaponSetMap[squadTemplateWeaponOption] = new List<int>();
                }
                weaponOptionToWeaponSetMap[squadTemplateWeaponOption].Add(weaponSetId);
            }
            return weaponOptionToWeaponSetMap;
        }

        private Dictionary<int, List<SquadWeaponOption>> GetSquadWeaponOptionsBySquadTemplateId(IDbConnection connection,
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
                int id = (int)reader[0];
                int squadTemplateId = (int)reader[1];
                string name = reader[2].ToString();
                int min = (int)reader[3];
                int max = (int)reader[4];

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
                int squadTemplateId = (int)reader[1];
                int soldierTypeId = (int)reader[2];
                int min = (int)reader[3];
                int max = (int)reader[4];

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
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name= reader[2].ToString();
                int defaultArmorId = (int)reader[3];
                int defaultWeaponSetId = (int)reader[4];
                int squadType = (int)reader[5];

                List<ArmorTemplate> armorList = armorTemplateMap[factionId];
                ArmorTemplate defaultArmor = armorList.First(at => at.Id == defaultArmorId);
                SquadTemplate squadTemplate = new SquadTemplate(id, name, weaponSetMap[defaultWeaponSetId],
                                                                squadWeaponOptionMap[id], defaultArmor,
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

        private Dictionary<int, List<SquadTemplate>> GetSquadTemplatesByUnitTemplateId(IDbConnection connection,
                                                                                       Dictionary<int, SquadTemplate> squadTemplateMap)
        {
            Dictionary<int, List<SquadTemplate>> unitSquadTemplateMap = new Dictionary<int, List<SquadTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM UnitTemplateSquadTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int unitTemplateId = (int)reader[1];
                int squadTemplateId = (int)reader[2];

                if (!unitSquadTemplateMap.ContainsKey(unitTemplateId))
                {
                    unitSquadTemplateMap[unitTemplateId] = new List<SquadTemplate>();
                }
                unitSquadTemplateMap[unitTemplateId].Add(squadTemplateMap[squadTemplateId]);
            }
            return unitSquadTemplateMap;
        }

        private Dictionary<int, List<int>> GetUnitTemplateHierarchy(IDbConnection connection)
        {
            Dictionary<int, List<int>> unitTemplateTree = new Dictionary<int, List<int>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM UnitTemplateTree";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int parentUnitId = (int)reader[1];
                int childUnitId = (int)reader[2];

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
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                bool isTop = (bool)reader[3];
                int hqSquadTemplateId = (int)reader[4];

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

        private Dictionary<int, int[]> GetStanceHitProbabilitiesByHitLocationId(IDbConnection connection)
        {
            Dictionary<int, int[]> hitProbabilityMap = new Dictionary<int, int[]>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM HitLocationStanceSize";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int hitLocationId = (int)reader[1];
                int stance = (int)reader[2];
                int size = (int)reader[3];
                
                if (!hitProbabilityMap.ContainsKey(hitLocationId))
                {
                    hitProbabilityMap[hitLocationId] = new int[3];
                }
                hitProbabilityMap[hitLocationId][stance] = size;
            }
            return hitProbabilityMap;
        }

        private Dictionary<int, AttributeTemplate> GetAttributeTemplates(IDbConnection connection)
        {
            Dictionary<int, AttributeTemplate> attributeTemplateMap =
                new Dictionary<int, AttributeTemplate>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM AttributeTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                float baseValue = (float)reader[1];
                float stdDev = (float)reader[2];
                AttributeTemplate attributeTemplate = new AttributeTemplate
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
                int id = (int)reader[0];
                int baseSkillId = (int)reader[1];
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
                int soldierTemplateId = (int)reader[1];
                int skillTemplateId = (int)reader[2];

                if (!skillTemplateListMap.ContainsKey(soldierTemplateId))
                {
                    skillTemplateListMap[soldierTemplateId] = new List<SkillTemplate>();
                }
                skillTemplateListMap[soldierTemplateId].Add(skillTemplateMap[skillTemplateId]);
            }
            return skillTemplateListMap;
        }


        private Dictionary<int, List<HitLocationTemplate>> GetHitLocationsByBodyId(IDbConnection connection,
                                                                                      Dictionary<int, int[]> stanceProbabilityMap)
        {
            Dictionary<int, List<HitLocationTemplate>> hitLocationTemplateMap =
                new Dictionary<int, List<HitLocationTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM HitLocationTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int bodyId = (int)reader[1];
                string name = reader[2].ToString();
                float naturalArmor = (float)reader[3];
                float woundMultiplier= (float)reader[4];
                int  crippleLevel = (int)reader[5];
                int severLevel = (int)reader[6];
                bool isMotive = (bool)reader[7];
                bool isRanged = (bool)reader[8];
                bool isMelee = (bool)reader[9];
                bool isVital = (bool)reader[10];
                int[] hitProbabilityMap = stanceProbabilityMap[id];
                HitLocationTemplate hitLocationTemplate =
                    new HitLocationTemplate
                    {
                        Id = id,
                        Name = name,
                        NaturalArmor = naturalArmor,
                        WoundMultiplier = woundMultiplier,
                        CrippleWound = (uint)crippleLevel,
                        SeverWound = (uint)severLevel,
                        IsMotive = isMotive,
                        IsRangedWeaponHolder = isRanged,
                        IsMeleeWeaponHolder = isMelee,
                        IsVital = isVital,
                        HitProbabilityMap = hitProbabilityMap
                    };
                if (!hitLocationTemplateMap.ContainsKey(bodyId))
                {
                    hitLocationTemplateMap[bodyId] = new List<HitLocationTemplate>();
                }
                hitLocationTemplateMap[bodyId].Add(hitLocationTemplate);
            }
            return hitLocationTemplateMap;
        }

        private Dictionary<int, List<SoldierTemplate>> GetSoldierTemplatesByFactionId(IDbConnection connection,
                                                                                         Dictionary<int, SoldierType> soldierTypeMap,
                                                                                         Dictionary<int, AttributeTemplate> attributeMap,
                                                                                         Dictionary<int, List<HitLocationTemplate>> hitLocationTemplateMap,
                                                                                         Dictionary<int, List<SkillTemplate>> skillTemplateMap)
        {
            Dictionary<int, List<SoldierTemplate>> soldierTemplateMap = new Dictionary<int, List<SoldierTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM SoldierTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                int bodyId = (int)reader[2];
                int soldierTypeId = (int)reader[3];
                string name = reader[4].ToString();
                int strengthTemplateId = (int)reader[5];
                int dexterityTemplateId = (int)reader[6];
                int constitutionTemplateId = (int)reader[7];
                int intelligenceTemplateId = (int)reader[8];
                int perceptionTemplateId = (int)reader[9];
                int egoTemplateId = (int)reader[10];
                int charismaTemplateId = (int)reader[11];
                int psychicTemplateId = (int)reader[12];
                int attackSpeedTemplateId = (int)reader[13];
                int moveSpeedTemplateId = (int)reader[14];
                int sizeTemplateId = (int)reader[15];
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

        private Dictionary<int, List<BoatTemplate>> GetBoatTemplatesByFactionId(IDbConnection connection)
        {
            Dictionary<int, List<BoatTemplate>> factionTemplateMap =
                new Dictionary<int, List<BoatTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM BoatTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                ushort soldierCap = (ushort)reader[3];
                BoatTemplate boatTemplate = new BoatTemplate(id, name, soldierCap);
                if (!factionTemplateMap.ContainsKey(factionId))
                {
                    factionTemplateMap[factionId] = new List<BoatTemplate>();
                }
                factionTemplateMap[factionId].Add(boatTemplate);
            }
            return factionTemplateMap;
        }

        private Dictionary<int, List<ShipTemplate>> GetShipTemplatesByFactionId(IDbConnection connection)
        {
            Dictionary<int, List<ShipTemplate>> factionTemplateMap =
                new Dictionary<int, List<ShipTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ShipTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();
                ushort soldierCap = (ushort)reader[3];
                ushort boatCap = (ushort)reader[4];
                ushort landerCap = (ushort)reader[5];
                ShipTemplate boatTemplate = new ShipTemplate(id, name, soldierCap, boatCap, landerCap);
                if (!factionTemplateMap.ContainsKey(factionId))
                {
                    factionTemplateMap[factionId] = new List<ShipTemplate>();
                }
                factionTemplateMap[factionId].Add(boatTemplate);
            }
            return factionTemplateMap;
        }

        private Dictionary<int, List<int>> GetFleetShipTemplateLists(IDbConnection connection)
        {
            Dictionary<int, List<int>> fleetToShipMap = new Dictionary<int, List<int>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM FleetTemplateShipTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int fleetId = (int)reader[1];
                int shipId = (int)reader[2];
                if (!fleetToShipMap.ContainsKey(fleetId))
                {
                    fleetToShipMap[fleetId] = new List<int>();
                }
                fleetToShipMap[fleetId].Add(shipId);
            }
            return fleetToShipMap;
        }

        private Dictionary<int, List<FleetTemplate>> GetFleetTemplatesByFactionId(IDbConnection connection,
                                                                                  Dictionary<int, List<ShipTemplate>> factionShipMap,
                                                                                  Dictionary<int, List<int>> fleetShipMap)
        {
            Dictionary<int, List<FleetTemplate>> factionTemplateMap =
                new Dictionary<int, List<FleetTemplate>>();
            IDbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM FleetTemplate";
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader[0];
                int factionId = (int)reader[1];
                string name = reader[2].ToString();

                List<ShipTemplate> baseList = factionShipMap[factionId];
                List<ShipTemplate> fleetShipTemplateList = new List<ShipTemplate>();
                foreach(int shipTemplateId in fleetShipMap[id])
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
            return factionTemplateMap;
        }

        private Color ConvertDatabaseObjectToColor(object obj)
        {
            int colorInt = (int)obj;
            int r = colorInt / 0x01000000;
            int g = (colorInt / 0x00010000) & 0x000000ff;
            int b = (colorInt / 0x00000100) & 0x000000ff;
            int a = colorInt & 0x000000ff;
            return new Color(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }
    }
}
