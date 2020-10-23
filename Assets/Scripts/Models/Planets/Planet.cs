using System.Collections.Generic;
using UnityEngine;

using OnlyWar.Scripts.Models.Fleets;
using OnlyWar.Scripts.Models.Squads;

namespace OnlyWar.Scripts.Models.Planets
{
    public class Planet
    {
        public readonly int Id;
        public readonly string Name;
        public readonly Vector2 Position;
        public readonly PlanetTemplate Template;
        public int ImperialPopulation;
        public readonly long Importance;
        public readonly int TaxLevel;

        public List<Fleet> Fleets;
        public Dictionary<int, List<Squad>> FactionSquadListMap;
        public Faction ControllingFaction;
        
        public Planet(int id, string name, Vector2 position, PlanetTemplate template,
            long importance, int taxLevel)
        {
            Id = id;
            Name = name;
            Position = position;
            Template = template;
            Importance = importance;
            TaxLevel = taxLevel;
            Fleets = new List<Fleet>();
            FactionSquadListMap = new Dictionary<int, List<Squad>>();
        }

    }
}