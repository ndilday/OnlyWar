using OnlyWar.Helpers;
using OnlyWar.Helpers.Battle;
using OnlyWar.Models.Planets;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers
{
    class GameController: MonoBehaviour
    {
        public UnityEvent OnTurnStart;
        public UnityEvent<BattleConfiguration> OnBattleStart;
        private readonly UnityEvent OnAllBattlesComplete;
        private readonly Queue<BattleConfiguration> _battleConfigurationQueue;

        [SerializeField]
        private GameSettings GameSettings;

        private int _currentBattlePlanet;

        GameController()
        {
            OnAllBattlesComplete = new UnityEvent();
            _battleConfigurationQueue = new Queue<BattleConfiguration>();
        }

        // Start is called before the first frame update
        void Start()
        {
            OnAllBattlesComplete.AddListener(GameController_OnAllBattlesComplete);
        }

        public void UIController_OnTurnEnd()
        {
            _currentBattlePlanet = -1;
            PrepareBattleList();
            HandleNextBattle();
        }

        public void BattleController_OnBattleComplete()
        {
            HandleNextBattle();
        }

        public void GameController_OnAllBattlesComplete()
        {
            HandlePlanetaryAssaults();
            // if we've scanned through the whole galaxy, battles are done, start a new turn
            OnTurnStart.Invoke();
        }

        private void PrepareBattleList()
        {
            int playerFactionId = GameSettings.Sector.PlayerFaction.Id;
            int defaultFactionId = GameSettings.Sector.DefaultFaction.Id;
            foreach(Planet planet in GameSettings.Sector.Planets.Values)
            {
                IReadOnlyList<BattleConfiguration> battleConfigList =
                    BattleConfigurationBuilder.BuildBattleConfigurations(planet,
                                                                         playerFactionId,
                                                                         defaultFactionId);
                if(battleConfigList != null)
                {
                    foreach(BattleConfiguration battleConfig in battleConfigList)
                    {
                        _battleConfigurationQueue.Enqueue(battleConfig);
                    }
                }
            }
        }

        private void HandleNextBattle()
        {
            if(_battleConfigurationQueue.Count == 0)
            {
                OnAllBattlesComplete.Invoke();
            }
            else
            {
                BattleConfiguration nextBattleConfiguration = 
                    _battleConfigurationQueue.Dequeue();
                // TODO: add some UX to give the player context around this battle
                if (nextBattleConfiguration.Planet.Id != _currentBattlePlanet)
                {
                    // this is a new planet, update accordingly
                    Debug.Log($"Battle breaks out on {nextBattleConfiguration.Planet.Name}!");
                }
                OnBattleStart.Invoke(nextBattleConfiguration);
            }
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
