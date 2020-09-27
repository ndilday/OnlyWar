using System.Collections.Generic;
using UnityEngine;

using Iam.Scripts.Models.Factions;

namespace Iam.Scripts.Models.Fleets
{
    public class Fleet
    {
        public int Id { get; set; }
        public FactionTemplate Faction { get; }
        public Vector2 Position { get; set; }
        public Planet Destination { get; set; }
        public Planet Planet { get; set; }
        List<Ship> Ships { get; }

        public Fleet(int id, FactionTemplate faction, int templateId)
        {
            Id = id;
            Faction = faction;
            Ships = new List<Ship>();
            int i = Id * 1000;
            foreach(ShipTemplate shipTemplate in faction.FleetTemplates[templateId].Ships)
            {
                Ships.Add(new Ship(i++, shipTemplate));
            }
        }
    }
}