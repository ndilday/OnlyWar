using OnlyWar.Models;
using OnlyWar.Models.Planets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Helpers
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

        public Planet GenerateNewPlanet(IReadOnlyDictionary<int, PlanetTemplate> planetTemplateMap, 
                                        Vector2 position, Faction controllingFaction, Faction infiltratingFaction)
        {
            PlanetTemplate template = DeterminePlanetTemplate(planetTemplateMap);
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
            if(infiltratingFaction != null)
            {
                double infiltrationRate = RNG.GetLinearDouble() / 2.0;
                PlanetFaction infiltration = new PlanetFaction(infiltratingFaction);
                infiltration.PlayerReputation = 0;
                infiltration.IsPublic = false;
                infiltration.Population = (int)(popToDistribute * infiltrationRate);
                infiltration.PDFMembers = (int)(infiltration.Population / 33);
                planet.PlanetFactionMap[infiltratingFaction.Id] = infiltration;
            }

            PlanetFaction planetFaction = new PlanetFaction(controllingFaction);
            planetFaction.PlayerReputation = 0;
            planetFaction.IsPublic = true;
            planetFaction.Population = popToDistribute;
            planetFaction.PDFMembers = popToDistribute / 33;
            planet.PlanetFactionMap[controllingFaction.Id] = planetFaction;
            planet.ControllingFaction = controllingFaction;
            return planet;
        }

        private PlanetTemplate DeterminePlanetTemplate(IReadOnlyDictionary<int, PlanetTemplate> templates)
        {
            // we're using the "lottery ball" approach to randomness here, where each point 
            // of probability for each available body party 
            // defines the size of the random linear distribution
            int max = templates.Values.Sum(pt => pt.Probability);
            int roll = RNG.GetIntBelowMax(0, max);
            foreach (PlanetTemplate template in templates.Values)
            {
                if (roll < template.Probability)
                {
                    return template;
                }
                else
                {
                    // this is basically an easy iterative way to figure out which body part on the "chart" the roll matches
                    roll -= template.Probability;
                }
            }
            // this should never happen
            throw new InvalidOperationException("Could not determine a planet template");
        }
    }
}
