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
    public class Galaxy
    {
        private readonly List<Fleet> _fleets;
        private readonly List<Planet> _planets;
        private readonly List<Character> _characters;
        private readonly IReadOnlyList<Faction> _factions;
        private readonly IReadOnlyDictionary<int, BaseSkill> _baseSkillMap;
        private readonly IReadOnlyList<SkillTemplate> _skillTemplateList;
        private readonly IReadOnlyDictionary<int, List<HitLocationTemplate>> _bodyHitLocationTemplateMap;
        private readonly IReadOnlyDictionary<int, PlanetTemplate> _planetTemplateMap;
        private readonly int _galaxySize;
        public List<Character> Characters { get => _characters; }
        public IReadOnlyList<Planet> Planets { get => _planets; }
        public IReadOnlyList<Fleet> Fleets { get => _fleets; }
        public IReadOnlyList<Faction> Factions { get => _factions; }
        public Faction PlayerFaction { get; }
        public IReadOnlyDictionary<int, BaseSkill> BaseSkillMap { get => _baseSkillMap; }
        public IReadOnlyList<SkillTemplate> SkillTemplateList { get => _skillTemplateList; }
        public IReadOnlyDictionary<int, List<HitLocationTemplate>> BodyHitLocationTemplateMap { get => _bodyHitLocationTemplateMap; }
        public IReadOnlyDictionary<int, PlanetTemplate> PlanetTemplateMap { get => _planetTemplateMap; }
        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; }
        public IReadOnlyDictionary<int, WeaponSet> WeaponSets { get; }

        public Galaxy(int galaxySize)
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

        public void GenerateGalaxy(List<Character> characters, List<Planet> planets, List<Fleet> fleets)
        {
            _characters.Clear();
            _characters.AddRange(characters);
            _planets.Clear();
            _planets.AddRange(planets);
            _fleets.Clear();
            _fleets.AddRange(fleets);
            foreach(Fleet fleet in fleets)
            {
                if(fleet.Planet != null)
                {
                    fleet.Planet.Fleets.Add(fleet);
                }
            }
        }

        public void GenerateGalaxy(int seed)
        {
            _planets.Clear();
            RNG.Reset(seed);
            for(int i = 0; i < _galaxySize; i++)
            {
                for (int j = 0; j < _galaxySize; j++)
                {
                    double random = RNG.GetLinearDouble();
                    if (random <= 0.05)
                    {
                        Planet planet = GeneratePlanet(new Vector2(i, j));
                        _planets.Add(planet);
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
                ship.Fleet = remainingFleet;
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
                ship.Fleet = newFleet;
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
                controllingFaction = Factions.First(f => f.IsDefaultFaction);
                random = RNG.GetLinearDouble();
                infiltratingFaction = random <= 0.1 ? _factions.First(f => f.Name == "Genestealer Cult") : null;
            }

            return PlanetFactory.Instance.GenerateNewPlanet(_planetTemplateMap, position, controllingFaction, infiltratingFaction);
        }
    }
}