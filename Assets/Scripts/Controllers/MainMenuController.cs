using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using UnityEngine.SceneManagement;

namespace Iam.Scripts.Controllers
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private GameSettings GameSettings;
        // Start is called before the first frame update

        public void NewGameButton_OnClick()
        {
            GenerateNewGame();
            SceneManager.LoadScene("GalaxyView");
        }

        public void LoadGameButton_OnClick()
        {
            // TODO: open file screen
            // load that file
            // SceneManager.LoadScene("GalaxyView");
        }

        public void QuitGameButton_OnClick()
        {
            Application.Quit();
        }

        private void GenerateNewGame()
        {
            GameSettings.Galaxy = new Galaxy(GameSettings.GalaxySize);
            GameSettings.Galaxy.GenerateGalaxy(1);
        }
    }
}