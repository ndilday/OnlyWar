using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class SquadMemberView : MonoBehaviour
    {
        public UnityEvent<int> OnSoldierSelected;
        
        public GameObject SquadMemberPrefab;
        public GameObject SquadMemberContent;
        public GameObject SelectedUnitHistoryContent;

        private void SquadMemberButtonClicked(int id)
        {
            OnSoldierSelected.Invoke(id);
        }

        public void ReplaceSelectedUnitText(string text)
        {
            Text selectedUnitText = SelectedUnitHistoryContent.GetComponent<Text>();
            selectedUnitText.text = text;
        }

        public void ReplaceSquadMemberContent(List<Tuple<int, string, string>> squadMemberList)
        {
            foreach(Transform child in SquadMemberContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            if (squadMemberList != null)
            {
                foreach (Tuple<int, string, string> squadMember in squadMemberList)
                {
                    GameObject squadUnit = Instantiate(SquadMemberPrefab,
                                    new Vector3(0, 0, 0),
                                    Quaternion.identity,
                                    SquadMemberContent.transform);
                    Text rankText = squadUnit.transform.Find("Rank").GetComponent<Text>();
                    Text nameText = squadUnit.transform.Find("Name").GetComponent<Text>();
                    rankText.text = squadMember.Item2;
                    nameText.text = squadMember.Item3;
                    squadUnit.transform.GetComponent<Button>().onClick.AddListener(() => SquadMemberButtonClicked(squadMember.Item1));
                }
            }
        }
    }
}