using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;
using System;

namespace Iam.Scripts.Controllers
{
    // responsible for holding main Galaxy data object, triggering file save/loads, and generating new galaxy
    public class GalaxyController : MonoBehaviour
    {
        public UnityEvent OnTurnStart;
        public UnityEvent OnEscapeKey;
        public UnityEvent<Planet> OnPlanetSelected;
        public UnityEvent<Planet> OnBattleStart;
        
        [SerializeField]
        private GameSettings GameSettings;
        [SerializeField]
        private GalaxyMapView Map;

        private Galaxy _galaxy;
        private int? _selectedFleetId;
        private int _planetBattleStartedId;

        // Start is called before the first frame update
        void Start()
        {
            _galaxy = new Galaxy(GameSettings.GalaxySize);
            Generate();
        }

        // Update is called once per frame
        void Update()
        {
            HandleKeyboardInput();
            HandleMouseMove();
            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseLeftClick();
            }
        }

        public void Generate()
        {
            Debug.Log("UniverseManager::Generate -- Generating a new Galaxy");
            _galaxy.GenerateGalaxy(1);
            // populate visuals on map
            for (int i = 0; i < _galaxy.Planets.Count; i++)
            {
                Color color;
                Planet planet = _galaxy.Planets[i];
                if(planet.ControllingFaction != null)
                {
                    color = planet.ControllingFaction.Color;
                    if (_galaxy.Planets[i].ControllingFaction.Name == "Space Marines")
                    {
                        GameSettings.ChapterPlanetId = _galaxy.Planets[i].Id;
                    }
                }
                else
                {
                    color = Color.white;
                }
                
                Map.DrawPlanet(i, _galaxy.Planets[i].Position, _galaxy.Planets[i].Name, color);
            }
            for (int i = 0; i < _galaxy.Fleets.Count; i++)
            {
                Map.DrawFleetAtLocation(i, _galaxy.Fleets[i].Position, true);
            }
        }

        public void ChapterController_OnChapterCreated()
        {
            // For now, put the chapter on their home planet
            Planet planet = _galaxy.GetPlanet(GameSettings.ChapterPlanetId);
            planet.FactionGroundUnitListMap = new Dictionary<int, List<Unit>>
            {
                [TempFactions.Instance.SpaceMarines.Id] = new List<Unit>
                {
                    GameSettings.Chapter.OrderOfBattle
                },
                [TempFactions.Instance.Tyranids.Id] = new List<Unit>
                {
                    TempTyranidArmyGenerator.GenerateTyranidArmy()
                }
            };
        }

        public void EndTurn_Clicked()
        {
            // move fleets
            for (int i = 0; i < _galaxy.Fleets.Count; i++)
            {
                Fleet fleet = _galaxy.Fleets[i];
                if (fleet.Destination != null)
                {
                    Map.RemoveFleetDestination(i);
                    Map.RemoveFleet(i);

                    // if the fleet has a destination, we need to move the fleet
                    // determine distance of line between two points
                    float distance = Vector2.Distance(fleet.Destination.Position, fleet.Position);
                    if (distance <= 1)
                    {
                        // the journey is done!
                        fleet.Planet = fleet.Destination;
                        fleet.Destination = null;
                        fleet.Position = fleet.Planet.Position;
                        Map.DrawFleetAtLocation(i, fleet.Position, true);
                    }
                    else
                    {
                        fleet.Planet = null;
                        // calculate one unit of movement along line between current position and destination
                        Vector2 path = (fleet.Destination.Position - fleet.Position);
                        path.Normalize();
                        fleet.Position += path;
                        Map.DrawFleetAtLocation(i, fleet.Position, false);
                        Map.DrawFleetDestination(i, fleet.Destination.Position);
                    }
                }
            }
            // TODO: Update resources
            _planetBattleStartedId = -1;
            HandleBattles();

        }

        public void BattleController_OnBattleComplete()
        {
            HandleBattles();
        }

        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscapeKey.Invoke();
            }
        }

        private void HandleMouseMove()
        {
            // check to see if there's a selected fleet
            if (_selectedFleetId != null)
            {
                Fleet fleet = _galaxy.Fleets[(int)_selectedFleetId];

                // if the fleet has no planet, it's en route, and can't change that route
                if (fleet.Planet != null)
                {
                    RaycastHit2D hitInfo = GetMouseHit();
                    if (hitInfo.collider != null && hitInfo.collider.name == "StarSprite")
                    {
                        // if the hover planet is the current location of the fleet, make sure it has no path drawn
                        int? planetId = Map.GetPlanetIdFromPosition(hitInfo.collider.transform.position);
                        if (planetId != null)
                        {
                            Planet planet = _galaxy.Planets[(int)planetId];
                            if (fleet.Planet == planet)
                            {
                                Map.RemoveFleetPath((int)_selectedFleetId);
                            }
                            else if (fleet.Destination == null || fleet.Destination != planet)
                            {
                                // draw a path from the fleet to the planet we're hovering over
                                // TODO: Are we worried about the efficiency of deleting and redrawing the same line over and over when the mouse is sitting on a planet?
                                Map.DrawFleetPath((int)_selectedFleetId, planet.Position);
                            }
                        }
                    }
                    else if (hitInfo.collider == null && fleet.Destination == null)
                    {
                        // not pointing at a planet, and the drawn path isn't permanent
                        // TODO: will need a real fleet key once we have multiple fleets
                        Map.RemoveFleetPath((int)_selectedFleetId);
                    }
                }
            }
        }

        private static RaycastHit2D GetMouseHit()
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            return hitInfo;
        }

        private void HandleMouseLeftClick()
        {
            // Mouse was clicked -- is it on a star?
            // TODO: Ignore clicks if over a UI element
            RaycastHit2D hitInfo = GetMouseHit();
            //if(Physics.Raycast(ray, out hitInfo, StarLayerMask))
            if (hitInfo.collider == null)
            {
                // we clicked on empty space, unselect any selected fleets/planets
                if (_selectedFleetId != null)
                {
                    Map.DeselectFleet((int)_selectedFleetId);
                    _selectedFleetId = null;
                }
            }
            else
            {
                // hit an object
                Debug.Log("Clicked on " + hitInfo.collider.name);
                if (hitInfo.collider.name == "FleetSprite")
                {
                    HandleFleetClick(hitInfo);
                }
                else if (hitInfo.collider.name == "StarSprite")
                {
                    // if we have a selected fleet that is not en route, the click becomes their new target system
                    if (_selectedFleetId != null && _galaxy.Fleets[(int)_selectedFleetId].Planet != null)
                    {
                        HandleFleetDestinationClick(hitInfo);
                    }
                    if (_selectedFleetId == null)
                    {
                        int? planetId = Map.GetPlanetIdFromPosition(hitInfo.collider.transform.position);
                        if (planetId == null) throw new NullReferenceException("Clicked planet does not exist");
                        //if no fleet is selected, this is a planet selection action
                        OnPlanetSelected.Invoke(_galaxy.Planets[(int)planetId]);
                    }
                }
            }
        }

        private void HandleFleetDestinationClick(RaycastHit2D hitInfo)
        {
            Fleet fleet = _galaxy.Fleets[(int)_selectedFleetId];
            // this is the selected fleet's new target system
            int? destinationPlanetId = Map.GetPlanetIdFromPosition(hitInfo.collider.transform.position);
            if (destinationPlanetId != null)
            {
                Planet planet = _galaxy.Planets[(int)destinationPlanetId];
                if (fleet.Planet == planet && fleet.Destination != null)
                {
                    // click is on the fleet's current location, remove fleet destination
                    Map.RemoveFleetDestination((int)_selectedFleetId);
                    fleet.Destination = null;
                }
                else
                {
                    // if we're worried about efficiency, we could remove the extra fleet draw in cases where they've clicked on their current destination
                    // redraw the path in a different color to represent a set course
                    Map.RemoveFleetPath((int)_selectedFleetId);
                    Map.DrawFleetDestination((int)_selectedFleetId, planet.Position);
                    // update fleet model
                    _galaxy.Fleets[(int)_selectedFleetId].Destination = _galaxy.Planets[(int)destinationPlanetId];
                }
            }
        }

        private void HandleFleetClick(RaycastHit2D hitInfo)
        {
            // clicked on a fleet

            // deselect currently selected fleet
            if (_selectedFleetId != null)
            {
                Map.DeselectFleet((int)_selectedFleetId);
            }
            // select new fleet
            _selectedFleetId = Map.GetFleetIdFromLocation(hitInfo.collider.transform.position);
            if (_selectedFleetId != null)
            {
                Map.SelectFleet((int)_selectedFleetId);
            }
        }

        private void HandleBattles()
        {
            int i = _planetBattleStartedId + 1;
            while (i < _galaxy.Planets.Count)
            {
                Planet planet = _galaxy.GetPlanet(i);
                if (planet.FactionGroundUnitListMap?.Keys.Count > 1)
                {
                    // a battle breaks out on this planet
                    Debug.Log("Battle breaks out on " + planet.Name);
                    _planetBattleStartedId = i;
                    OnBattleStart.Invoke(planet);
                    return;
                }
                i++;
            }
            // if we've scanned through the whole galaxy, battles are done, start a new turn
            OnTurnStart.Invoke();
        }
    }
}