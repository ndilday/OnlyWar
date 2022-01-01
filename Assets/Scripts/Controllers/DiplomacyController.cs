using OnlyWar.Models;
using OnlyWar.Views;
using UnityEngine;
using UnityEngine.Events;

namespace OnlyWar.Scripts.Controllers
{
    class DiplomacyController : MonoBehaviour
    {
        public UnityEvent<int> PlanetSelected;
        [SerializeField]
        private UnitTreeView RequestTreeView;
        [SerializeField]
        private BasicTextView DetailView;
        [SerializeField]
        private GameSettings GameSettings;

        public void DiplomacyButton_OnClick()
        {
            DetailView.gameObject.SetActive(true);
            RequestTreeView.ClearTree();
            PopulateRequestTree();
        }

        public void UIController_OnTurnEnd()
        {
            // make sure the dialog is closed
            DetailView.gameObject.SetActive(false);
        }

        public void RequestTreeView_OnRequestSelected(int requestId)
        {
            IRequest request = GameSettings.Chapter.Requests[requestId];
            string text = 
                $@"{request.DateRequestMade.ToString()}
                From: Governor, {request.TargetPlanet.Name}
                To: Any Adeptes Astartes in the sector
                ----------
                We need help!";
            DetailView.UpdateEventReport(text);
            PlanetSelected.Invoke(request.TargetPlanet.Id);
        }

        private void PopulateRequestTree()
        {
            foreach (IRequest request in GameSettings.Chapter.Requests)
            {
                string text = $"{request.DateRequestMade.ToString()} - {request.TargetPlanet.Name}";
                Color color = request.DateRequestFulfilled == null ? Color.white : Color.green;
                RequestTreeView.AddLeafSquad(request.Id, text, color);
            }
        }
    }
}
