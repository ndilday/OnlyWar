using System.Collections.Generic;
using UnityEngine;

namespace Iam.Scripts.Models.Fleets
{
    public class Fleet
    {
        public Vector2 Position { get; set; }
        public Planet Destination { get; set; }
        public Planet Planet { get; set; }
        List<Ship> Ships { get; set; }
    }
}