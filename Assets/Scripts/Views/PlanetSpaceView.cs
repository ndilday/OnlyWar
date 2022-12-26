using TMPro;
using UnityEngine;

namespace OnlyWar.Views
{
    class PlanetSpaceView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI PlanetReport;
        [SerializeField]
        private TextMeshProUGUI GovernmentReport;
        [SerializeField]
        private TextMeshProUGUI AlliedForcesReport;
        [SerializeField]
        private TextMeshProUGUI OpposingForcesReport;

        public void UpdatePlanetReport(string newText)
        {
            PlanetReport.text = newText;
        }

        public void UpdateGovernmentReport(string newText)
        {
            GovernmentReport.text = newText;
        }

        public void UpdateAlliedForcesReport(string newText)
        {
            AlliedForcesReport.text = newText;
        }

        public void UpdateOpposingForcesReport(string newText)
        {
            OpposingForcesReport.text = newText;
        }
    }
}
