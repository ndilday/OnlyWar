
using UnityEngine;
using UnityEngine.UI;

namespace OnlyWar.Views
{
    class BasicTextView : MonoBehaviour
    {
        [SerializeField]
        private Text EventReport;

        public void UpdateEventReport(string newText)
        {
            EventReport.text = newText;
        }
    }
}
