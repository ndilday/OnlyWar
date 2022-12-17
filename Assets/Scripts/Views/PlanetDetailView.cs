using UnityEngine;
using UnityEngine.UI;

namespace OnlyWar.Views
{
    class PlanetDetailView : MonoBehaviour
    {
        [SerializeField]
        private Text ScoutingReport;
        [SerializeField]
        private Text GovernmentReport;
        [SerializeField]
        private Text AlliedForcesReport;
        [SerializeField]
        private Text OpposingForcesReport;

        public void UpdateScoutingReport(string newText)
        {
            ScoutingReport.text = newText;
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
