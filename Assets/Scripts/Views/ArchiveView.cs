using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyWar.Scripts.Views
{
    class ArchiveView : MonoBehaviour
    {
        [SerializeField]
        private Text EventReport;

        public void UpdateEventReport(string newText)
        {
            EventReport.text = newText;
        }
    }
}
