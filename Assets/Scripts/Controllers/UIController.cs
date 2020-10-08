using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using OnlyWar.Scripts.Models;

namespace OnlyWar.Scripts.Controllers
{
    public class UIController : MonoBehaviour
    {
        public UnityEvent OnTurnEnd;
        [SerializeField]
        private GameObject BottomUI;
        [SerializeField]
        private Text Date;
        [SerializeField]
        private Text ScreenTitle;
        [SerializeField]
        private GameSettings GameSettings;

        public void Start()
        {
            Date.text = GameSettings.Date.ToString();
            ScreenTitle.text = "Sector Map";
        }

        public void EndTurnButton_OnClick()
        {
            DisableUI();
            GameSettings.Date.IncrementWeek();
            Date.text = GameSettings.Date.ToString();
            OnTurnEnd.Invoke();
        }

        public void GalaxyController_OnTurnStart()
        {
            EnableUI();
        }

        public void EndGameButton_OnClick()
        {
            Application.Quit();
        }

        public void ChapterButton_OnClick()
        {
            ScreenTitle.text = "Chapter Organization";
        }

        public void ArchiveButton_OnClick()
        {
            ScreenTitle.text = "Chapter Organization";
        }

        public void ApothecaryButton_OnClick()
        {
            ScreenTitle.text = "Apothecarium Report";
        }

        public void RecruitButton_OnClick()
        {
            ScreenTitle.text = "Conquistorum Report";
        }

        public void GalaxyController_OnBattleStart(Planet planet)
        {
            ScreenTitle.text = "Battle on " + planet.Name;
        }

        public void Dialog_OnClose()
        {
            ScreenTitle.text = "Sector Map";
        }

        public void GalaxyController_OnPlanetSelected(Planet planet)
        {
            ScreenTitle.text = planet.Name;
        }

        private void DisableUI()
        {
            BottomUI.SetActive(false);
        }

        private void EnableUI()
        {
            BottomUI.SetActive(true);
        }
    }
}