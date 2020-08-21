using System;
using UnityEngine;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class BattleView : MonoBehaviour
    {
        public Text BattleLog;
        public Text TempPlayerWoundTrack;
        public Text TempOpposingWoundTrack;
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

        public void OverwritePlayerWoundTrack(string text)
        {
            TempPlayerWoundTrack.text = text;
        }

        public void OverwriteOpposingWoundTrack(string text)
        {
            TempOpposingWoundTrack.text = text;
        }
    }
}
