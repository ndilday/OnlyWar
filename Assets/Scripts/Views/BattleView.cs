using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class BattleView : MonoBehaviour
    {
        public UnityEvent<int> OnSquadSelected;
        public Text BattleLog;
        public Text TempPlayerWoundTrack;
        public Text TempOpposingWoundTrack;
        public GameObject Map;
        public GameObject NextStepButton;
        public GameObject SquadPrefab;
        public ScrollRect ScrollRect;
        public GameSettings GameSettings;

        private bool _scrollToBottom = false;
        private Dictionary<int, GameObject> _squadMap;

        public BattleView()
        {
            _squadMap = new Dictionary<int, GameObject>();
        }

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

        public void SetMapSize(Vector2 size)
        {
            size.Scale(GameSettings.BattleMapScale);
            Map.GetComponent<RectTransform>().sizeDelta = size;
        }

        public void AddSquad(int id, string name, Vector2 position, Vector2 size)
        {
            position.Scale(GameSettings.BattleMapScale);
            GameObject squad = Instantiate(SquadPrefab,
                                position,
                                Quaternion.identity,
                                Map.transform);
            squad.GetComponentInChildren<Text>().text = name;
            squad.GetComponent<Button>().onClick.AddListener(() => Squad_OnClick(id));
            var rt = squad.GetComponent<RectTransform>();
            rt.anchoredPosition = position;
            size.Scale(GameSettings.BattleMapScale);
            rt.sizeDelta = size;
            _squadMap[id] = squad;
        }

        public void Clear()
        {
            BattleLog.text = "";
            TempOpposingWoundTrack.text = "";
            TempPlayerWoundTrack.text = "";
            foreach(KeyValuePair<int, GameObject> kvp in _squadMap)
            {
                GameObject.Destroy(kvp.Value);
            }
            _squadMap.Clear();
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

        private void Squad_OnClick(int squadId)
        {
            OnSquadSelected.Invoke(squadId);
            
        }
    }
}
