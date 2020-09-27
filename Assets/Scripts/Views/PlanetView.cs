using UnityEngine;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    class PlanetView : MonoBehaviour
    {
        [SerializeField]
        private Text ScoutingReport;
        [SerializeField]
        private Button LoadToShipButton;
        [SerializeField]
        private Button RemoveFromShipButton;

        public void UpdateScoutingReport(string newText)
        {
            ScoutingReport.text = newText;
        }

        public void EnableLoadInShipButton(bool enable)
        {
            LoadToShipButton.gameObject.SetActive(enable);
        }

        public void EnableRemoveFromShipButton(bool enable)
        {
            RemoveFromShipButton.gameObject.SetActive(enable);
        }
    }
}
