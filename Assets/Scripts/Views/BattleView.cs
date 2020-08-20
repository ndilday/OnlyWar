using System;
using UnityEngine;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class BattleView : MonoBehaviour
    {
        public Text BattleLog;
        public GameObject NextStepButton;
        public ScrollRect ScrollRect;

        private bool _scrollToBottom = false;

        public void LateUpdate()
        {
            if(_scrollToBottom)
            {
                Canvas.ForceUpdateCanvases();
                ScrollRect.verticalNormalizedPosition = 0;
                _scrollToBottom = false;
                Canvas.ForceUpdateCanvases();
            }
        }

        public void ClearBattleLog()
        {
            BattleLog.text = "";
        }

        public void LogToBattleLog(string text)
        {
            BattleLog.text += text + "\n";
            _scrollToBottom = true;
        }
    }
}
