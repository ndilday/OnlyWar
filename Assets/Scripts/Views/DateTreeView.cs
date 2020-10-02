using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Iam.Scripts.Models;

namespace Iam.Scripts.Views
{
    public class DateTreeView : MonoBehaviour
    {
        public UnityEvent<Date, int> OnEventSelected;
        
        [HideInInspector]
        public bool Initialized = false;

        [SerializeField]
        private GameObject TreeNodePrefab;
        [SerializeField]
        private GameObject ChildLeafPrefab;
        [SerializeField]
        private GameObject NodeContent;

        public void AddDateAndEvents(Date id, List<string> eventData)
        {
            GameObject unit = Instantiate(TreeNodePrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                NodeContent.transform);
            Transform eventContainer = unit.transform.Find("SquadList");
            Text companyName = unit.transform.Find("Header").Find("CompanyName").GetComponent<Text>();
            companyName.text = id.ToString();
            for (int i = 0; i < eventData.Count; i++)
            {
                GameObject eventObject = Instantiate(ChildLeafPrefab,
                                new Vector3(0, 0, 0),
                                Quaternion.identity,
                                eventContainer);
                Text eventName = eventObject.transform.Find("SquadName").GetComponent<Text>();
                eventName.text = eventData[i];
                int foo = i;
                eventObject.transform.Find("BackgroundImage").GetComponent<Button>().onClick.AddListener(() => NodeButton_OnClick(id, foo));
            }
        }

        public void ClearTree()
        {
            foreach(Transform child in NodeContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void NodeButton_OnClick(Date id, int index)
        {
            OnEventSelected.Invoke(id, index);
        }
    }
}
