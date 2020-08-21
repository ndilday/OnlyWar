using System.Collections.Generic;
using UnityEngine;

using Iam.Scripts.Models.Units;
using Assets.Scripts.Models;

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
        public Planet(Vector2 position) { Position = position; }
        public string Name;
        public int StarType;
        public readonly Vector2 Position;
        public PlanetType PlanetType;
        public Fleet LocalFleet;
        public Dictionary<Faction, List<Unit>> GroundUnits;
        public Faction ControllingFaction;
        // how to represent forces on planet?
    }
}