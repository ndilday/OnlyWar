﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models.Factions;
using Iam.Scripts.Models;

namespace Iam.Scripts.Helpers
{
    public class Galaxy
    {
        private readonly int _galaxySize;
        public readonly List<Planet> Planets;
        public readonly List<Fleet> Fleets;

        public Galaxy(int galaxySize)
        {
            _galaxySize = galaxySize;
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
            UnityEngine.Random.InitState(seed);
            for(int i = 0; i < _galaxySize; i++)
            {
                for (int j = 0; j < _galaxySize; j++)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) <= 0.05f)
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
                        if(UnityEngine.Random.Range(0.0f, 1.0f) <= 0.1f)
                        {
                            p.ControllingFaction = TempFactions.Instance.TyranidFaction;
                        }
                        p.Id = Planets.Count;
                        Planets.Add(p);
                    }
                }
            }
            AddFleet();
        }

        public void AddFleet()
        {
            int startingPlanet = Planets.Count / 2;
            Fleet fleet = new Fleet
            {
                Planet = Planets[startingPlanet],
                Destination = null
            };
            fleet.Position = fleet.Planet.Position;
            Planets[startingPlanet].LocalFleet = fleet;
            Planets[startingPlanet].ControllingFaction = TempFactions.Instance.SpaceMarineFaction;
            Fleets.Add(fleet);
        }
    }
}