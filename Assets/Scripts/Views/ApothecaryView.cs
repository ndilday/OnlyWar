using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OnlyWar.Views
{
    public class ApothecaryView : MonoBehaviour
    {
        public UnityEvent<int> OnSoldierSelected;

        [SerializeField]
        private Text GeneSeedText;
        [SerializeField]
        private Text SelectedSoldierReportText;
        [SerializeField]
        private GameObject SquadMemberPrefab;
        [SerializeField]
        private GameObject SquadMemberContent;

        public void UpdateGeneSeedText(string newText)
        {
            GeneSeedText.text = newText;
        }

        private void SquadMember_OnClick(int id)
        {
            OnSoldierSelected.Invoke(id);
        }

        public void ReplaceSelectedSoldierText(string text)
        {
            SelectedSoldierReportText.text = text;
        }

        public void ReplaceSquadMemberContent(List<Tuple<int, string, string>> squadMemberList)
        {
            foreach (Transform child in SquadMemberContent.transform)
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
                    squadUnit.transform.GetComponent<Button>().onClick.AddListener(() => SquadMember_OnClick(squadMember.Item1));
                }
            }
        }
    }
}