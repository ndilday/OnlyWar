using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class RecruitmentView : MonoBehaviour
    {
        public UnityEvent<int, ushort> OnToggleChange;
        public Text SquadDescription;
        public Text RecruiterMessage;
        public GameObject ToggleParent;
        public Text ToggleHeaderText;

        private int _selectedUnitId;
        private Toggle[] _toggles;
        private const string _toggleText = "What would you like this squad to focus on? (Note: selecting all of the options is the same as selecting none of them.)";

        public void Start()
        {
            ToggleHeaderText.text = "";
            ToggleParent.SetActive(false);
            _toggles = ToggleParent.GetComponentsInChildren<Toggle>();

        }

        public void Unit_Selected(int id)
        {
            ToggleHeaderText.text = _toggleText;
            ToggleParent.SetActive(true);
            _selectedUnitId = id;
            Toggle_Clicked();
        }

        public void SetRecruiterMessage(string message)
        {
            RecruiterMessage.text = message;
        }

        public void SetSquadFlags(ushort flags)
        {
            _toggles[0].isOn = (flags & 0x1) == 0x1;
            _toggles[1].isOn = (flags & 0x2) == 0x2;
            _toggles[2].isOn = (flags & 0x3) == 0x3;
            _toggles[3].isOn = (flags & 0x4) == 0x4;
        }

        public void Toggle_Clicked()
        {
            ushort toggled = 0;
            if (_toggles[0].isOn)
            {
                toggled += 1;
                _toggles[0].GetComponent<Image>().color = Color.grey;
            }
            else
            {
                _toggles[0].GetComponent<Image>().color = Color.white;
            }
            if (_toggles[1].isOn)
            {
                toggled += 2;
                _toggles[1].GetComponent<Image>().color = Color.grey;
            }
            else
            {
                _toggles[1].GetComponent<Image>().color = Color.white;
            }
            if (_toggles[2].isOn)
            {
                toggled += 4;
                _toggles[2].GetComponent<Image>().color = Color.grey;
            }
            else
            {
                _toggles[2].GetComponent<Image>().color = Color.white;
            }
            if (_toggles[3].isOn)
            {
                toggled += 8;
                _toggles[3].GetComponent<Image>().color = Color.grey;
            }
            else
            {
                _toggles[3].GetComponent<Image>().color = Color.white;
            }
            OnToggleChange.Invoke(_selectedUnitId, toggled);
        }
    }
}
