using OnlyWar.Scripts.Models.Soldiers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OnlyWar.Scripts.Views
{
    public class SquadMemberView : MonoBehaviour
    {
        public UnityEvent<int> OnSoldierSelected;
        public UnityEvent<Tuple<int, SoldierTemplate, string>> OnSoldierTransferred;

        [SerializeField]
        private GameObject SquadMemberPrefab;
        [SerializeField]
        private GameObject SquadMemberContent;
        [SerializeField]
        private GameObject SelectedUnitHistoryContent;
        [SerializeField]
        private GameObject TransferPanel;

        private Dictionary<int, Button> _buttonMap;
        private Button _selectedButton;
        private Dropdown _transferDropdown;
        private Button _transferConfirmationButton;
        private List<Tuple<int, SoldierTemplate, string>> _openingsList;

        private void SquadMemberButton_OnClick(int id)
        {
            if(_selectedButton != null)
            {
                _selectedButton.interactable = true;
            }
            OnSoldierSelected.Invoke(id);
            _selectedButton = _buttonMap[id];
            _selectedButton.interactable = false;
        }

        public void ReplaceSelectedUnitText(string text)
        {
            Text selectedUnitText = SelectedUnitHistoryContent.GetComponent<Text>();
            selectedUnitText.text = text;
        }

        public void ReplaceSquadMemberContent(List<Tuple<int, string, string, Color>> squadMemberList)
        {
            _selectedButton = null;
            if (_buttonMap == null)
            {
                _buttonMap = new Dictionary<int, Button>();
            }
            else
            {
                _buttonMap.Clear();
            }
            foreach (Transform child in SquadMemberContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            if (squadMemberList != null)
            {
                foreach (Tuple<int, string, string, Color> squadMember in squadMemberList)
                {
                    GameObject squadUnit = Instantiate(SquadMemberPrefab,
                                    new Vector3(0, 0, 0),
                                    Quaternion.identity,
                                    SquadMemberContent.transform);
                    squadUnit.transform.GetComponent<Image>().color = squadMember.Item4;
                    Text rankText = squadUnit.transform.Find("Rank").GetComponent<Text>();
                    Text nameText = squadUnit.transform.Find("Name").GetComponent<Text>();
                    rankText.text = squadMember.Item2;
                    nameText.text = squadMember.Item3;
                    var button = squadUnit.transform.GetComponent<Button>();
                    _buttonMap[squadMember.Item1] = button;
                    button.onClick.AddListener(() => SquadMemberButton_OnClick(squadMember.Item1));
                }
            }
        }

        public void DisplayTransferPanel(bool isDisplayed)
        {
            TransferPanel.SetActive(isDisplayed);
            if(isDisplayed && _transferDropdown == null)
            {
                _transferDropdown = TransferPanel.GetComponentInChildren<Dropdown>();
                _transferConfirmationButton = TransferPanel.GetComponentInChildren<Button>();
            }
            _transferConfirmationButton.interactable = false;
        }

        public void PopulateTransferDropdown(List<Tuple<int, SoldierTemplate, string>> entries)
        {
            if(!TransferPanel.activeSelf)
            {
                DisplayTransferPanel(true);
            }
            _transferConfirmationButton.interactable = false;
            _transferDropdown.ClearOptions();
            _openingsList = entries;
            _transferDropdown.AddOptions(_openingsList.Select(
                tuple => new Dropdown.OptionData(tuple.Item3)).ToList());
        }

        public void Dropdown_OnValueChanged()
        {
            _transferConfirmationButton.interactable = _transferDropdown.value != 0;
        }

        public void Button_OnClick()
        {
            OnSoldierTransferred.Invoke(_openingsList[_transferDropdown.value]);

        }
    }
}