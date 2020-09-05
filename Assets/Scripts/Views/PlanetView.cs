using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    class PlanetView : MonoBehaviour
    {
        [SerializeField]
        private Text ScoutingReport;

        public void UpdateScoutingReport(string newText)
        {
            ScoutingReport.text = newText;
        }
    }
}
