using OnlyWar.Helpers;
using OnlyWar.Models;
using OnlyWar.Models.Planets;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Controllers
{
    public class PlanetController : MonoBehaviour
    {
        [SerializeField]
        private GameSettings GameSettings;

        // TODO: should we switch this to respond to battles complete, instead?
        public void UIController_OnTurnEnd()
        {
            foreach (Planet planet in GameSettings.Galaxy.Planets.Values)
            {
                EndOfTurnPlanetUpdate(planet);
            }
        }

        private void EndOfTurnPlanetUpdate(Planet planet)
        {
            // increase the population of the planet
            float pdfRatio = ((float)planet.PlanetaryDefenseForces) / planet.Population;
            
            EndOfTurnPlanetFactionsUpdate(planet, pdfRatio);

            // see if the planet is called to tithe a regiment
            //tax level / 50
            if (RNG.GetLinearDouble() < planet.TaxLevel / 50f)
            {
                GenerateNewRegiment(planet);
            }
        }

        private void EndOfTurnPlanetFactionsUpdate(Planet planet, float pdfRatio)
        {
            foreach (PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
            {
                float newPop = 0;
                switch (planetFaction.Faction.GrowthType)
                {
                    case GrowthType.Logistic:
                        newPop = planetFaction.Population * 1.00015f;
                        break;
                    case GrowthType.Conversion:
                        newPop = ConvertPopulation(planet, planetFaction, newPop);
                        if (planetFaction.Faction.Id != planet.ControllingFaction.Id &&
                            planet.PlanetFactionMap[planet.ControllingFaction.Id].Leader != null)
                        {
                            // TODO: see if the governor notices the converted population
                        }
                        break;
                    default:
                        newPop = planetFaction.Population;
                        break;
                }
                planetFaction.Population = (int)newPop;
                if (RNG.GetLinearDouble() < newPop % 1)
                {
                    planetFaction.Population++;
                }

                // if the pdf is less than three percent of the population, more people are drafted
                // additionally, secret factions love to infiltrate the PDF
                if (pdfRatio < 0.03f || !planetFaction.IsPublic)
                {
                    planetFaction.PDFMembers += (int)(newPop * 0.05f);
                }
                else if (planetFaction.Faction == planet.ControllingFaction || !planetFaction.IsPublic)
                {
                    planetFaction.PDFMembers += (int)(newPop * 0.03f);
                }

                // see if this faction leader is the sort who'd request aid from the player
                if (planetFaction.Leader != null)
                {
                    EndOfTurnLeaderUpdate(planet, planetFaction);
                }
            }
        }

        private void EndOfTurnLeaderUpdate(Planet planet, PlanetFaction planetFaction)
        {
            // TODO: see if this leader dies
            if(planetFaction.Leader.ActiveRequest != null)
            {
                // see if the request has been fulfilled
                if (planetFaction.Leader.ActiveRequest.IsRequestCompleted())
                {
                    // remove the active request
                    planetFaction.Leader.ActiveRequest = null;
                    // improve leader opinion of player
                    planetFaction.Leader.OpinionOfPlayerForce += 
                        planetFaction.Leader.Appreciation * (1 - planetFaction.Leader.OpinionOfPlayerForce);
                }
                else
                {
                    // decrement the leader's opinion based on the unfulfilled request
                    // the average governor will drop 0.01 opinion per week.
                    planetFaction.Leader.OpinionOfPlayerForce -= (0.005f / planetFaction.Leader.Patience);
                    // TODO: some notion of canceling a request?
                }
            }
            else if(planetFaction.Leader.OpinionOfPlayerForce > 0)
            {
                GenerateRequests(planet, planetFaction);
            }
        }

        private void GenerateRequests(Planet planet, PlanetFaction planetFaction)
        {
            bool found = false;
            bool evidenceFound = false;
            if (planet.PlanetFactionMap.Count > 1)
            {
                // there are other factions on planet
                foreach (PlanetFaction planetOtherFaction in planet.PlanetFactionMap.Values)
                {

                    // make sure this is a different faction and that there isn't already a request about it
                    if (planetOtherFaction.Faction.Id != planetFaction.Faction.Id)
                    {
                        if (!planetOtherFaction.IsPublic)
                        {
                            // see if the leader detects this faction
                            float popRatio = ((float)planetOtherFaction.Population) / ((float)planet.Population);
                            float chance = popRatio * planetFaction.Leader.Investigation;
                            double roll = RNG.GetLinearDouble();
                            if (roll < chance)
                            {
                                found = true;
                                evidenceFound = roll < (chance / 10.0);
                                break;
                            }
                        }
                        else
                        {
                            found = true;
                            evidenceFound = true;
                            break;
                        }
                    }
                }
            }
            if (!found)
            {
                // no real threats, see if the leader is paranoid enough to see a threat anyway
                double roll = RNG.GetLinearDouble();
                if (roll < planetFaction.Leader.Paranoia)
                {
                    found = true;
                    evidenceFound = roll < (planetFaction.Leader.Paranoia / 10.0);
                }
            }

            if (found)
            {
                // determine if the leader wants to turn this finding into a request
                float chance = planetFaction.Leader.Neediness * planetFaction.Leader.OpinionOfPlayerForce;
                double roll = RNG.GetLinearDouble();
                if (roll < chance)
                {
                    // generate a new request
                    IRequest request = 
                        RequestFactory.Instance.GenerateNewRequest(planet, planetFaction.Leader, 
                                                                   GameSettings, GameSettings.Date);
                    planetFaction.Leader.ActiveRequest = request;
                    GameSettings.Chapter.Requests.Add(request);
                }
            }
        }

        private float ConvertPopulation(Planet planet, PlanetFaction planetFaction, float newPop)
        {
            PlanetFaction defaultFaction = planet.PlanetFactionMap
                                                                         .Values
                                                                         .First(pf => pf.Faction.IsDefaultFaction);
            // converting factions always convert one new member per week
            if (defaultFaction?.Population > 0)
            {
                defaultFaction.Population--;
                planetFaction.Population++;
                float pdfChance = (float)(defaultFaction.PDFMembers) / defaultFaction.Population;
                if (RNG.GetLinearDouble() < pdfChance)
                {
                    defaultFaction.PDFMembers--;
                    planetFaction.PDFMembers++;
                }
                if (planetFaction.Population > 100)
                {
                    // at larger sizes, converting factions
                    // also grow organically 
                    // at a much faster rate than a normal population
                    newPop = planetFaction.Population * 1.002f;
                }
                // if the converting population is larger than
                // the non-converted PDF force, they start their revolt
                if (newPop > (planet.PlanetaryDefenseForces - planetFaction.PDFMembers)
                    && !planet.IsUnderAssault)
                {
                    planetFaction.IsPublic = true;
                    planet.IsUnderAssault = true;
                }
            }

            return newPop;
        }

        private void GenerateNewRegiment(Planet planet)
        {
            // TODO: Right now, the new regiment just disappears
            // TODO: should we track faction membership in IG regiments, so that as they move, 
            // that faction grows where they settle?
            foreach (PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
            {
                int troopsLost = planetFaction.PDFMembers / 10;
                planetFaction.PDFMembers -= troopsLost;
                planetFaction.Population -= troopsLost;
            }
        }

    }
}
