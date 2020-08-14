using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Iam.Scripts.Models;
using Iam.Scripts.Views;
using System;

namespace Iam.Scripts
{
    public class ChapterController : MonoBehaviour
    {
        public ChapterView ChapterView;
        UnitTemplate _chapter;
        Dictionary<int, UnitTemplate> _unitMap;
        // Start is called before the first frame update
        void Start()
        {
            _chapter = TempChapterOrganization.Instance.Chapter;
            _unitMap = new Dictionary<int, UnitTemplate>();
        }

        public void OnChapterButtonClick()
        {
            ChapterView.gameObject.SetActive(true);
            if (!ChapterView.Initialized)
            {
                ChapterView.AddChapterHq(_chapter.Id + 1000, _chapter.Name + " HQ Squad");
                _unitMap[_chapter.Id + 1000] = _chapter;
                _unitMap[_chapter.Id] = _chapter;
                foreach (UnitTemplate company in _chapter.ChildUnits)
                {
                    List<Tuple<int, string>> squadList = new List<Tuple<int, string>>();
                    squadList.Add(new Tuple<int, string>(company.Id + 1000, company.Name + " HQ Squad"));
                    _unitMap[company.Id + 1000] = company;
                    _unitMap[company.Id] = company;
                    int i = 1;
                    foreach (UnitTemplate squad in company.ChildUnits)
                    {
                        
                        squadList.Add(new Tuple<int, string>(company.Id * 10 + i, squad.Name));
                        _unitMap[company.Id * 10 + i] = squad;
                        i++;
                    }
                    ChapterView.AddCompany(company.Id, company.Name, squadList);
                }
                ChapterView.Initialized = true;
            }
        }

        public void UnitSelected(int unitId)
        {

            ChapterView.ReplaceSelectedUnitText(_unitMap[unitId].Name + " is a very fine unit");
        }
    }
}