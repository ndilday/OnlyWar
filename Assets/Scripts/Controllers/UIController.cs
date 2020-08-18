using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Iam.Scripts.Controllers
{
    public class UIController : MonoBehaviour
    {
        public GameObject BottomUI;
        public void OnEndTurnClick()
        {
            DisableUI();
        }

        public void OnTurnStart()
        {
            EnableUI();
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