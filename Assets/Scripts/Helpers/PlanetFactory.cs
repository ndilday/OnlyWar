using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Planets;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyWar.Scripts.Helpers
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

        public Planet GenerateNewPlanet(PlanetTemplate template, Vector2 position, 
                                        Faction defaultFaction, Faction infiltratingFaction)
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

            int popToDistribute = (int)(template.PopulationRange.BaseValue)
                + (int)(Mathf.Pow(10, (float)RNG.NextGaussianDouble()) * template.PopulationRange.StandardDeviation);
            // determine if this planet starts with a genestealer cult in place
            // TODO: make this configurable
            double odds = RNG.GetLinearDouble();
            if(odds <= 0.02)
            {
                PlanetFaction infiltration = new PlanetFaction(infiltratingFaction);
                infiltration.PlayerReputation = 0;
                infiltration.IsPublic = false;
                infiltration.Population = (int)(popToDistribute * odds);
                infiltration.PDFMembers = (int)(infiltration.Population / 33);
                planet.PlanetFactionMap[infiltratingFaction.Id] = infiltration;

            }

            PlanetFaction planetFaction = new PlanetFaction(defaultFaction);
            planetFaction.PlayerReputation = 0;
            planetFaction.IsPublic = true;
            planetFaction.Population = popToDistribute;
            planetFaction.PDFMembers = popToDistribute / 33;
            planet.PlanetFactionMap[defaultFaction.Id] = planetFaction;
            return planet;
        }
    }
}
