using UnityEngine;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    class PlanetView : MonoBehaviour
    {
        [SerializeField]
        private Text ScoutingReport;
        [SerializeField]
        private Button AddToFleetButton;
        [SerializeField]
        private Button RemoveFromFleetButton;

        public void UpdateScoutingReport(string newText)
        {
            ScoutingReport.text = newText;
        }

        public void EnableAddToFleetButton(bool enable)
        {
            AddToFleetButton.gameObject.SetActive(enable);
        }

        public void EnableRemoveFromFleetButton(bool enable)
        {
            RemoveFromFleetButton.gameObject.SetActive(enable);
        }
    }
}
