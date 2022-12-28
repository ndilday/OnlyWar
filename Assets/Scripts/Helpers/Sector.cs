using OnlyWar.Builders;
using OnlyWar.Helpers.Database.GameRules;
using OnlyWar.Models;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Equippables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Helpers
{
    public class Sector
    {
        private readonly Dictionary<int, TaskForce> _fleets;
        private readonly Dictionary<int, Planet> _planets;
        private readonly Dictionary<ushort, List<Tuple<ushort, ushort>>> _subsectorPlanetMap;
        private readonly Dictionary<ushort, Tuple<ushort, ushort>> _subsectorCenterMap;
        private readonly List<Character> _characters;
        private readonly IReadOnlyList<Faction> _factions;
        private readonly IReadOnlyDictionary<int, BaseSkill> _baseSkillMap;
        private readonly IReadOnlyList<SkillTemplate> _skillTemplateList;
        private readonly IReadOnlyDictionary<int, List<HitLocationTemplate>> _bodyHitLocationTemplateMap;
        private readonly IReadOnlyDictionary<int, PlanetTemplate> _planetTemplateMap;
        private readonly int _sectorSize;
        public List<Character> Characters { get => _characters; }
        public IReadOnlyDictionary<int, Planet> Planets { get => _planets; }
        public IReadOnlyDictionary<ushort, List<Tuple<ushort, ushort>>> SubsectorPlanetMap { get => _subsectorPlanetMap; }
        public IReadOnlyDictionary<ushort, Tuple<ushort, ushort>> SubsectorCenterMap { get => _subsectorCenterMap; }
        public IReadOnlyDictionary<int, TaskForce> Fleets { get => _fleets; }
        public IReadOnlyList<Faction> Factions { get => _factions; }
        public Faction PlayerFaction { get; }
        public Faction DefaultFaction { get; }
        public IReadOnlyDictionary<int, BaseSkill> BaseSkillMap { get => _baseSkillMap; }
        public IReadOnlyList<SkillTemplate> SkillTemplateList { get => _skillTemplateList; }
        public IReadOnlyDictionary<int, List<HitLocationTemplate>> BodyHitLocationTemplateMap { get => _bodyHitLocationTemplateMap; }
        public IReadOnlyDictionary<int, PlanetTemplate> PlanetTemplateMap { get => _planetTemplateMap; }
        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; }
        public IReadOnlyDictionary<int, WeaponSet> WeaponSets { get; }

        public Sector(int sectorSize)
        {
            var gameBlob = GameRulesDataAccess.Instance.GetData();
            _characters = new List<Character>();
            _factions = gameBlob.Factions;
            _baseSkillMap = gameBlob.BaseSkills;
            _skillTemplateList = gameBlob.SkillTemplates;
            _bodyHitLocationTemplateMap = gameBlob.BodyTemplates;
            _planetTemplateMap = gameBlob.PlanetTemplates;
            RangedWeaponTemplates = gameBlob.RangedWeaponTemplates;
            MeleeWeaponTemplates = gameBlob.MeleeWeaponTemplates;
            WeaponSets = gameBlob.WeaponSets;
            PlayerFaction = _factions.First(f => f.IsPlayerFaction);
            DefaultFaction = _factions.First(f => f.IsDefaultFaction);
            _sectorSize = sectorSize;
            _planets = new Dictionary<int, Planet>();
            _fleets = new Dictionary<int, TaskForce>();
            _subsectorPlanetMap = new Dictionary<ushort, List<Tuple<ushort, ushort>>>();
            _subsectorCenterMap = new Dictionary<ushort, Tuple<ushort, ushort>>();
        }

        public IReadOnlyList<Faction> GetNonPlayerFactions()
        {
            return _factions.Where(f => !f.IsPlayerFaction).ToList();
        }

        public Planet GetPlanet(int planetId)
        {
            return Planets[planetId];
        }

        public Planet GetPlanetByPosition(Vector2 worldPosition)
        {
            return Planets.Values.Where(p => p.Position != null && p.Position == worldPosition).SingleOrDefault();
        }

        public IEnumerable<TaskForce> GetFleetsByPosition(Vector2 worldPosition)
        {
            return Fleets.Values.Where(f => f.Position == worldPosition);
        }

        public void GenerateSector(List<Character> characters, List<Planet> planets, List<TaskForce> fleets)
        {
            _characters.Clear();
            _characters.AddRange(characters);
            
            _planets.Clear();
            foreach(Planet planet in planets)
            {
                _planets[planet.Id] = planet;
            }

            _fleets.Clear();
            foreach (TaskForce fleet in fleets)
            {
                _fleets[fleet.Id] = fleet;
                if (fleet.Planet != null)
                {
                    fleet.Planet.TaskForces.Add(fleet);
                }
            }
        }

        public void GenerateSector(int seed)
        {
            _planets.Clear();
            RNG.Reset(seed);
            ushort currentSubsectorId = 1;

            for(ushort i = 0; i < _sectorSize; i++)
            {
                for (ushort j = 0; j < _sectorSize; j++)
                {
                    double random = RNG.GetLinearDouble();
                    if (random <= 0.05)
                    {
                        Planet planet = GeneratePlanet(new Vector2(i, j));
                        _planets[planet.Id] = planet;

                        // create a subsector with just this planet
                        // we will use Agglomerative Hierarchical Clustering
                        // to combine them into reasonable subsectors
                        _subsectorPlanetMap[currentSubsectorId] = new List<Tuple<ushort, ushort>>();
                        _subsectorPlanetMap[currentSubsectorId].Add(new Tuple<ushort, ushort>(i, j));
                        currentSubsectorId++;

                        if (planet.PlanetFactionMap[planet.ControllingFaction.Id].Leader != null)
                        {
                            Character leader = 
                                planet.PlanetFactionMap[planet.ControllingFaction.Id].Leader;
                            _characters.Add(leader);
                        }
                    }
                }
            }
            // we assume each space on the sector map is approximately 2ly x 2ly
            // so a max diameter of 10 == 20ly
            CombineSubsectors(_subsectorPlanetMap, 10);
            foreach(var coordinates in _subsectorPlanetMap)
            {
                ushort combinedX = (ushort)(coordinates.Value.Sum(t => t.Item1) / coordinates.Value.Count);
                ushort combinedY = (ushort)(coordinates.Value.Sum(t => t.Item2) / coordinates.Value.Count);
                _subsectorCenterMap[coordinates.Key] = new Tuple<ushort, ushort>(combinedX, combinedY);
            }
        }

        public void AddNewFleet(TaskForce newFleet)
        {
            _fleets[newFleet.Id] = newFleet;
            if(newFleet.Planet != null)
            {
                newFleet.Planet.TaskForces.Add(newFleet);
            }
        }

        public void CombineFleets(TaskForce remainingFleet, TaskForce mergingFleet)
        {
            if(mergingFleet.Planet != remainingFleet.Planet 
                || mergingFleet.Position != remainingFleet.Position
                || mergingFleet.Faction.Id != remainingFleet.Faction.Id)
            {
                throw new InvalidOperationException("The two fleets cannot be merged");
            }
            foreach(Ship ship in mergingFleet.Ships)
            {
                remainingFleet.Ships.Add(ship);
                ship.Fleet = remainingFleet;
            }
            mergingFleet.Ships.Clear();
            remainingFleet.Ships.Sort((x, y) => x.Template.Id.CompareTo(y.Template.Id));
            _fleets.Remove(mergingFleet.Id);
            mergingFleet.Planet.TaskForces.Remove(mergingFleet);
        }

        public TaskForce SplitOffNewFleet(TaskForce originalFleet, 
                                      IReadOnlyCollection<Ship> newFleetShipList)
        {
            TaskForce newFleet = new TaskForce(originalFleet.Faction)
            {
                Planet = originalFleet.Planet,
                Position = originalFleet.Position,
                Destination = originalFleet.Destination
            };
            foreach (Ship ship in newFleetShipList)
            {
                originalFleet.Ships.Remove(ship);
                newFleet.Ships.Add(ship);
                ship.Fleet = newFleet;
            }
            if(newFleet.Planet != null)
            {
                newFleet.Planet.TaskForces.Add(newFleet);
            }
            _fleets[newFleet.Id] = newFleet;
            return newFleet;
        }

        public void TakeControlOfPlanet(Planet planet, Faction faction)
        {
            planet.ControllingFaction = faction;
        }

        private Planet GeneratePlanet(Vector2 position)
        {
            // TODO: There should be game start config settings for planet ownership by specific factions
            // TODO: Once genericized, move into planet factory
            double random = RNG.GetLinearDouble();
            Faction controllingFaction, infiltratingFaction;
            if (random <= 0.05)
            {
                controllingFaction = _factions.First(f => f.Name == "Genestealer Cult");
                infiltratingFaction = null;
            }
            else if (random <= 0.25f)
            {
                controllingFaction = _factions.First(f => f.Name == "Tyranids");
                infiltratingFaction = null;
            }
            else
            {
                controllingFaction = DefaultFaction;
                random = RNG.GetLinearDouble();
                infiltratingFaction = random <= 0.1 ? _factions.First(f => f.Name == "Genestealer Cult") : null;
            }

            return PlanetBuilder.Instance.GenerateNewPlanet(_planetTemplateMap, position, controllingFaction, infiltratingFaction);
        }

        private void CombineSubsectors(Dictionary<ushort, List<Tuple<ushort, ushort>>> subsectorPlanetMap,
                                      ushort subsectorMaxDiameter)
        {
            int maxDistanceSquared = subsectorMaxDiameter * subsectorMaxDiameter;
            Dictionary<Tuple<ushort, ushort>, int> subsectorDistanceMap =
                new Dictionary<Tuple<ushort, ushort>, int>();
            Dictionary<ushort, List<ushort>> subsectorPairMap =
                new Dictionary<ushort, List<ushort>>();

            // calculate the distance between each subsector
            foreach (var kvp in subsectorPlanetMap)
            {
                foreach (var kvp2 in subsectorPlanetMap)
                {
                    if (kvp.Key >= kvp2.Key) continue;
                    // find the maximum distance between subsectors
                    int longestPlanetaryDistance =
                        CalculateLongestPlanetaryDistance(kvp.Value, kvp2.Value);
                    // only keep results that could potentially merge
                    if (longestPlanetaryDistance < maxDistanceSquared)
                    {
                        Tuple<ushort, ushort> sectorPairId = new Tuple<ushort, ushort>(kvp.Key, kvp2.Key);
                        subsectorDistanceMap[sectorPairId] = longestPlanetaryDistance;
                        if (!subsectorPairMap.ContainsKey(kvp.Key))
                        {
                            subsectorPairMap[kvp.Key] = new List<ushort>();
                        }
                        if (!subsectorPairMap.ContainsKey(kvp2.Key))
                        {
                            subsectorPairMap[kvp2.Key] = new List<ushort>();
                        }
                        subsectorPairMap[kvp.Key].Add(kvp2.Key);
                        subsectorPairMap[kvp2.Key].Add(kvp.Key);
                    }
                }
            }
            while (subsectorDistanceMap.Count > 0)
            {
                var shortestDistance = subsectorDistanceMap.OrderBy(kvp => kvp.Value).First();

                // add the points from the second subsector to the first, and remove the second subsector
                subsectorPlanetMap[shortestDistance.Key.Item1].AddRange(subsectorPlanetMap[shortestDistance.Key.Item2]);
                subsectorPlanetMap.Remove(shortestDistance.Key.Item2);

                // remove all kvps involving the second subsector
                foreach (ushort otherSubsector in subsectorPairMap[shortestDistance.Key.Item2])
                {
                    // the smaller subsector id is always the first item
                    if (otherSubsector < shortestDistance.Key.Item2)
                    {

                        subsectorDistanceMap.Remove(new Tuple<ushort, ushort>(otherSubsector, shortestDistance.Key.Item2));
                    }
                    else
                    {
                        subsectorDistanceMap.Remove(new Tuple<ushort, ushort>(shortestDistance.Key.Item2, otherSubsector));
                    }
                    subsectorPairMap[otherSubsector].Remove(shortestDistance.Key.Item2);
                }
                subsectorPairMap.Remove(shortestDistance.Key.Item2);

                // recalculate distances for the new combined subsector
                foreach (ushort otherSubsector in subsectorPairMap[shortestDistance.Key.Item1].ToList())
                {
                    Tuple<ushort, ushort> pair;
                    if(otherSubsector < shortestDistance.Key.Item1)
                    {
                        pair = new Tuple<ushort, ushort>(otherSubsector, shortestDistance.Key.Item1);
                    }
                    else if(otherSubsector > shortestDistance.Key.Item1)
                    {
                        pair = new Tuple<ushort, ushort>(shortestDistance.Key.Item1, otherSubsector);
                    }
                    else
                    {
                        continue;
                    }

                    int newDistanceSquared =
                        CalculateLongestPlanetaryDistance(subsectorPlanetMap[pair.Item1], subsectorPlanetMap[pair.Item2]);

                    if (newDistanceSquared > maxDistanceSquared)
                    {
                        // after combination, the other subsector is no longer in range
                        if (subsectorDistanceMap.ContainsKey(pair))
                        {
                            subsectorDistanceMap.Remove(pair);
                            subsectorPairMap[pair.Item1].Remove(pair.Item2);
                            subsectorPairMap[pair.Item2].Remove(pair.Item1);
                        }
                    }
                    else
                    {
                        subsectorDistanceMap[pair] = newDistanceSquared;
                    }
                }
            }
        }

        private int CalculateLongestPlanetaryDistance(List<Tuple<ushort, ushort>> coordinates1, List<Tuple<ushort, ushort>> coordinates2)
        {
            int longestPlanetaryDistance = 0;
            foreach (var coordinate1 in coordinates1)
            {
                foreach (var coordinate2 in coordinates2)
                {
                    int distance = CalculateDistanceSquared(coordinate1, coordinate2);
                    if (distance > longestPlanetaryDistance)
                    {
                        longestPlanetaryDistance = distance;
                    }
                }
            }

            return longestPlanetaryDistance;
        }

        private int CalculateDistanceSquared(Tuple<ushort, ushort> coordinate1, Tuple<ushort, ushort> coordinate2)
        {
            int xDiff = coordinate1.Item1 - coordinate2.Item1;
            int yDiff = coordinate1.Item2 - coordinate2.Item2;
            return (xDiff * xDiff) + (yDiff * yDiff);
        }

    }
}