using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OnlyWar.Scripts.Views
{
    public class BattleView : MonoBehaviour
    {
        public UnityEvent<int> OnSoldierPointerEnter;
        public UnityEvent OnSoldierPointerExit;
        public UnityEvent<int> OnSoldierPointerClick;
        
        [SerializeField]
        private Text BattleLog;
        [SerializeField]
        private Text TempPlayerWoundTrack;
        [SerializeField]
        private GameObject Map;
        [SerializeField]
        private GameObject NextStepButton;
        [SerializeField]
        private ScrollRect ScrollRect;
        [SerializeField]
        private GameSettings GameSettings;
        [SerializeField]
        private GameObject SoldierPrefab;

        private bool _scrollToBottom = false;
        private readonly Dictionary<int, Tuple<RectTransform, Image, Image, EventTrigger>> _soldierMap;
        private Text _nextStepButtonText;

        public BattleView()
        {
            _soldierMap = new Dictionary<int, Tuple<RectTransform, Image, Image, EventTrigger>>();
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

        public void AddSoldier(int id, Vector2 position, Color color)
        {
            GameObject soldier = Instantiate(SoldierPrefab,
                                position,
                                Quaternion.identity,
                                Map.transform);

            RectTransform rt = soldier.GetComponent<RectTransform>();
            position.Scale(GameSettings.BattleMapScale);
            rt.anchoredPosition = position;

            Image soldierImage = soldier.transform.Find("SoldierCircle").GetComponent<Image>();
            soldierImage.gameObject.name = id.ToString();
            soldierImage.color = color;

            Image haloImage = soldier.transform.Find("SoldierHalo").GetComponent<Image>();
            haloImage.gameObject.name = id.ToString();

            EventTrigger trigger = soldier.GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            entry.callback.AddListener((eventData) => { SoldierCircle_OnPointerEnter(eventData); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            entry.callback.AddListener((eventData) => { SoldierCircle_OnPointerExit(eventData); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((eventData) => { SoldierCircle_OnPointerClick(eventData); });
            trigger.triggers.Add(entry);

            _soldierMap[id] = new Tuple<RectTransform, Image, Image, EventTrigger>
                (rt, soldierImage, haloImage, trigger);
            
        }

        public void MoveSoldier(int id, Vector2 newPosition)
        {
            RectTransform rt = _soldierMap[id].Item1;
            newPosition.Scale(GameSettings.BattleMapScale);
            rt.anchoredPosition = newPosition;
        }

        public void RemoveSoldier(int id)
        {
            GameObject.Destroy(_soldierMap[id].Item1.gameObject);
            _soldierMap.Remove(id);
        }

        public void Clear()
        {
            BattleLog.text = "";
            TempPlayerWoundTrack.text = "";

            foreach(Tuple<RectTransform, Image, Image, EventTrigger> tuple in _soldierMap.Values)
            {
                GameObject.Destroy(tuple.Item1.gameObject);
            }
            _soldierMap.Clear();
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

        public void UpdateNextStepButton(string text, bool enabled)
        {
            NextStepButton.gameObject.SetActive(enabled);
            if(_nextStepButtonText == null)
            {
                _nextStepButtonText = NextStepButton.GetComponentInChildren<Text>();
            }
            _nextStepButtonText.text = text;
        }

        public void OverwritePlayerWoundTrack(string text)
        {
            TempPlayerWoundTrack.text = text;
        }

        public void HighlightSoldiers(IEnumerable<int> soldiers, bool highlight, Color color)
        {
            foreach (int soldierId in soldiers)
            {
                var currentSoldier = _soldierMap[soldierId];
                currentSoldier.Item3.gameObject.SetActive(highlight);
                if (highlight)
                {
                    currentSoldier.Item3.color = color;
                }
            }
        }

        private void SoldierCircle_OnPointerEnter(BaseEventData bed)
        {
            PointerEventData pointerData = (PointerEventData)bed;
            int id = Convert.ToInt32(pointerData.pointerCurrentRaycast.gameObject.name);
            OnSoldierPointerEnter.Invoke(id);
        }

        private void SoldierCircle_OnPointerExit(BaseEventData bed)
        {
            OnSoldierPointerExit.Invoke();
        }

        private void SoldierCircle_OnPointerClick(BaseEventData bed)
        {
            PointerEventData pointerData = (PointerEventData)bed;
            int id = Convert.ToInt32(pointerData.pointerCurrentRaycast.gameObject.name);
            OnSoldierPointerClick.Invoke(id);
        }
    }
}
