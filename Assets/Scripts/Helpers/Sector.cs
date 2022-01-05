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
        private readonly Dictionary<int, Fleet> _fleets;
        private readonly Dictionary<int, Planet> _planets;
        private readonly List<Character> _characters;
        private readonly IReadOnlyList<Faction> _factions;
        private readonly IReadOnlyDictionary<int, BaseSkill> _baseSkillMap;
        private readonly IReadOnlyList<SkillTemplate> _skillTemplateList;
        private readonly IReadOnlyDictionary<int, List<HitLocationTemplate>> _bodyHitLocationTemplateMap;
        private readonly IReadOnlyDictionary<int, PlanetTemplate> _planetTemplateMap;
        private readonly int _sectorSize;
        public List<Character> Characters { get => _characters; }
        public IReadOnlyDictionary<int, Planet> Planets { get => _planets; }
        public IReadOnlyDictionary<int, Fleet> Fleets { get => _fleets; }
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
            _fleets = new Dictionary<int, Fleet>();
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

        public IEnumerable<Fleet> GetFleetsByPosition(Vector2 worldPosition)
        {
            return Fleets.Values.Where(f => f.Position == worldPosition);
        }

        public void GenerateSector(List<Character> characters, List<Planet> planets, List<Fleet> fleets)
        {
            _characters.Clear();
            _characters.AddRange(characters);
            
            _planets.Clear();
            foreach(Planet planet in planets)
            {
                _planets[planet.Id] = planet;
            }

            _fleets.Clear();
            foreach (Fleet fleet in fleets)
            {
                _fleets[fleet.Id] = fleet;
                if (fleet.Planet != null)
                {
                    fleet.Planet.Fleets.Add(fleet);
                }
            }
        }

        public void GenerateSector(int seed)
        {
            _planets.Clear();
            RNG.Reset(seed);
            for(int i = 0; i < _sectorSize; i++)
            {
                for (int j = 0; j < _sectorSize; j++)
                {
                    double random = RNG.GetLinearDouble();
                    if (random <= 0.05)
                    {
                        Planet planet = GeneratePlanet(new Vector2(i, j));
                        _planets[planet.Id] = planet;
                        if(planet.PlanetFactionMap[planet.ControllingFaction.Id].Leader != null)
                        {
                            Character leader = 
                                planet.PlanetFactionMap[planet.ControllingFaction.Id].Leader;
                            _characters.Add(leader);
                        }
                    }
                }
            }
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

        public void AddNewFleet(Fleet newFleet)
        {
            _fleets[newFleet.Id] = newFleet;
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
            _fleets[newFleet.Id] = newFleet;
            return newFleet;
        }

        public void TakeControlOfPlanet(Planet planet, Faction faction)
        {
            planet.ControllingFaction = faction;
        }
    }
}