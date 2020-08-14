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
        public GameObject HqSquadPrefab;
        public GameObject CompanyPrefab;
        public GameObject CompanySquadPrefab;
        public GameObject UnitContent;
        public GameObject SelectedUnitHistory;
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
            unit.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => ButtonClicked(id));
        }

        public void AddCompany(int id, string name, List<Tuple<int, string>> squadData)
        {
            GameObject unit = Instantiate(CompanyPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            unit.transform.Find("Header").GetComponent<Button>().onClick.AddListener(() => ButtonClicked(id));
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
                squadUnit.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => ButtonClicked(squad.Item1));
            }
        }

        private void ButtonClicked(int id)
        {
            OnUnitSelected.Invoke(id);
        }

        public void ReplaceSelectedUnitText(string text)
        {
            Text selectedUnitText = SelectedUnitHistory.GetComponent<Text>();
            selectedUnitText.text = text;
        }

    }
}