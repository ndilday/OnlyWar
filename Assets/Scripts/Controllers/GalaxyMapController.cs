﻿using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Fleets;
using OnlyWar.Scripts.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace OnlyWar.Scripts.Controllers
{
    // responsible for holding main Galaxy data object, triggering file save/loads, and generating new galaxy
    public class GalaxyMapController : MonoBehaviour
    {
        public UnityEvent OnTurnStart;
        public UnityEvent OnEscapeKey;
        public UnityEvent<Planet> OnPlanetSelected;
        public UnityEvent<Planet> OnBattleStart;
        
        [SerializeField]
        private GameSettings GameSettings;
        [SerializeField]
        private GalaxyMapView Map;
        [SerializeField]
        private UnitTreeView FleetView;

        private int? _selectedFleetId;
        private int _planetBattleStartedId;
        private List<Ship> _selectedShips;

        // Start is called before the first frame update
        void Start()
        {
            _selectedShips = new List<Ship>();
            GameSettings.OpposingFactions = GameSettings.Galaxy.GetNonPlayerFactions();
            for (int i = 0; i < GameSettings.Galaxy.Planets.Count; i++)
            {
                Color color;
                Planet planet = GameSettings.Galaxy.Planets[i];
                if (planet.ControllingFaction != null)
                {
                    color = planet.ControllingFaction.Color;
                    if (GameSettings.Galaxy.Planets[i].ControllingFaction == GameSettings.Galaxy.PlayerFaction)
                    {
                        GameSettings.ChapterPlanetId = GameSettings.Galaxy.Planets[i].Id;
                    }
                }
                else
                {
                    color = Color.white;
                }

                Map.CreatePlanet(i, GameSettings.Galaxy.Planets[i].Position, GameSettings.Galaxy.Planets[i].Name, color);
            }
            foreach (Fleet fleet in GameSettings.Galaxy.Fleets)
            {
                Map.CreateFleet(fleet.Id, fleet.Position, false);
            }
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

        public void UIController_OnTurnEnd()
        {
            // move fleets
            foreach(Fleet fleet in GameSettings.Galaxy.Fleets)
            {
                if (fleet.Destination != null)
                {
                    Map.RemoveFleetDestination(fleet.Id);
                    //Map.RemoveFleet(fleet.Id);

                    // if the fleet has a destination, we need to move the fleet
                    // determine distance of line between two points
                    float distance = Vector2.Distance(fleet.Destination.Position, fleet.Position);
                    if (distance <= 1)
                    {
                        // the journey is done!
                        fleet.Planet = fleet.Destination;
                        fleet.Destination = null;
                        fleet.Position = fleet.Planet.Position;
                        var mergeFleet = fleet.Planet.Fleets.FirstOrDefault(f => f.Destination == null);
                        if (mergeFleet != null)
                        {
                            GameSettings.Galaxy.CombineFleets(mergeFleet, fleet);
                            Map.RemoveFleet(fleet.Id);
                        }
                        else
                        {
                            fleet.Planet.Fleets.Add(fleet);
                            Map.MoveFleet(fleet.Id, fleet.Position, false);
                        }
                    }
                    else
                    {
                        fleet.Planet = null;
                        // calculate one unit of movement along line between current position and destination
                        Vector2 path = (fleet.Destination.Position - fleet.Position);
                        path.Normalize();
                        fleet.Position += path;
                        Map.MoveFleet(fleet.Id, fleet.Position, true);
                        Map.DrawFleetDestination(fleet.Id, fleet.Destination.Position);
                    }
                }
            }
            // TODO: Update resources
            _planetBattleStartedId = -1;
            Map.gameObject.SetActive(false);
            HandleBattles();

        }

        public void BattleController_OnBattleComplete()
        {
            HandleBattles();
        }

        public void FleetView_OnShipSelected(int shipId)
        {
            Fleet fleet = GameSettings.Galaxy.Fleets[(int)_selectedFleetId];
            Ship ship = fleet.Ships.First(s => s.Id == shipId);
            if (!_selectedShips.Contains(ship))
            {
                _selectedShips.Add(ship);
                FleetView.UpdateUnitBadge(shipId, Badge.CHECK);
            }
            else
            {
                _selectedShips.Remove(ship);
                FleetView.UpdateUnitBadge(shipId, Badge.NORMAL);
            }
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
            if (_selectedShips.Count > 0)
            {
                Fleet fleet = GameSettings.Galaxy.Fleets[(int)_selectedFleetId];

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
                            Planet planet = GameSettings.Galaxy.Planets[(int)planetId];
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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Mouse was clicked -- is it on a star?
            RaycastHit2D hitInfo = GetMouseHit();
            
            if (hitInfo.collider == null)
            {
                // we clicked on empty space, unselect any selected fleets/planets
                if (_selectedFleetId != null)
                {
                    Map.SelectFleet((int)_selectedFleetId, false);
                    _selectedFleetId = null;
                    _selectedShips.Clear();
                    FleetView.ClearTree();
                    FleetView.gameObject.SetActive(false);
                }
            }
            else
            {
                // hit an object
                if (hitInfo.collider.name == "FleetSprite")
                {
                    HandleFleetClick(hitInfo);
                }
                else if (hitInfo.collider.name == "StarSprite")
                {
                    // if we have a selected fleet that is not en route, the click becomes their new target system
                    if (_selectedFleetId != null && GameSettings.Galaxy.Fleets[(int)_selectedFleetId].Planet != null)
                    {
                        HandleFleetDestinationClick(hitInfo);
                    }
                    else if (_selectedFleetId == null)
                    {
                        int? planetId = Map.GetPlanetIdFromPosition(hitInfo.collider.transform.position);
                        if (planetId == null) throw new NullReferenceException("Clicked planet does not exist");
                        //if no fleet is selected, this is a planet selection action
                        OnPlanetSelected.Invoke(GameSettings.Galaxy.Planets[(int)planetId]);
                    }
                }
            }
        }

        private void HandleFleetDestinationClick(RaycastHit2D hitInfo)
        {
            Fleet fleet = GameSettings.Galaxy.Fleets[(int)_selectedFleetId];
            // this is the selected fleet's new target system
            int? destinationPlanetId = Map.GetPlanetIdFromPosition(hitInfo.collider.transform.position);
            if (destinationPlanetId != null)
            {
                Planet destination = GameSettings.Galaxy.Planets[(int)destinationPlanetId];
                if (fleet.Ships.Count != _selectedShips.Count)
                {
                    // this set of ships is a subset of the original fleet, 
                    // we'll need to break them out on their own
                    fleet = GameSettings.Galaxy.SplitOffNewFleet(fleet, _selectedShips);
                }

                foreach(Fleet adjacentFleet in fleet.Planet.Fleets)
                {
                    // if there's another fleet at this planet doing what this fleet
                    // is now doing, just merge them
                    if(adjacentFleet.Id != fleet.Id)
                    {
                        if(adjacentFleet.Destination == null
                        && fleet.Planet == destination)
                        {
                            GameSettings.Galaxy.CombineFleets(adjacentFleet, fleet);
                            DeselectFleet();
                            Map.RemoveFleet(fleet.Id);
                            return;
                        }
                        if(adjacentFleet.Destination != null
                            && adjacentFleet.Destination == destination)
                        {
                            GameSettings.Galaxy.CombineFleets(adjacentFleet, fleet);
                            DeselectFleet();
                            Map.RemoveFleet(fleet.Id);
                            return;
                        }
                    }
                }
                    
                if (fleet.Planet == destination && fleet.Destination != null)
                {
                    // click is on the fleet's current location, remove fleet destination
                    // and return fleet to static position
                    Map.RemoveFleetDestination((int)_selectedFleetId);
                    if (fleet.Id == _selectedFleetId)
                    {
                        Map.MoveFleet(fleet.Id, destination.Position, false);
                    }
                    else
                    {
                        Map.CreateFleet(fleet.Id, destination.Position, false);
                    }
                    DeselectFleet();
                    fleet.Destination = null;
                }
                else
                {
                    // the click was not at the current planet
                    if(fleet.Destination == null)
                    {
                        // need to move the fleet
                        if (fleet.Id == _selectedFleetId)
                        {
                            Map.MoveFleet(fleet.Id, fleet.Position, true);
                        }
                        else
                        {
                            Map.CreateFleet(fleet.Id, fleet.Position, true);
                        }
                    }
                    else
                    {
                        // need to remove the current destination line
                        Map.RemoveFleetDestination((int)_selectedFleetId);
                    }
                    // need to remove the current destination line
                    Map.DrawFleetDestination(fleet.Id, destination.Position);
                    
                    // update fleet model
                    fleet.Destination = GameSettings.Galaxy.Planets[(int)destinationPlanetId];
                    DeselectFleet();
                }
            }
        }

        private void DeselectFleet()
        {
            Map.RemoveFleetPath((int)_selectedFleetId);
            Map.SelectFleet((int)_selectedFleetId, false);
            FleetView.gameObject.SetActive(false);
            _selectedShips.Clear();
            _selectedFleetId = null;
        }

        private void HandleFleetClick(RaycastHit2D hitInfo)
        {
            // clicked on a fleet

            // deselect currently selected fleet
            if (_selectedFleetId != null)
            {
                Map.SelectFleet((int)_selectedFleetId, false);
            }
            // select new fleet
            _selectedFleetId = Map.GetFleetIdFromLocation(hitInfo.collider.transform.position);
            if (_selectedFleetId != null)
            {
                Map.SelectFleet((int)_selectedFleetId, true);
            }
            // display fleet overlay
            FleetView.gameObject.SetActive(true);
            PopulateFleetView((int)_selectedFleetId);
        }

        private void PopulateFleetView(int fleetId)
        {
            Fleet fleet = GameSettings.Galaxy.Fleets[fleetId];
            FleetView.ClearTree();
            foreach (Ship ship in fleet.Ships)
            {
                FleetView.AddLeafSquad(ship.Id, GetShipText(ship), Color.white);
            }
        }

        private string GetShipText(Ship ship)
        {
            return $"{ship.Name}\n{ship.LoadedSquads.Count} Squads, {ship.LoadedSoldierCount}/{ship.Template.SoldierCapacity} Capacity";
        }

        private void HandleBattles()
        {
            int i = _planetBattleStartedId + 1;
            while (i < GameSettings.Galaxy.Planets.Count)
            {
                Planet planet = GameSettings.Galaxy.GetPlanet(i);
                if (planet.FactionSquadListMap.Count > 1)
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
            Map.gameObject.SetActive(true);
            OnTurnStart.Invoke();
        }
    }
}