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
    [Flags]
    public enum TrainingFocuses
    {
        None = 0,
        Physical = 0x1,
        Vehicles = 0x2,
        Melee = 0x4,
        Ranged = 0x8
    }

    public class RecruitmentController : MonoBehaviour
    {
        [SerializeField]
        private UnitTreeView ScoutSquadView;
        [SerializeField]
        private GameSettings GameSettings;
        [SerializeField]
        private RecruitmentView RecruitmentView;

        private readonly Dictionary<int, Squad> _scoutSquads;
        private Dictionary<int, TrainingFocuses> _squadSkillFocusMap;
        private int _scoutCount;
        private int _squadCount;
        private int _readyCount;


        private const string RECRUITER_FORMAT = @"Greetings, sir. My report on the current status of our Neophytes and Aspriants is as follows.

We currently have {0} Neophytes divided amongh {1} squads in our Tenth Company. Of those, I would consider {2} ready to take the sacrament of the Black Carapace and join us as full Battle Brothers.

We have {3} Aspriants in training who have not yet joined the ranks of our Neophytes.

I await any further instructions you have on our recruiting and training efforts.";

        public RecruitmentController()
        {
            _scoutSquads = new Dictionary<int, Squad>();
            _squadSkillFocusMap = new Dictionary<int, TrainingFocuses>();
        }

        public void GalaxyController_OnChapterCreated()
        {
            PopulateScoutSquadMap();
            EvaluateScouts();
        }

        public void EndTurnButton_OnClick()
        {
            // at the end of each week, scouts who are on ship or on home planet get trained and re-evaluated
            TrainScouts();
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
                ScoutSquadView.AddLeafUnit(squad.Id, squad.Name);
            }
        }

        public void SquadSelected(int squadId)
        {
            string squadReport = "";
            Squad squad = _scoutSquads[squadId];
            // should we ignore the SGT here or not?
            foreach(Soldier soldier in squad.Members)
            {
                SpaceMarine marine = (SpaceMarine)soldier;
                squadReport += GetRecruiterDescription(marine);
            }
            RecruitmentView.UpdateSquadDescription(squadReport);
            RecruitmentView.SetSquadFlags((ushort)_squadSkillFocusMap[squadId]);

        }

        public void RecruitmentView_OnToggleChange(int squadId, ushort newFlags)
        {
            _squadSkillFocusMap[squadId] = (TrainingFocuses)newFlags;
        }

        private string GetRecruiterDescription(SpaceMarine marine)
        {
            if (marine.MeleeScore > 400)
            {
                if (marine.RangedScore > 60)
                {
                    if (marine.MeleeScore > 500 && marine.RangedScore > 70)
                    {
                        return marine.ToString() + " is ready to accept the Black Carapace and become a full Battle Brother in any squad required.\n";
                    }
                    else
                    {
                        return marine.ToString() + " could be promoted to Battle Brother if needed, but I would prefer he earn more seasoning first.\n";
                    }
                }
                else if (marine.MeleeScore > 500)
                {
                    return marine.ToString() + " would make an able assault marine, but I would prefer he continue to train until his ranged proficiency meets our standards.\n";
                }
                else
                {
                    return marine.ToString() + " could be promoted to an assault squad in an emergency, but I would not recommend it.\n";
                }
            }
            else if (marine.RangedScore > 70)
            {
                return marine.ToString() + " could be assigned to devastator duties if necessary, but I would prefer he continue to train until his melee proficiency meets our standards.\n";
            }
            else if (marine.RangedScore > 60)
            {
                return marine.ToString() + " could be promoted to a devastator squad in an emergency, but I would not recommend it.\n";
            }
            else
            {
                return marine.ToString() + " is not ready to become a Battle Brother, and should acquire more seasoning before taking the Black Carapace.\n";
            }
        }

        private void PopulateScoutSquadMap()
        {
            Dictionary<int, TrainingFocuses> newMap = new Dictionary<int, TrainingFocuses>();
            foreach (Unit company in GameSettings.Chapter.ChildUnits)
            {
                foreach (Squad squad in company.Squads)
                {
                    if (squad.SquadTemplate == TempSpaceMarineSquadTemplates.Instance.ScoutSquadTemplate)
                    {
                        _scoutSquads[squad.Id] = squad;
                        if(_squadSkillFocusMap.ContainsKey(squad.Id))
                        {
                            newMap[squad.Id] = _squadSkillFocusMap[squad.Id];
                        }
                        else
                        {
                            newMap[squad.Id] = TrainingFocuses.Physical | TrainingFocuses.Vehicles | TrainingFocuses.Melee | TrainingFocuses.Ranged;
                        }
                    }
                }
            }
            _squadSkillFocusMap = newMap;
        }

        private void TrainScouts()
        {
            foreach(Squad squad in _scoutSquads.Values)
            {
                bool goodTeacher = false;
                TrainingFocuses focuses = _squadSkillFocusMap[squad.Id];
                int numberOfAreas = 0;
                if ((focuses & TrainingFocuses.Melee) != TrainingFocuses.None) numberOfAreas++;
                if ((focuses & TrainingFocuses.Physical) != TrainingFocuses.None) numberOfAreas++;
                if ((focuses & TrainingFocuses.Ranged) != TrainingFocuses.None) numberOfAreas++;
                if ((focuses & TrainingFocuses.Vehicles) != TrainingFocuses.None) numberOfAreas++;
                if (numberOfAreas == 0)
                {
                    numberOfAreas = 4;
                    focuses = TrainingFocuses.Melee | TrainingFocuses.Physical | TrainingFocuses.Ranged | TrainingFocuses.Vehicles;
                }
                // 200 hours per point means about 5 weeks, so about 1/5 point per week
                float baseLearning = 0.2f;
                foreach (Soldier soldier in squad.GetAllMembers())
                {
                    SpaceMarine marine = (SpaceMarine)soldier;
                    if (marine.Rank == TempSpaceMarineRanks.ScoutSergeant)
                    {
                        // he's working as a teacher, give him quarter credit toward teaching
                        Skill learnedSkill = marine.Skills[TempBaseSkillList.Instance.Teaching.Id];
                        learnedSkill.AddPoints(0.05f);
                        if (marine.Intelligence + learnedSkill.SkillBonus >= 12.0f) goodTeacher = true;
                        break;
                    }
                }
                if (!goodTeacher)
                {
                    // with a sub-par teacher, learning is halfway between teaching and practicing
                    baseLearning *= 0.75f;
                }
                foreach (Soldier soldier in squad.GetAllMembers())
                {   
                    SpaceMarine marine = (SpaceMarine)soldier;
                    if(marine.Rank != TempSpaceMarineRanks.ScoutSergeant)
                    {
                        if((focuses & TrainingFocuses.Melee) != TrainingFocuses.None)
                        {
                            TrainMelee(marine, baseLearning / numberOfAreas);
                        }
                        if ((focuses & TrainingFocuses.Physical) != TrainingFocuses.None)
                        {
                            TrainPhysical(marine, baseLearning / numberOfAreas);
                        }
                        if ((focuses & TrainingFocuses.Ranged) != TrainingFocuses.None)
                        {
                            TrainRanged(marine, baseLearning / numberOfAreas);
                        }
                        if ((focuses & TrainingFocuses.Vehicles) != TrainingFocuses.None)
                        {
                            TrainVehicles(marine, baseLearning / numberOfAreas);
                        }
                    }
                }
            }
        }

        private void TrainMelee(SpaceMarine marine, float points)
        {
            float pointShare = points / 5;
            marine.Skills[TempBaseSkillList.Instance.Sword.Id].AddPoints(pointShare * 2);
            marine.Skills[TempBaseSkillList.Instance.Shield.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Axe.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Fist.Id].AddPoints(pointShare);
        }

        private void TrainPhysical(SpaceMarine marine, float points)
        {
            // Traits are 10 for 11, 20 for 12, 40 for 13, 80 for 14, 160 for 15, 320 for 16
            // y = (2^(x-11))*10
            // y = 
            float pointShare = points / 3;
            marine.Strength = GetModifiedStat(marine.Strength, pointShare);
            marine.Dexterity = GetModifiedStat(marine.Dexterity, pointShare);
            marine.Constitution = GetModifiedStat(marine.Constitution, pointShare);
        }

        private void TrainRanged(SpaceMarine marine, float points)
        {
            float pointShare = points / 5;
            marine.Skills[TempBaseSkillList.Instance.Bolter.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Lascannon.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Flamer.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Sniper.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Shotgun.Id].AddPoints(pointShare);
        }

        private void TrainVehicles(SpaceMarine marine, float points)
        {
            float pointShare = points / 4;
            marine.Skills[TempBaseSkillList.Instance.Bike.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.LandSpeeder.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.Rhino.Id].AddPoints(pointShare);
            marine.Skills[TempBaseSkillList.Instance.GunneryBolter.Id].AddPoints(pointShare);
        }

        private float GetModifiedStat(float currentValue, float points)
        {
            float curPoints = Mathf.Pow(2, currentValue - 11) * 10;
            return Mathf.Log((curPoints + points) / 10.0f, 2) + 11;

        }

        private void EvaluateScouts()
        {
            _scoutCount = 0;
            _squadCount = 0;
            _readyCount = 0;
            foreach(Squad squad in _scoutSquads.Values)
            {
                _squadCount++;
                foreach(Soldier soldier in squad.Members)
                {
                    SpaceMarine marine = (SpaceMarine)soldier;
                    SpaceMarineEvaluator.Instance.EvaluateMarine(marine);
                    if(marine.Rank == TempSpaceMarineRanks.Scout)
                    {
                        _scoutCount++;
                        if(marine.MeleeScore > 500 && marine.RangedScore > 70)
                        {
                            _readyCount++;
                        }
                    }
                }
            }
        }
    }
}
