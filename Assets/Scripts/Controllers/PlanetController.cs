using OnlyWar.Scripts.Helpers;
using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Planets;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Scripts.Controllers
{
    public class PlanetController : MonoBehaviour
    {
        [SerializeField]
        private GameSettings GameSettings;

        public void UIController_OnTurnEnd()
        {
            foreach (Planet planet in GameSettings.Galaxy.Planets)
            {
                EndOfTurnPlanetUpdate(planet);
            }
        }

        private void EndOfTurnPlanetUpdate(Planet planet)
        {
            // increase the population of the planet
            float pdfRatio = ((float)planet.PlanetaryDefenseForces) / planet.Population;
            foreach (PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
            {
                float newPop = 0;
                switch (planetFaction.Faction.GrowthType)
                {
                    case GrowthType.Logistic:
                        newPop = planetFaction.Population * 1.00015f;
                        break;
                    case GrowthType.Conversion:

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
            }

            // see if the planet is called to tithe a regiment
            //tax level / 50
            if (RNG.GetLinearDouble() < planet.TaxLevel / 50f)
            {
                GenerateNewRegiment(planet);
            }
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
