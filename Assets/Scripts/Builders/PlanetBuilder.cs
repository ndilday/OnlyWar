using OnlyWar.Helpers;
using OnlyWar.Models;
using OnlyWar.Models.Planets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Builders
{
    class PlanetBuilder
    {
        private PlanetBuilder() 
        {
            _usedPlanetNameIndexes = new HashSet<int>();
        }
        private static PlanetBuilder _instance;
        public static PlanetBuilder Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new PlanetBuilder();
                }
                return _instance;
            }
        }

        private static HashSet<int> _usedPlanetNameIndexes;

        private static int _nextId = 0;
        private static int _leaderId = 0;
        public Planet GenerateNewPlanet(IReadOnlyDictionary<int, PlanetTemplate> planetTemplateMap, 
                                        Vector2 position, Faction controllingFaction, Faction infiltratingFaction)
        {
            PlanetTemplate template = DeterminePlanetTemplate(planetTemplateMap);
            Faction leaderFaction = controllingFaction;
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
            // for now, we're hardcoding all planets to be size 10
            Planet planet = new Planet(_nextId, TempPlanetList.PlanetNames[nameIndex], 
                                       position, 10, template, importance, taxLevel);
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
                if(RNG.GetLinearDouble() < infiltrationRate / 2)
                {
                    leaderFaction = infiltratingFaction;
                }
            }

            PlanetFaction planetFaction = new PlanetFaction(controllingFaction);
            planetFaction.PlayerReputation = 0;
            planetFaction.IsPublic = true;
            planetFaction.Population = popToDistribute;
            planetFaction.PDFMembers = popToDistribute / 33;
            // for now, all planets start completely in the control of a single faction
            planetFaction.PlanetaryControl = 10;
            planet.PlanetFactionMap[controllingFaction.Id] = planetFaction;
            planet.ControllingFaction = controllingFaction;

            if(controllingFaction.IsDefaultFaction)
            {
                planetFaction.Leader = CharacterBuilder.GenerateCharacter(_leaderId, leaderFaction);
                if(!leaderFaction.IsDefaultFaction)
                {
                    // if the planetary leader is a member of a GC,
                    // they'll never reuest player aid 
                    planetFaction.Leader.OpinionOfPlayerForce = -1;
                }
                _leaderId++;
            }
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
