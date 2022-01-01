using OnlyWar.Models.Planets;
using OnlyWar.Helpers.Database.GameState;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OnlyWar.Controllers
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
        [SerializeField]
        private Text SaveButtonText;

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

        public void DiplomacyButton_OnClick()
        {
            ScreenTitle.text = "Communiques";
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

        public void TempSaveButton_OnClick()
        {
            var ships = GameSettings.Galaxy.Fleets.Values.SelectMany(fleet => fleet.Ships);
            var units = GameSettings.Galaxy.Factions.SelectMany(f => f.Units);
            GameStateDataAccess.Instance.SaveData("default.s3db",
                                                  GameSettings.Date,
                                                  GameSettings.Galaxy.Characters,
                                                  GameSettings.Chapter.Requests,
                                                  GameSettings.Galaxy.Planets.Values,
                                                  GameSettings.Galaxy.Fleets.Values,
                                                  units,
                                                  GameSettings.Chapter.PlayerSoldierMap.Values,
                                                  GameSettings.Chapter.BattleHistory);
            StartCoroutine(TemporarySaveButtonUpdateCoroutine());
        }

        private void DisableUI()
        {
            BottomUI.SetActive(false);
        }

        private void EnableUI()
        {
            BottomUI.SetActive(true);
        }

        private IEnumerator TemporarySaveButtonUpdateCoroutine()
        {
            SaveButtonText.text = "<b>SAVED!</b>";
            yield return new WaitForSeconds(2);
            SaveButtonText.text = "Save";
        }
    }
}