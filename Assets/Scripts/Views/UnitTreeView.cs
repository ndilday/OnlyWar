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
        public GameObject LeafSquadPrefab;
        public GameObject TreeUnitPrefab;
        public GameObject ChildLeafPrefab;
        public GameObject UnitContent;
        public bool Initialized = false;

        public void AddLeafUnit(int id, string name)
        {
            GameObject unit = Instantiate(LeafSquadPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            Text squadName = unit.transform.Find("SquadName").GetComponent<Text>();
            squadName.text = name;
            //GetInstanceID is guaranteed to be unique
            unit.transform.Find("Image").GetComponent<Button>().onClick.AddListener(() => UnitButtonClicked(id));
        }

        public void AddTreeUnit(int id, string name, List<Tuple<int, string>> squadData)
        {
            GameObject unit = Instantiate(TreeUnitPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                UnitContent.transform);
            unit.transform.Find("Header").GetComponent<Button>().onClick.AddListener(() => UnitButtonClicked(id));
            Transform squadList = unit.transform.Find("SquadList");
            Text companyName = unit.transform.Find("Header").Find("CompanyName").GetComponent<Text>();
            companyName.text = name;
            foreach (Tuple<int, string> squad in squadData)
            {
                GameObject squadUnit = Instantiate(ChildLeafPrefab,
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
    }
}
