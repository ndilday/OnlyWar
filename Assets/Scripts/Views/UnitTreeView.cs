using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class UnitTreeView : MonoBehaviour
    {
        public UnityEvent<int> OnUnitSelected;
        public UnityEvent<int> OnSquadSelected;
        
        [HideInInspector]
        public bool Initialized = false;

        [SerializeField]
        private GameObject LeafSquadPrefab;
        [SerializeField]
        private GameObject TreeUnitPrefab;
        [SerializeField]
        private GameObject ChildLeafPrefab;
        [SerializeField]
        private GameObject UnitContent;

        private Button _selectedButton;
        private readonly Dictionary<int, Button> _buttonMap;
        private readonly Dictionary<int, Badge> _badgeMap;

        UnitTreeView()
        {
            _buttonMap = new Dictionary<int, Button>();
            _badgeMap = new Dictionary<int, Badge>();
        }

        public void AddLeafSquad(int id, string name, Color color, int badgeVal = -1)
        {
            GameObject unit = Instantiate(LeafSquadPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);

            Text squadName = unit.transform.Find("SquadName").GetComponent<Text>();
            squadName.text = name;

            Transform backgroundImage = unit.transform.Find("BackgroundImage");
            backgroundImage.GetComponent<Image>().color = color;

            Button button = backgroundImage.GetComponent<Button>();
            button.onClick.AddListener(() => SquadButton_OnClick(id));
            _buttonMap[id] = button;

            Badge badge = backgroundImage.Find("Badge").GetComponent<Badge>();
            _badgeMap[id] = badge;
            if (badgeVal != -1)
            {
                badge.gameObject.SetActive(true);
                badge.SetBadge(badgeVal);
            }
        }

        public void AddTreeUnit(int id, string name, Color rootColor, int badgeVal, 
                                List<Tuple<int, string, Color, int>> squadData)
        {
            GameObject unit = Instantiate(TreeUnitPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);

            Transform header = unit.transform.Find("Header");
            header.GetComponent<Image>().color = rootColor;
            
            Button button = header.GetComponent<Button>();
            button.onClick.AddListener(() => UnitButton_OnClick(id));
            _buttonMap[id] = button;

            Transform squadList = unit.transform.Find("SquadList");
            
            Text companyName = header.Find("CompanyName").GetComponent<Text>();
            companyName.text = name;

            Badge badge = header.Find("Badge").GetComponent<Badge>();
            _badgeMap[id] = badge;

            if (badgeVal != -1)
            {
                badge.gameObject.SetActive(true);
                badge.SetBadge(badgeVal);
            }

            foreach (Tuple<int, string, Color, int> squad in squadData)
            {
                GameObject squadUnit = Instantiate(ChildLeafPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                squadList);
                Text squadName = squadUnit.transform.Find("SquadName").GetComponent<Text>();
                squadName.text = squad.Item2;

                Transform imageTransform = squadUnit.transform.Find("BackgroundImage");
                imageTransform.GetComponent<Image>().color = squad.Item3;
                
                button = imageTransform.GetComponent<Button>();
                button.onClick.AddListener(() => SquadButton_OnClick(squad.Item1));
                _buttonMap[squad.Item1] = button;
                
                badge = imageTransform.Find("Badge").GetComponent<Badge>();
                _badgeMap[squad.Item1] = badge;

                if (squad.Item4 != -1)
                {    
                    badge.gameObject.SetActive(true);
                    badge.SetBadge(squad.Item4);
                }
            }
        }

        public void ClearTree()
        {
            foreach(Transform child in UnitContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            _buttonMap.Clear();
            _selectedButton = null;
        }

        public void UnitButton_OnClick(int id)
        {
            if(_selectedButton != null)
            {
                _selectedButton.interactable = true;
            }
            _selectedButton = _buttonMap[id];
            _selectedButton.interactable = false;
            OnUnitSelected.Invoke(id);
        }

        public void SquadButton_OnClick(int id)
        {
            if (_selectedButton != null)
            {
                _selectedButton.interactable = true;
            }
            _selectedButton = _buttonMap[id];
            _selectedButton.interactable = false;
            OnSquadSelected.Invoke(id);
        }

        public void UpdateUnitBadge(int unitId, int badgeVal)
        {
            Badge badge = _badgeMap[unitId];
            if (badgeVal == -1)
            {
                badge.gameObject.SetActive(false);
            }
            else
            {
                badge.gameObject.SetActive(true);
                badge.SetBadge(badgeVal);
            }
        }
    }
}
