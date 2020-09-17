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

        UnitTreeView()
        {
            _buttonMap = new Dictionary<int, Button>();
        }

        public void AddLeafUnit(int id, string name)
        {
            GameObject unit = Instantiate(LeafSquadPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            Text squadName = unit.transform.Find("SquadName").GetComponent<Text>();
            squadName.text = name;
            //GetInstanceID is guaranteed to be unique
            Button button = unit.transform.Find("Image").GetComponent<Button>();
            button.onClick.AddListener(() => UnitButton_OnClick(id));
            _buttonMap[id] = button;
        }

        public void AddTreeUnit(int id, string name, List<Tuple<int, string>> squadData)
        {
            GameObject unit = Instantiate(TreeUnitPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            Transform header = unit.transform.Find("Header");
            Button button = header.GetComponent<Button>();
            button.onClick.AddListener(() => UnitButton_OnClick(id));
            Transform squadList = unit.transform.Find("SquadList");
            Text companyName = header.Find("CompanyName").GetComponent<Text>();
            companyName.text = name;
            _buttonMap[id] = button;
            foreach (Tuple<int, string> squad in squadData)
            {
                GameObject squadUnit = Instantiate(ChildLeafPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                squadList);
                Text squadName = squadUnit.transform.Find("SquadName").GetComponent<Text>();
                squadName.text = squad.Item2;
                button = squadUnit.transform.Find("Image").GetComponent<Button>();
                button.onClick.AddListener(() => UnitButton_OnClick(squad.Item1));
                _buttonMap[squad.Item1] = button;
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

        private void UnitButton_OnClick(int id)
        {
            ColorBlock colors;
            if(_selectedButton != null)
            {
                colors = _selectedButton.colors;
                colors.normalColor = Color.white;
                _selectedButton.colors = colors;
            }
            _selectedButton = _buttonMap[id];
            colors = _selectedButton.colors;
            colors.normalColor = Color.gray;
            _selectedButton.colors = colors;
            OnUnitSelected.Invoke(id);
        }
    }
}
