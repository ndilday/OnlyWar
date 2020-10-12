using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OnlyWar.Scripts.Views
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
        private readonly Dictionary<int, Tuple<Button, Image, Badge>> _unitDisplayMap;

        UnitTreeView()
        {
            _unitDisplayMap = new Dictionary<int, Tuple<Button, Image, Badge>>();
        }

        public void AddLeafSquad(int id, string name, Color color, int badgeVal = -1)
        {
            GameObject unit = Instantiate(LeafSquadPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);

            Text squadName = unit.transform.Find("SquadName").GetComponent<Text>();
            squadName.text = name;

            Transform background = unit.transform.Find("BackgroundImage");
            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = color;

            Button button = background.GetComponent<Button>();
            button.onClick.AddListener(() => SquadButton_OnClick(id));

            Badge badge = background.Find("Badge").GetComponent<Badge>();
            if (badgeVal != -1)
            {
                badge.gameObject.SetActive(true);
                badge.SetBadge(badgeVal);
            }

            _unitDisplayMap[id] = new Tuple<Button, Image, Badge>(button, backgroundImage, badge);
        }

        public void AddTreeUnit(int id, string name, Color rootColor, int badgeVal, 
                                List<Tuple<int, string, Color, int>> squadData)
        {
            GameObject unit = Instantiate(TreeUnitPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);

            Transform header = unit.transform.Find("Header");
            Image headerImage = header.GetComponent<Image>();
            headerImage.color = rootColor;
            
            Button button = header.GetComponent<Button>();
            button.onClick.AddListener(() => UnitButton_OnClick(id));

            Transform squadList = unit.transform.Find("SquadList");
            
            Text companyName = header.Find("CompanyName").GetComponent<Text>();
            companyName.text = name;

            Badge badge = header.Find("Badge").GetComponent<Badge>();

            _unitDisplayMap[id] = new Tuple<Button, Image, Badge>(button, headerImage, badge);

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
                Image image = imageTransform.GetComponent<Image>();
                image.color = squad.Item3;
                
                button = imageTransform.GetComponent<Button>();
                button.onClick.AddListener(() => SquadButton_OnClick(squad.Item1));
                
                badge = imageTransform.Find("Badge").GetComponent<Badge>();

                _unitDisplayMap[squad.Item1] = new Tuple<Button, Image, Badge>(button, image, badge);

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
            _unitDisplayMap.Clear();
            _selectedButton = null;
        }

        public void UnitButton_OnClick(int id)
        {
            if(_selectedButton != null)
            {
                _selectedButton.interactable = true;
            }
            _selectedButton = _unitDisplayMap[id].Item1;
            _selectedButton.interactable = false;
            OnUnitSelected.Invoke(id);
        }

        public void SquadButton_OnClick(int id)
        {
            if (_selectedButton != null)
            {
                _selectedButton.interactable = true;
            }
            _selectedButton = _unitDisplayMap[id].Item1;
            _selectedButton.interactable = false;
            OnSquadSelected.Invoke(id);
        }

        public void UpdateUnitBadge(int unitId, int badgeVal)
        {
            Badge badge = _unitDisplayMap[unitId].Item3;
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

        public void UpdateUnitColor(int unitId, Color color)
        {
            Image image = _unitDisplayMap[unitId].Item2;
            image.color = color;
        }

        public void ExpandUnit(int unitId)
        {
            _unitDisplayMap[unitId].Item1.transform.GetChild(0)
                                                   .GetComponent<Button>()
                                                   .onClick.Invoke();
        }
    }
}
