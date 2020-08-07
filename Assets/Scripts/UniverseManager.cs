using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Iam.Scripts.Models;

namespace Iam.Scripts
{
    // responsible for holding main Galaxy data object, triggering file save/loads, and generating new galaxy
    public class UniverseManager : MonoBehaviour
    {
        Galaxy galaxy;
        public GalaxyMap Map;
        // Start is called before the first frame update
        void Start()
        {
            galaxy = new Galaxy();
            Generate();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Generate()
        {
            Debug.Log("UniverseManager::Generate -- Generating a new Galaxy");
            galaxy.GenerateGalaxy(1);
            // populate visuals on map
            Map.InitializeVisuals(galaxy);
        }
    }
}