using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iam.Scripts.Models;

namespace Iam.Scripts
{
    public class GalaxyMap : MonoBehaviour
    {
        Galaxy galaxy;
        public GameObject StarPrefab;
        public LayerMask StarLayerMask;
        public LayerMask FleetLayerMask;
        const float SCALE = 10.0f;
        bool IsFleetSelected = false;

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonUp(0))
            {
                // Mouse was clicked -- is it on a star?
                // TODO: Ignore clicks if over a UI element
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                //if(Physics.Raycast(ray, out hitInfo, StarLayerMask))
                if (hitInfo.collider != null)
                {
                    // hit a star
                    Vector2 position = new Vector2(hitInfo.collider.transform.position.x / SCALE, hitInfo.collider.transform.position.y / SCALE);
                    Planet planet = galaxy.Planets.Where(p => p.Position == position).SingleOrDefault();
                    Debug.Log("Clicked on " + planet.Name);
                }
            }
        }

        public void InitializeVisuals(Galaxy newGalaxy)
        {
            galaxy = newGalaxy;
            for (int i = 0; i < galaxy.Planets.Count; i++)
            {
                GameObject star = Instantiate(StarPrefab,
                    new Vector2(galaxy.Planets[i].Position.x * SCALE, galaxy.Planets[i].Position.y * SCALE),
                    Quaternion.identity,
                    this.transform);
                TextMesh textMesh = star.GetComponentInChildren<TextMesh>();
                textMesh.text = galaxy.Planets[i].Name;
                // add color shading of star based on planet type
            }
        }
    }
}