using System.Collections.Generic;
using UnityEngine;

using OnlyWar.Helpers;
using OnlyWar.Models.Units;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Views;

namespace OnlyWar.Controllers
{
    public class RecruitmentController : MonoBehaviour
    {
        [SerializeField]
        private UnitTreeView ScoutSquadView;
        [SerializeField]
        private GameSettings GameSettings;
        [SerializeField]
        private RecruitmentView RecruitmentView;

        private readonly Dictionary<int, Squad> _scoutSquads;
        private readonly Dictionary<int, TrainingFocuses> _squadSkillFocusMap;
        private int _scoutCount;
        private int _squadCount;
        private int _readyCount;
        private SoldierTrainingCalculator _trainingHelper;

        private const string RECRUITER_FORMAT = @"Greetings, sir. My report on the current status of our Neophytes and Aspriants is as follows.

We currently have {0} Neophytes divided amongh {1} squads in our Tenth Company. Of those, I would consider {2} ready to take the sacrament of the Black Carapace and join us as full Battle Brothers.

We have {3} Aspriants in training who have not yet joined the ranks of our Neophytes.

I await any further instructions you have on our recruiting and training efforts.";

        public RecruitmentController()
        {
            _scoutSquads = new Dictionary<int, Squad>();
            _squadSkillFocusMap = new Dictionary<int, TrainingFocuses>();
        }

        private void Start()
        {
            // this if block exists to get around Unity's annoying editor start logic
            if (GameSettings.Galaxy != null)
            {
                _trainingHelper = new SoldierTrainingCalculator(GameSettings.Galaxy.BaseSkillMap.Values);
                PopulateScoutSquadMap();
                EvaluateScouts();
            }
        }

        public void UIController_OnTurnEnd()
        {
            // make sure the dialog is closed
            RecruitmentView.gameObject.SetActive(false);

            // at the end of each week, scouts who are on ship or on home planet get trained and re-evaluated
            foreach(Squad scoutSquad in _scoutSquads.Values)
            {
                if (scoutSquad.IsInReserve)
                {
                    _trainingHelper.TrainScouts(_scoutSquads.Values, _squadSkillFocusMap);
                }
                else
                {
                    foreach (PlayerSoldier soldier in scoutSquad.Members)
                    {
                        _trainingHelper.ApplyScoutWorkExperience(soldier, 0.1f);
                    }
                }
            }
            
            if(GameSettings.Date.Week % 13 == 1)
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
            string message = string.Format(RECRUITER_FORMAT, _scoutCount, _squadCount, _readyCount, 0);
            RecruitmentView.SetRecruiterMessage(message);
            foreach(Squad squad in _scoutSquads.Values)
            {
                ScoutSquadView.AddLeafSquad(squad.Id, squad.Name, Color.white); ;
            }
        }

        public void UnitTreeView_OnSquadSelected(int squadId)
        {
            string squadReport = "";
            Squad squad = _scoutSquads[squadId];
            bool showDeleteSquadButton = false;
            // should we ignore the SGT here or not?
            if (squad.Members.Count == 0)
            {
                squadReport += "This squad has no members. ";
                if(_scoutSquads.Keys.Count > 10)
                {
                    squadReport += "Given the number of scout squads we have, I recommend removing this squad from our order of battle.";
                }
                showDeleteSquadButton = true;
            }
            else
            {
                foreach (PlayerSoldier soldier in squad.Members)
                {
                    if (soldier.Template.IsSquadLeader)
                    {
                        // TODO: add code to test whether the SGT still feels he has things
                        // to teach the soldiers
                    }
                    else
                    {
                        squadReport += GetRecruiterDescription(soldier);
                    }
                }
                RecruitmentView.SetSquadFlags((ushort)_squadSkillFocusMap[squadId]);
            }
            RecruitmentView.UpdateSquadDescription(squadReport);
            RecruitmentView.EnableDeleteSquadButton(showDeleteSquadButton);

        }

        public void RecruitmentView_OnToggleChange(int squadId, ushort newFlags)
        {
            _squadSkillFocusMap[squadId] = (TrainingFocuses)newFlags;
        }

        public void RecruitmentView_OnSquadDeleted(int squadId)
        {
            Squad deletedSquad = _scoutSquads[squadId];
            Unit parentUnit = deletedSquad.ParentUnit;
            parentUnit.RemoveSquad(deletedSquad);
            _scoutSquads.Remove(squadId);
            GameSettings.Chapter.SquadMap.Remove(squadId);
            PopulateScoutSquadMap();
        }

        private string GetRecruiterDescription(PlayerSoldier soldier)
        {
            if (soldier.RangedRating > 105)
            {
                if (soldier.MeleeRating > 90)
                {
                    if (soldier.MeleeRating > 100 && soldier.RangedRating > 105)
                    {
                        return soldier.Name + " is ready to accept the Black Carapace and join a Devastator Squad; I think he will rise through the ranks quickly.\n";
                    }
                    else
                    {
                        return soldier.Name + " is ready to be promoted to a Devastator Squad, but I would prefer he earn more seasoning first.\n";
                    }
                }
                else
                {
                    return soldier.Name + " could be promoted in an emergency, but is not ready to face hand-to-hand combat.\n";
                }
            }
            else if (soldier.MeleeRating > 90)
            {
                return soldier.Name + " has a good grasp of the sword, but his mastery of the bolter leaves something to be desired.\n";
            }
            else
            {
                return soldier.Name + " is not ready to become a Battle Brother, and should acquire more seasoning before taking the Black Carapace.\n";
            }
        }

        private void PopulateScoutSquadMap()
        {
            _squadSkillFocusMap.Clear();
            foreach (Unit company in GameSettings.Chapter.OrderOfBattle.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if ((squad.SquadTemplate.SquadType & SquadTypes.Scout) > 0)
                    {
                        _scoutSquads[squad.Id] = squad;
                        if(_squadSkillFocusMap.ContainsKey(squad.Id))
                        {
                            _squadSkillFocusMap[squad.Id] = _squadSkillFocusMap[squad.Id];
                        }
                        else
                        {
                            _squadSkillFocusMap[squad.Id] = TrainingFocuses.Physical | TrainingFocuses.Vehicles | TrainingFocuses.Melee | TrainingFocuses.Ranged;
                        }
                    }
                }
            }
        }

        private void EvaluateScouts()
        {
            _scoutCount = 0;
            _squadCount = 0;
            _readyCount = 0;
            foreach(Squad squad in _scoutSquads.Values)
            {
                _squadCount++;
                foreach(PlayerSoldier soldier in squad.Members)
                {
                    _trainingHelper.UpdateRatings(soldier);
                    if(soldier.Template.Name == "Scout Marine")
                    {
                        _scoutCount++;
                        if(soldier.MeleeRating > 95 && soldier.RangedRating > 98)
                        {
                            _readyCount++;
                        }
                    }
                }
            }
        }
    }
}
