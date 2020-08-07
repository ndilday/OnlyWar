using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iam.Scripts.Models
{
    public class Galaxy
    {
        const int GALAXY_WIDTH = 30;
        public List<Planet> Planets;

        public Galaxy()
        {
            Planets = new List<Planet>();
        }

        public Planet GetPlanet(int planetId)
        {
            return Planets[planetId];
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
        }
    }
}