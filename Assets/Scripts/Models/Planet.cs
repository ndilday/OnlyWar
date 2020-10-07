﻿using System.Collections.Generic;
using UnityEngine;

using Iam.Scripts.Models.Factions;
using Iam.Scripts.Models.Fleets;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Models
{
    public enum PlanetType
    {
        Agri,
        Daemon,
        Dead,
        Death,
        Desert,
        Feral,
        Feudal,
        Forge,
        Fortress,
        Frontier,
        Hive,
        Ice,
        Mining,
        Penal,
        Pleasure,
        Ocean,
        Shrine
    }

    public class Planet
    {
        public int Id;
        public string Name;
        public readonly Vector2 Position;
        public PlanetType PlanetType;
        public List<Fleet> Fleets;
        public Dictionary<int, List<Unit>> FactionGroundUnitListMap;
        public Faction ControllingFaction;
        
        public Planet(int id, string name, Vector2 position, PlanetType type)
        {
            Id = id;
            Name = name;
            Position = position;
            PlanetType = type;
            Fleets = new List<Fleet>();
            FactionGroundUnitListMap = new Dictionary<int, List<Unit>>();
        }

    }
}