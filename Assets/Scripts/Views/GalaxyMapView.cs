using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Iam.Scripts.Views
{
    public class GalaxyMapView : MonoBehaviour
    {
        public UnityEvent<int, Vector2, Vector2> OnFleetPathDraw;
        public UnityEvent<int> OnFleetPathRemove;
        public UnityEvent<int, Vector2, Vector2> OnFleetDestinationDraw;
        public UnityEvent<int> OnFleetDestinationRemove;

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

        public GalaxyMapView()
        {
            _planetViewMap = new Dictionary<int, Tuple<Transform, SpriteRenderer, TextMesh>>();
            _fleetViewMap = new Dictionary<int, Transform>();
        }

        public void DrawPlanet(int planetId, Vector2 position, string planetName, Color planetColor)
        {
            GameObject star = Instantiate(StarPrefab,
                                Vector2.Scale(position, GameSettings.MapScale),
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

        public void DrawFleetAtLocation(int fleetId, Vector2 location, bool isOffset)
        {
            Vector2 offset = new Vector2(isOffset ? 5.0f : 0f, isOffset ? 5.0f : 0f);
            GameObject fleetSprite = Instantiate(FleetPrefab,
                                        Vector2.Scale(location, GameSettings.MapScale) + offset,
                                        Quaternion.identity,
                                        this.transform);
            _fleetViewMap[fleetId] = fleetSprite.transform;
        }

        public void RemoveFleet(int fleetId)
        {
            Destroy(_fleetViewMap[fleetId].gameObject);
            _fleetViewMap.Remove(fleetId);
        }

        public int? GetFleetIdFromLocation(Vector2 position)
        {
            var result = _fleetViewMap.SingleOrDefault(kvp => kvp.Value.position == (Vector3)position);
            if (result.Equals(default)) return null;
            return result.Key;
        }

        public void SelectFleet(int fleetId)
        {
            // highlight the ship
            _fleetViewMap[fleetId].GetChild(1).gameObject.SetActive(true);
        }

        public void DeselectFleet(int fleetId)
        {
            _fleetViewMap[fleetId].GetChild(1).gameObject.SetActive(false);
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
    }
}