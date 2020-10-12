using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using OnlyWar.Scripts.Helpers.Database.GameRules;
using OnlyWar.Scripts.Models.Fleets;
using OnlyWar.Scripts.Models;
using System;
using OnlyWar.Scripts.Models.Soldiers;

namespace OnlyWar.Scripts.Helpers
{
    public class Galaxy
    {
        private readonly List<Fleet> _fleets;
        private readonly List<Planet> _planets;
        private readonly List<Faction> _factions;
        private readonly Dictionary<int, BaseSkill> _baseSkillMap;
        private readonly Dictionary<int, List<HitLocationTemplate>> _bodyHitLocationTemplateMap;
        private readonly int _galaxySize;
        public IReadOnlyList<Planet> Planets { get => _planets; }
        public IReadOnlyList<Fleet> Fleets { get => _fleets; }
        public IReadOnlyList<Faction> Factions { get => _factions; }
        public Faction PlayerFaction { get; }
        public IReadOnlyDictionary<int, BaseSkill> BaseSkillMap { get => _baseSkillMap; }
        public IReadOnlyDictionary<int, List<HitLocationTemplate>> BodyHitLocationTemplateMap { get => _bodyHitLocationTemplateMap; }

        public Galaxy(int galaxySize)
        {
            var gameBlob = GameRulesDataAccess.Instance.GetData();
            _factions = gameBlob.Factions;
            _baseSkillMap = gameBlob.BaseSkills;
            _bodyHitLocationTemplateMap = gameBlob.BodyTemplates;
            PlayerFaction = _factions.First(f => f.IsPlayerFaction);
            _galaxySize = galaxySize;
            _planets = new List<Planet>();
            _fleets = new List<Fleet>();
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
            return Planets.Where(p => p.Position != null && p.Position == worldPosition).SingleOrDefault();
        }

        public IEnumerable<Fleet> GetFleetsByPosition(Vector2 worldPosition)
        {
            return Fleets.Where(f => f.Position == worldPosition);
        }

        public void GenerateGalaxy(List<Planet> planets, List<Fleet> fleets)
        {
            _planets.Clear();
            _planets.AddRange(planets);
            _fleets.Clear();
            _fleets.AddRange(fleets);
        }

        public void GenerateGalaxy(int seed)
        {
            _planets.Clear();
            UnityEngine.Random.InitState(seed);
            int id = 0;
            for(int i = 0; i < _galaxySize; i++)
            {
                for (int j = 0; j < _galaxySize; j++)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.05f)
                    {
                        string name;
                        PlanetType type;

                        if (id < TempPlanetList.Planets.Length)
                        {
                            name = TempPlanetList.Planets[id].Name;
                            type = TempPlanetList.Planets[id].Type;
                        }
                        else
                        {
                            name = $"{i},{j}";
                            type = PlanetType.Death;
                        }
                        
                        Planet p = new Planet(id, name, new Vector2(i, j), type);
                        
                        if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1f)
                        {
                            p.ControllingFaction = _factions.First(f => f.Name == "Tyranids");
                        }

                        _planets.Add(p);
                        id++;
                    }
                }
            }
        }

        public void AddNewFleet(Fleet newFleet)
        {
            _fleets.Add(newFleet);
            if(newFleet.Planet != null)
            {
                newFleet.Planet.Fleets.Add(newFleet);
            }
        }

        public void CombineFleets(Fleet remainingFleet, Fleet mergingFleet)
        {
            if(mergingFleet.Planet != remainingFleet.Planet 
                || mergingFleet.Position != remainingFleet.Position
                || mergingFleet.Faction.Id != remainingFleet.Faction.Id)
            {
                throw new InvalidOperationException("The two fleets are not in the same place");
            }
            foreach(Ship ship in mergingFleet.Ships)
            {
                remainingFleet.Ships.Add(ship);
            }
            mergingFleet.Ships.Clear();
            remainingFleet.Ships.Sort((x, y) => x.Template.Id.CompareTo(y.Template.Id));
            _fleets.Remove(mergingFleet);
            mergingFleet.Planet.Fleets.Remove(mergingFleet);
        }

        public Fleet SplitOffNewFleet(Fleet originalFleet, 
                                      IReadOnlyCollection<Ship> newFleetShipList)
        {
            Fleet newFleet = new Fleet(originalFleet.Faction)
            {
                Planet = originalFleet.Planet,
                Position = originalFleet.Position,
                Destination = originalFleet.Destination
            };
            foreach (Ship ship in newFleetShipList)
            {
                originalFleet.Ships.Remove(ship);
                newFleet.Ships.Add(ship);
            }
            if(newFleet.Planet != null)
            {
                newFleet.Planet.Fleets.Add(newFleet);
            }
            _fleets.Add(newFleet);
            return newFleet;
        }

        public void TakeControlOfPlanet(Planet planet, Faction faction)
        {
            planet.ControllingFaction = faction;
        }
    }
}