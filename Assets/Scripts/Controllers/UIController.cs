using UnityEngine;
using UnityEngine.UI;

using Iam.Scripts.Models;

namespace Iam.Scripts.Controllers
{
    public class UIController : MonoBehaviour
    {
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