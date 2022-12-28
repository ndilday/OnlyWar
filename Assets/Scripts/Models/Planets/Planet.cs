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
        public readonly int Size;
        public bool IsUnderAssault { get; set; }

        public List<TaskForce> TaskForces;
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

        public Planet(int id, string name, Vector2 position, int size, 
            PlanetTemplate template, int importance, int taxLevel)
        {
            Id = id;
            Name = name;
            Position = position;
            Size = size;
            Template = template;
            Importance = importance;
            TaxLevel = taxLevel;
            TaskForces = new List<TaskForce>();
            PlanetFactionMap = new Dictionary<int, PlanetFaction>();
        }

    }
}