using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Iam.Scripts.Models;

namespace Iam.Scripts
{
    public class GalaxyMap : MonoBehaviour
    {
        public UnityEvent<int, Vector2, Vector2, Color> OnFleetPathDraw;
        public UnityEvent<int> OnFleetPathRemove;

        Galaxy _galaxy;
        public GameObject StarPrefab;
        public GameObject FleetPrefab;
        public LayerMask StarLayerMask;
        public LayerMask FleetLayerMask;
        public GameSettings GameSettings;
        Transform _selectedFleet;

        // Update is called once per frame
        void FixedUpdate()
        {
            HandleMouseMove();
            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseLeftClick();
            }
        }

        private void HandleMouseMove()
        {
            if (_selectedFleet != null)
            {
                RaycastHit2D hitInfo = GetMouseHit();
                if(hitInfo.collider != null && hitInfo.collider.name == "StarSprite")
                {
                    // draw a path from the fleet to the planet we're hovering over
                    // TODO: Are we worried about the efficiency of deleting and redrawing the same line over and over when the mouse is sitting on a planet?
                    // TODO: will need a real fleet key once we have multiple fleets
                    OnFleetPathDraw.Invoke(0, _selectedFleet.position, hitInfo.collider.transform.position, Color.cyan);
                }
                else
                {
                    // not pointing at a planet, remove path for selected fleet
                    // TODO: will need a real fleet key once we have multiple fleets
                    OnFleetPathRemove.Invoke(0);
                }
            }
        }

        private void HandleMouseLeftClick()
        {
            // Mouse was clicked -- is it on a star?
            // TODO: Ignore clicks if over a UI element
            RaycastHit2D hitInfo = GetMouseHit();
            //if(Physics.Raycast(ray, out hitInfo, StarLayerMask))
            if (hitInfo.collider != null)
            {
                // hit an object
                Vector2 position = new Vector2(hitInfo.collider.transform.position.x / GameSettings.MapScale, hitInfo.collider.transform.position.y / GameSettings.MapScale);
                //Planet planet = galaxy.Planets.Where(p => p.Position == position).SingleOrDefault();
                Debug.Log("Clicked on " + hitInfo.collider.name);
                if (hitInfo.collider.name == "FleetSprite")
                {
                    // clicked on a ship
                    HandleShipClick(hitInfo);
                }
            }
            else
            {

                // we clicked on something, unselect any selected fleets/planets
                _selectedFleet.GetChild(1).gameObject.SetActive(false);
                _selectedFleet = null;
            }
        }

        private static RaycastHit2D GetMouseHit()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            return hitInfo;
        }

        private void HandleShipClick(RaycastHit2D hitInfo)
        {
            _selectedFleet = hitInfo.collider.transform.parent;
            // highlight the ship
            _selectedFleet.GetChild(hitInfo.collider.transform.GetSiblingIndex() + 1).gameObject.SetActive(true);
        }

        public void InitializeVisuals(Galaxy newGalaxy)
        {
            _galaxy = newGalaxy;
            for (int i = 0; i < _galaxy.Planets.Count; i++)
            {
                DrawPlanet(_galaxy.Planets[i]);
            }
            for(int i = 0; i < _galaxy.Fleets.Count; i++)
            {
                DrawFleet(_galaxy.Fleets[i]);
            }
        }

        private void DrawPlanet(Planet planet)
        {
            GameObject star = Instantiate(StarPrefab,
                                new Vector2(planet.Position.x * GameSettings.MapScale, planet.Position.y * GameSettings.MapScale),
                                Quaternion.identity,
                                this.transform);
            TextMesh textMesh = star.GetComponentInChildren<TextMesh>();
            textMesh.text = planet.Name;
            // add color shading of star based on planet type
        }

        private void DrawFleet(Fleet fleet)
        {
            GameObject fleetSprite = Instantiate(FleetPrefab,
                                        new Vector2(fleet.Planet.Position.x * GameSettings.MapScale + 5, fleet.Planet.Position.y * GameSettings.MapScale + 5),
                                        Quaternion.identity,
                                        this.transform);
        }

    }
}