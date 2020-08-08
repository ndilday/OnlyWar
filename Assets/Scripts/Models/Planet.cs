using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iam.Scripts.Models
{
    public class PlanetGraphic
    {

    }
    public class Planet
    {
        public Planet(Vector2 position) { Position = position; }
        public string Name;
        public int StarType;
        public readonly Vector2 Position;
        public PlanetType PlanetType;
        public PlanetGraphic PlanetGraphic;
        public Color TextColor;
        public Color PlanetColor;
        public Fleet LocalFleet;
        // how to represent forces on planet?
    }
}