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
        public readonly int Importance;
        public readonly int TaxLevel;
        public int ImperialPopulation;
        public float PlayerReputation;
        // I suspect I'm going to change my mind regularly on the scale for this value
        // for now, let's be simple, and let it be headcount
        public int PlanetaryDefenseForces;

        public List<Fleet> Fleets;
        public Dictionary<int, List<Squad>> FactionSquadListMap;
        public Faction ControllingFaction;
        
        public Planet(int id, string name, Vector2 position, PlanetTemplate template,
            int importance, int taxLevel)
        {
            Id = id;
            Name = name;
            Position = position;
            Template = template;
            Importance = importance;
            TaxLevel = taxLevel;
            Fleets = new List<Fleet>();
            FactionSquadListMap = new Dictionary<int, List<Squad>>();
            PlayerReputation = 0;
        }

    }
}