using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Views;

namespace OnlyWar.Scripts.Controllers
{
    class ArchiveController : MonoBehaviour
    {
        [SerializeField]
        private DateTreeView DateTreeView;
        [SerializeField]
        private ArchiveView ArchiveView;
        [SerializeField]
        private GameSettings GameSettings;

        public void ArchiveButton_OnClick()
        {
            ArchiveView.gameObject.SetActive(true);
            DateTreeView.ClearTree();
            PopulateEventTree();
        }

        public void UIController_OnTurnEnd()
        {
            // make sure the dialog is closed
            ArchiveView.gameObject.SetActive(false);
        }

        public void DateTreeView_OnEventSelected(Date id, int eventId)
        {
            List<EventHistory> dateEvents = GameSettings.Chapter.BattleHistory[id];
            EventHistory selectedHistory = dateEvents[eventId];
            string displayText = "";
            foreach(string eventLine in selectedHistory.SubEvents)
            {
                displayText += eventLine + "\n";
            }
            ArchiveView.UpdateEventReport(displayText);
        }

        private void PopulateEventTree()
        {
            var sortedEvents = GameSettings.Chapter.BattleHistory.OrderBy(kvp => kvp.Key);
            foreach(KeyValuePair<Date, List<EventHistory>> kvp in sortedEvents)
            {
                DateTreeView.AddDateAndEvents(kvp.Key, kvp.Value.Select(eh => eh.EventTitle).ToList());
            }
        }
    }
}
