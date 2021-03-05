using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace OnlyWar.Scripts.Views
{
    public class GalaxyMapView : MonoBehaviour
    {
        public UnityEvent<int, Vector2, Vector2> OnFleetPathDraw;
        public UnityEvent<int> OnFleetPathRemove;
        public UnityEvent<int, Vector2, Vector2> OnFleetDestinationDraw;
        public UnityEvent<int> OnFleetDestinationRemove;
        public UnityEvent<int, Vector2> OnFleetOriginAdjust;

        [SerializeField]
        private GameObject StarPrefab;
        [SerializeField]
        private GameObject FleetPrefab;
        [SerializeField]
        private LayerMask StarLayerMask;
        [SerializeField]
        private LayerMask FleetLayerMask;
        [SerializeField]
        private GameSettings GameSettings;
        
        private readonly Dictionary<int, Tuple<Transform, SpriteRenderer, TextMesh>> _planetViewMap;
        private readonly Dictionary<int, Transform> _fleetViewMap;
        private readonly HashSet<Vector2> _planetPositions;
        private readonly Dictionary<Vector2, List<int>> _movingFleetsAtLocationMap;

        public GalaxyMapView()
        {
            _planetViewMap = new Dictionary<int, Tuple<Transform, SpriteRenderer, TextMesh>>();
            _fleetViewMap = new Dictionary<int, Transform>();
            _planetPositions = new HashSet<Vector2>();
            _movingFleetsAtLocationMap = new Dictionary<Vector2, List<int>>();
        }

        public void CreatePlanet(int planetId, Vector2 position, string planetName, Color planetColor)
        {
            Vector2 realPosition = Vector2.Scale(position, GameSettings.MapScale);
            GameObject star = Instantiate(StarPrefab,
                                realPosition,
                                Quaternion.identity,
                                this.transform);
            SpriteRenderer planetRenderer = star.GetComponentInChildren<SpriteRenderer>();
            TextMesh textMesh = star.GetComponentInChildren<TextMesh>();
            planetRenderer.color = planetColor;
            textMesh.color = planetColor;
            textMesh.text = planetName;
            _planetViewMap[planetId] = new Tuple<Transform, SpriteRenderer, TextMesh>(
                                            star.transform,
                                            planetRenderer,
                                            textMesh);
            _planetPositions.Add(realPosition);
            // add color shading of star based on planet type
        }

        public void UpdatePlanetColor(int planetId, Color newColor)
        {
            var tuple = _planetViewMap[planetId];
            tuple.Item2.color = newColor;
            tuple.Item3.color = newColor;
        }

        public int? GetPlanetIdFromPosition(Vector2 position)
        {
            // assumes only one fleet at a given position
            var result = _planetViewMap.SingleOrDefault(kvp => kvp.Value.Item1.position == (Vector3)position);
            if (result.Equals(default)) return null;
            return result.Key;
        }

        public void CreateFleet(int fleetId, Vector2 location, bool isMoving)
        {
            if (_fleetViewMap.ContainsKey(fleetId))
            {
                throw new ArgumentException($"Fleet {fleetId} already exists.");
            }
            Vector2 realLocation = Vector2.Scale(location, GameSettings.MapScale);
            bool atPlanet = _planetPositions.Contains(realLocation);
            Vector2 offset;

            if(!isMoving && atPlanet)
            {
                // we're current expecting only a single stationary fleet at a planet at a time
                // in the style of MOO, stationary fleets go on the right
                offset = new Vector2(5.0f, 5.0f);
            }
            else if(isMoving && atPlanet)
            {
                // this is our complicated case
                // see if any other fleets are also moving in this location
                if(_movingFleetsAtLocationMap.ContainsKey(realLocation))
                {
                    // there are other fleets here
                    _movingFleetsAtLocationMap[realLocation].Add(fleetId);
                    offset = new Vector2(-5f, 10 - (5 * _movingFleetsAtLocationMap[realLocation].Count));
                }
                else
                {
                    _movingFleetsAtLocationMap[realLocation] = new List<int>
                    {
                        fleetId
                    };
                    offset = new Vector2(-5f, 5f);
                }
                
            }
            else
            {
                // if the ship is not at a planet, it doesn't need to be offset.
                // this has a minor chance of moving fleet overlap;
                // since you can't change the course of moving fleets, anyway, 
                // I'm okay with this
                offset = new Vector2(0, 0);
            }

            GameObject fleetSprite = Instantiate(FleetPrefab,
                                        realLocation + offset,
                                        Quaternion.identity,
                                        this.transform);
            _fleetViewMap[fleetId] = fleetSprite.transform;
        }

        public void MoveFleet(int fleetId, Vector2 location, bool isMoving)
        {
            if (!_fleetViewMap.ContainsKey(fleetId))
            {
                throw new ArgumentException($"Fleet {fleetId} does not exist");
            }
            
            // need to move the existing sprite, rather than creating a new one
            Vector2 realLocation = Vector2.Scale(location, GameSettings.MapScale);
            Vector2 offset;

            if(!isMoving)
            {
                // the only time isMoving should be false is when orbiting a planet
                // so confirm that location is a planet, and then place to the right
                bool atPlanet = _planetPositions.Contains(realLocation);
                if (!atPlanet)
                {
                    throw new InvalidOperationException("Fleet is stationary but not at a planet");
                }
                // we're current expecting only a single stationary fleet at a planet at a time
                // in the style of MOO, stationary fleets go on the right
                offset = new Vector2(5.0f, 5.0f);
                _fleetViewMap[fleetId].position = realLocation + offset;

                // see if this fleet is in the moving fleet list for this location
                if (_movingFleetsAtLocationMap.ContainsKey(realLocation))
                {
                    AdjustMovingFleets(fleetId, realLocation);
                }
            }
            // the next two clauses should only happen when an orbiting fleet
            // goes into motion
            else
            {
                if (!_movingFleetsAtLocationMap.ContainsKey(realLocation))
                {
                    _movingFleetsAtLocationMap[realLocation] = new List<int>
                    {
                        fleetId
                    };
                }
                else
                {
                    _movingFleetsAtLocationMap[realLocation].Add(fleetId);
                }
                offset = new Vector2(-5, 10 - (5 * _movingFleetsAtLocationMap[realLocation].Count));
                _fleetViewMap[fleetId].position = realLocation + offset;
            }
        }

        public void RemoveFleet(int fleetId)
        {
            if (_fleetViewMap.ContainsKey(fleetId))
            {
                Vector2 location = _fleetViewMap[fleetId].position;
                Destroy(_fleetViewMap[fleetId].gameObject);
                _fleetViewMap.Remove(fleetId);
            }
            RemoveFleetDestination(fleetId);
            var entry = 
                _movingFleetsAtLocationMap.FirstOrDefault(kvp => kvp.Value.Contains(fleetId));
            
            if (entry.Value != null)
            {
                AdjustMovingFleets(fleetId, entry.Key);
            }
        }

        public int? GetFleetIdFromLocation(Vector2 position)
        {
            var result = _fleetViewMap.SingleOrDefault(kvp => kvp.Value.position == (Vector3)position);
            if (result.Equals(default)) return null;
            return result.Key;
        }

        public void SelectFleet(int fleetId, bool isSelected)
        {
            // highlight the ship
            _fleetViewMap[fleetId].GetChild(1).gameObject.SetActive(isSelected);
        }

        public void DrawFleetPath(int fleetId, Vector2 endpoint)
        {
            //convert 
            OnFleetPathDraw.Invoke(fleetId, _fleetViewMap[fleetId].position, Vector2.Scale(endpoint, GameSettings.MapScale));
        }

        public void RemoveFleetPath(int fleetId)
        {
            OnFleetPathRemove.Invoke(fleetId);
        }

        public void DrawFleetDestination(int fleetId, Vector2 endpoint)
        {
            OnFleetDestinationDraw.Invoke(fleetId, _fleetViewMap[fleetId].position, Vector2.Scale(endpoint, GameSettings.MapScale));
        }

        public void RemoveFleetDestination(int fleetId)
        {
            OnFleetDestinationRemove.Invoke(fleetId);
        }

        public void CenterCameraOnPlanet(int planetId)
        {
            Vector3 newPosition = _planetViewMap[planetId].Item1.position;
            newPosition.z = -100;
            float screenHeightInUnits = Camera.main.orthographicSize * 2.0f;
            float unitsPerPixel = screenHeightInUnits / Screen.height;
            float screenWidthInUnits = screenHeightInUnits * Screen.width / Screen.height;
            float mapTop = GameSettings.GalaxySize * GameSettings.MapScale.y;
            float mapRight = GameSettings.GalaxySize * GameSettings.MapScale.x;
            // assume for now that the world is 450x450
            float minX = (screenWidthInUnits / 2.0f) - (40.0f * unitsPerPixel);
            float maxX = mapRight + (40.0f * unitsPerPixel) - (screenWidthInUnits / 2.0f);
            float minY = (screenHeightInUnits / 2.0f) - (90.0f * unitsPerPixel);
            float maxY = mapTop + (100.0f * unitsPerPixel) - (screenHeightInUnits / 2.0f);
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
            Camera.main.transform.position = newPosition;
        }

        private void AdjustMovingFleets(int fleetId, Vector2 realLocation)
        {
            var localFleetList = _movingFleetsAtLocationMap[realLocation];
            if (localFleetList.Contains(fleetId))
            {
                int index = localFleetList.IndexOf(fleetId);
                for (int i = index + 1; i < localFleetList.Count; i++)
                {
                    int adjustFleetId = localFleetList[i];
                    // move this fleet position up five units
                    Transform fleetTransform = _fleetViewMap[adjustFleetId];
                    Vector3 position = fleetTransform.position;
                    position.y += 5;
                    fleetTransform.position = position;
                    OnFleetOriginAdjust.Invoke(adjustFleetId, position);
                }
                localFleetList.RemoveAt(index);
            }
        }
    }
}