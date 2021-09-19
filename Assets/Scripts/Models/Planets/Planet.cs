using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OnlyWar.Models.Fleets;
using OnlyWar.Models.Squads;

namespace OnlyWar.Models.Planets
{
    public class Planet
    {
        public readonly int Id;
        public readonly string Name;
        public readonly Vector2 Position;
        public readonly PlanetTemplate Template;
        public readonly int Importance;
        public readonly int TaxLevel;
        public bool IsUnderAssault { get; set; }

        public List<Fleet> Fleets;
        public readonly Dictionary<int, List<Squad>> FactionSquadListMap;
        public readonly Dictionary<int, PlanetFaction> PlanetFactionMap;
        public Faction ControllingFaction;
        
        // planetary population is in thousands
        public long Population
        {
            get
            {
                return PlanetFactionMap.Sum(pfm => pfm.Value.Population);
            }
        }

        // I suspect I'm going to change my mind regularly on the scale for this value
        // for now, let's be simple, and let it be headcount
        public int PlanetaryDefenseForces
        {
            get
            {
                return PlanetFactionMap.Sum(pfm => pfm.Value.PDFMembers);
            }
        }

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
            PlanetFactionMap = new Dictionary<int, PlanetFaction>();
        }

    }
}