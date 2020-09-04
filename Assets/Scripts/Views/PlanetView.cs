using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    class PlanetView : MonoBehaviour
    {
        public Text ScoutingReport;

        public void UpdateScoutingReport(string newText)
        {
            ScoutingReport.text = newText;
        }
    }
}
