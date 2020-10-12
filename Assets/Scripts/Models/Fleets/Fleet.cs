using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OnlyWar.Scripts.Models;

namespace OnlyWar.Scripts.Models.Fleets
{
    public class Fleet
    {
        private static int _nextFleetId = 0;
        public int Id { get; set; }
        public Faction Faction { get; }
        public Vector2 Position { get; set; }
        public Planet Destination { get; set; }
        public Planet Planet { get; set; }
        public List<Ship> Ships { get; }

        public Fleet(int id, Faction faction, Vector2 position, 
                     Planet location, Planet destination, List<Ship> ships)
        {
            Id = id;
            Faction = faction;
            Position = position;
            Planet = location;
            Destination = destination;
            Ships = ships;
            foreach(Ship ship in ships)
            {
                ship.Fleet = this;
            }
        }

        public Fleet(Faction faction, FleetTemplate template) : this(faction)
        {
            int i = Id * 1000;
            BoatTemplate boatTemplate = faction.BoatTemplates.First().Value;
            foreach(ShipTemplate shipTemplate in template.Ships)
            {
                Ship newShip = new Ship(i, $"{shipTemplate.ClassName}-{i}", shipTemplate, boatTemplate)
                {
                    Fleet = this
                };
                Ships.Add(newShip);
                i++;
            }
        }

        public Fleet(Faction faction)
        {
            Id = _nextFleetId++;
            Faction = faction;
            Ships = new List<Ship>();
        }
    }
}