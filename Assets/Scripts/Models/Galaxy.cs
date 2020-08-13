using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Iam.Scripts.Models
{
    public class Galaxy
    {
        const int GALAXY_WIDTH = 30;
        public List<Planet> Planets;
        public List<Fleet> Fleets;

        public Galaxy()
        {
            Planets = new List<Planet>();
            Fleets = new List<Fleet>();
        }

        public Planet GetPlanet(int planetId)
        {
            return Planets[planetId];
        }

        public Planet GetPlanetByPosition(Vector2 worldPosition)
        {
            return Planets.Where(p => p.Position != null && p.Position == worldPosition).SingleOrDefault();
        }

        public Fleet GetFleetByPosition(Vector2 worldPosition)
        {
            return Fleets.Where(f => f.Position != null && f.Position == worldPosition).SingleOrDefault();
        }

        public void GenerateGalaxy(int seed)
        {
            Random.InitState(seed);
            for(int i = 0; i < GALAXY_WIDTH; i++)
            {
                for (int j = 0; j < GALAXY_WIDTH; j++)
                {
                    if (Random.Range(0.0f, 1.0f) <= 0.05f)
                    {
                        Planet p = new Planet(new Vector2(i, j));
                        if (Planets.Count < 60)
                        {
                            p.Name = TempPlanetList.Planets[Planets.Count].Name;
                            p.PlanetType = TempPlanetList.Planets[Planets.Count].Type;
                        }
                        else
                        {
                            p.Name = i.ToString() + j.ToString();
                            p.PlanetType = PlanetType.Death;
                        }
                        Planets.Add(p);
                    }
                }
            }
            AddFleet();
        }

        public void AddFleet()
        {
            int startingPlanet = Planets.Count / 2;
            Fleet fleet = new Fleet();
            fleet.Planet = Planets[startingPlanet];
            fleet.Destination = null;
            fleet.Position = fleet.Planet.Position;
            Planets[startingPlanet].LocalFleet = fleet;
            Fleets.Add(fleet);
        }
    }
}