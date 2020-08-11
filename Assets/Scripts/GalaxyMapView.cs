using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Iam.Scripts
{
    public class GalaxyMapView : MonoBehaviour
    {
        public UnityEvent<int, Vector2, Vector2> FleetPathDraw;
        public UnityEvent<int> FleetPathRemove;
        public UnityEvent<int, Vector2, Vector2> FleetDestinationDraw;
        public UnityEvent<int> FleetDestinationRemove;

        public GameObject StarPrefab;
        public GameObject FleetPrefab;
        public LayerMask StarLayerMask;
        public LayerMask FleetLayerMask;
        public GameSettings GameSettings;
        Dictionary<int, Transform> _planetViewMap;
        Dictionary<int, Transform> _fleetViewMap;

        public GalaxyMapView()
        {
            _planetViewMap = new Dictionary<int, Transform>();
            _fleetViewMap = new Dictionary<int, Transform>();
        }

        public void DrawPlanet(int planetId, Vector2 position, string planetName)
        {
            GameObject star = Instantiate(StarPrefab,
                                Vector2.Scale(position, GameSettings.MapScale),
                                Quaternion.identity,
                                this.transform);
            TextMesh textMesh = star.GetComponentInChildren<TextMesh>();
            textMesh.text = planetName;
            _planetViewMap[planetId] = star.transform;
            // add color shading of star based on planet type
        }

        public int? GetPlanetIdFromPosition(Vector2 position)
        {
            // assumes only one fleet at a given position
            var result = _planetViewMap.SingleOrDefault(kvp => kvp.Value.position == (Vector3)position);
            if (result.Equals(default)) return null;
            return result.Key;
        }

        public void SelectPlanet(int planetId)
        {

        }

        public void DeselectPlanet(int planetId)
        {

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
            Object.Destroy(_fleetViewMap[fleetId].gameObject);
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
            FleetPathDraw.Invoke(fleetId, _fleetViewMap[fleetId].position, Vector2.Scale(endpoint, GameSettings.MapScale));
        }

        public void RemoveFleetPath(int fleetId)
        {
            FleetPathRemove.Invoke(fleetId);
        }

        public void DrawFleetDestination(int fleetId, Vector2 endpoint)
        {
            FleetDestinationDraw.Invoke(fleetId, _fleetViewMap[fleetId].position, Vector2.Scale(endpoint, GameSettings.MapScale));
        }

        public void RemoveFleetDestination(int fleetId)
        {
            FleetDestinationRemove.Invoke(fleetId);
        }
    }
}