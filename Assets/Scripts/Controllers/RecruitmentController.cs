using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Helpers;

namespace Iam.Scripts.Controllers
{
    public class RecruitmentController : MonoBehaviour
    {
        public UnitTreeView ScoutSquadView;
        public GameSettings GameSettings;
        public RecruitmentView RecruitmentView;

        private Dictionary<int, Unit> _scoutSquads;

        public RecruitmentController()
        {
            _scoutSquads = new Dictionary<int, Unit>();
        }

        public void Chapter_Created()
        {
            PopulateScoutSquadMap();
            EvaluateScouts();
        }

        public void EndTurn_OnClick()
        {
            // at the end of each week, scouts who are on ship or on home planet 
            if(GameSettings.Week % 13 == 1)
            {
                EvaluateScouts();
            }
        }

        public void RecruitButton_OnClick()
        {
            _scoutSquads.Clear();
            PopulateScoutSquadMap();
            ScoutSquadView.gameObject.SetActive(true);
            RecruitmentView.gameObject.SetActive(true);
            foreach(Unit squad in _scoutSquads.Values)
            {
                ScoutSquadView.AddLeafUnit(squad.Id, squad.Name);
            }
        }

        public void UnitSelected(int unitId)
        {
            string squadReport = "";
            Unit squad = _scoutSquads[unitId];
            foreach(Soldier soldier in squad.Members)
            {
                SpaceMarine marine = (SpaceMarine)soldier;
                if(marine.Rank == TempSpaceMarineRanks.ScoutSergeant)
                {
                    continue;
                }
                if(marine.MeleeScore > 400)
                {
                    if(marine.RangedScore > 60)
                    {
                        if (marine.MeleeScore > 500 && marine.RangedScore > 70)
                        {
                            squadReport += marine.ToString() + " is ready to accept the Black Carapace and become a full Battle Brother in any squad required.\n";
                        }
                        else
                        {
                            squadReport += marine.ToString() + " could be promoted to Battle Brother if needed, but I would prefer he earn more seasoning first.\n";
                        }
                    }
                    else if(marine.MeleeScore > 500)
                    {
                        squadReport += marine.ToString() + " would make an able assault marine, but I would prefer he continue to train until his ranged proficiency meets our standards.\n";
                    }
                    else
                    {
                        squadReport += marine.ToString() + " could be promoted to an assault squad in an emergency, but I would not recommend it.\n";
                    }
                }
                else if(marine.RangedScore > 70)
                {
                    squadReport += marine.ToString() + " could be assigned to devastator duties if necessary, but I would prefer he continue to train until his melee proficiency meets our standards.\n";
                }
                else if(marine.RangedScore > 60)
                {
                    squadReport += marine.ToString() + " could be promoted to a devastator squad in an emergency, but I would not recommend it.\n";
                }
                else
                {
                    squadReport += marine.ToString() + " is not ready to become a Battle Brother, and should acquire more seasoning before taking the Black Carapace.\n";
                }
            }
            RecruitmentView.SquadDescription.text = squadReport;
        }
    
        private void PopulateScoutSquadMap()
        {
            foreach (Unit company in GameSettings.Chapter.ChildUnits)
            {
                foreach (Unit squad in company.ChildUnits)
                {
                    if (squad.UnitTemplate.Name == "Scout Squad")
                    {
                        _scoutSquads[squad.Id] = squad;
                    }
                }
            }
        }

        private void EvaluateScouts()
        {
            foreach(Unit squad in _scoutSquads.Values)
            {
                foreach(Soldier soldier in squad.Members)
                {
                    SpaceMarine marine = (SpaceMarine)soldier;
                    SpaceMarineEvaluator.Instance.EvaluateMarine(marine);
                }
            }
        }
    }
}
