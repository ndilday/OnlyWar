using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Iam.Scripts.Views
{
    public class ChapterView : MonoBehaviour
    {
        public UnityEvent<int> OnUnitSelected;
        public UnityEvent<int> OnSoldierSelected;
        public GameObject HqSquadPrefab;
        public GameObject CompanyPrefab;
        public GameObject CompanySquadPrefab;
        public GameObject SquadMemberPrefab;
        public GameObject UnitContent;
        public GameObject SquadMemberContent;
        public GameObject SelectedUnitHistoryContent;
        public bool Initialized = false;

        public void AddChapterHq(int id, string name)
        {
            GameObject unit = Instantiate(HqSquadPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            Text squadName = unit.transform.Find("SquadName").GetComponent<Text>();
            squadName.text = name;
            //GetInstanceID is guaranteed to be unique
            unit.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => UnitButtonClicked(id));
        }

        public void AddCompany(int id, string name, List<Tuple<int, string>> squadData)
        {
            GameObject unit = Instantiate(CompanyPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            unit.transform.Find("Header").GetComponent<Button>().onClick.AddListener(() => UnitButtonClicked(id));
            Transform squadList = unit.transform.Find("SquadList");
            Text companyName = unit.transform.Find("Header").Find("CompanyName").GetComponent<Text>();
            companyName.text = name;
            foreach(Tuple<int, string> squad in squadData)
            {
                GameObject squadUnit = Instantiate(CompanySquadPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                squadList);
                Text squadName = squadUnit.transform.Find("SquadName").GetComponent<Text>();
                squadName.text = squad.Item2;
                squadUnit.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => UnitButtonClicked(squad.Item1));
            }
        }

        private void UnitButtonClicked(int id)
        {
            OnUnitSelected.Invoke(id);
        }

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
            foreach(Tuple<int, string, string> squadMember in squadMemberList)
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