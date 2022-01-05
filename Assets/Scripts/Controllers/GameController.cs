using OnlyWar.Helpers;
using OnlyWar.Helpers.Battle;
using OnlyWar.Models.Planets;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Controllers
{
    class GameController: MonoBehaviour
    {
        public UnityEvent OnTurnStart;
        public UnityEvent<BattleConfiguration> OnBattleStart;
        private readonly UnityEvent OnAllBattlesComplete;

        [SerializeField]
        private GameSettings GameSettings;

        private int _planetBattleStartedId;

        GameController()
        {
            OnAllBattlesComplete = new UnityEvent();
        }
        // Start is called before the first frame update
        void Start()
        {
            OnAllBattlesComplete.AddListener(GameController_OnAllBattlesComplete);
        }

        public void UIController_OnTurnEnd()
        {
            _planetBattleStartedId = -1;
            HandleBattles();

        }

        public void BattleController_OnBattleComplete()
        {
            HandleBattles();
        }

        public void GameController_OnAllBattlesComplete()
        {
            HandlePlanetaryAssaults();
            // if we've scanned through the whole galaxy, battles are done, start a new turn
            OnTurnStart.Invoke();
        }

        private void HandleBattles()
        {
            int i = _planetBattleStartedId + 1;
            while (i < GameSettings.Sector.Planets.Count)
            {
                Planet planet = GameSettings.Sector.GetPlanet(i);
                BattleConfiguration battleConfig =
                    BattleConfigurationBuilder.BuildBattleConfiguration(planet);
                if (battleConfig != null)
                {
                    Debug.Log("Battle breaks out on " + planet.Name);
                    _planetBattleStartedId = i;
                    OnBattleStart.Invoke(battleConfig);
                    return;
                }
                i++;
            }
            OnAllBattlesComplete.Invoke();
        }

        private void HandlePlanetaryAssaults()
        {
            foreach (Planet planet in GameSettings.Sector.Planets.Values)
            {
                if (planet.IsUnderAssault)
                {
                    PlanetFaction controllingForce = planet.PlanetFactionMap[planet.ControllingFaction.Id];
                    foreach (PlanetFaction planetFaction in planet.PlanetFactionMap.Values)
                    {
                        if (planetFaction != controllingForce && planetFaction.IsPublic)
                        {
                            // this is a revolting force
                            long attackPower = planetFaction.Population * 3 / 4;
                            long defensePower = controllingForce.PDFMembers;
                            // revolting PDF members count triple for their ability to wreck defensive forces
                            attackPower += planetFaction.PDFMembers * 2;
                            double attackMultiplier = (RNG.GetLinearDouble() / 25.0) + 0.01;
                            double defenseMultiplier = (RNG.GetLinearDouble() / 25.0) + 0.01;

                            if (planetFaction.PDFMembers > 0)
                            {
                                // having PDF members means it's the first round of revolt, triple defensive casualties
                                attackMultiplier *= 3;
                                planetFaction.PDFMembers = 0;
                            }
                            int defendCasualties = defensePower == 0 ?
                                (int)(attackPower * attackMultiplier * 1000) :
                                (int)(attackPower * attackMultiplier / defensePower);
                            int attackCasualties = (int)(defensePower * defenseMultiplier / attackPower);
                            planetFaction.Population -= attackCasualties;
                            if (planetFaction.Population <= 100)
                            {
                                planet.IsUnderAssault = false;
                                planetFaction.IsPublic = false;
                            }
                            controllingForce.PDFMembers -= defendCasualties;
                            controllingForce.Population -= defendCasualties;
                            if (controllingForce.PDFMembers <= 0)
                            {
                                controllingForce.Population += controllingForce.PDFMembers;
                                controllingForce.PDFMembers = 0;
                                planet.ControllingFaction = planetFaction.Faction;
                            }
                        }
                    }
                }
            }
        }
    }
}
