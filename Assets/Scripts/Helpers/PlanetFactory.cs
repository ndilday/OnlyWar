using OnlyWar.Scripts.Helpers;
using OnlyWar.Scripts.Models.Planets;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    class PlanetFactory
    {
        private PlanetFactory() 
        {
            _usedPlanetNameIndexes = new HashSet<int>();
        }
        private static PlanetFactory _instance;
        public static PlanetFactory Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new PlanetFactory();
                }
                return _instance;
            }
        }

        private static HashSet<int> _usedPlanetNameIndexes;

        private static int _nextId = 0;

        public Planet GenerateNewPlanet(PlanetTemplate template, Vector2 position)
        {
            int nameIndex = RNG.GetIntBelowMax(0, TempPlanetList.PlanetNames.Length);
            while(_usedPlanetNameIndexes.Contains(nameIndex))
            {
                nameIndex = RNG.GetIntBelowMax(0, TempPlanetList.PlanetNames.Length);
            }
            _usedPlanetNameIndexes.Add(nameIndex);
            int importance = (int)(template.ImportanceRange.BaseValue)
                + (int)(RNG.NextGaussianDouble() * template.ImportanceRange.StandardDeviation);
            int taxLevel = 
                RNG.GetIntBelowMax(template.TaxRange.MinValue, template.TaxRange.MaxValue + 1);
            Planet planet = new Planet(_nextId, TempPlanetList.PlanetNames[nameIndex], 
                                       position, template, importance, taxLevel);
            _nextId++;
            planet.ImperialPopulation = (int)(template.PopulationRange.BaseValue)
                + (int)(Mathf.Pow(10, (float)RNG.NextGaussianDouble()) * template.PopulationRange.StandardDeviation);
            planet.PlayerReputation = 0;
            planet.PlanetaryDefenseForces = planet.ImperialPopulation * 10;
            return planet;
        }
    }
}
