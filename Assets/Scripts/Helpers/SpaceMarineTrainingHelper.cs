using System;
using System.Collections.Generic;

using UnityEngine;

using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using System.Linq;

namespace Iam.Scripts.Helpers
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

    public class SpaceMarineTrainingHelper
    {
        public void ApplyMarineWorkExperience(SpaceMarine marine, float points)
        {
            float powerArmorSkill = marine.Skills[TempBaseSkillList.Instance.PowerArmor.Id].SkillBonus + marine.Dexterity;
            // if any gunnery, ranged, melee, or vehicle skill is below the PA skill, focus on improving PA
            foreach(Skill skill in marine.Skills.Values)
            {
                //if()
            }
        }

        public void TrainScouts(IEnumerable<Squad> scoutSquads, Dictionary<int, TrainingFocuses> squadFocusMap)
        {
            foreach (Squad squad in scoutSquads)
            {
                // scout squads on active duty don't have time to train, they'll get battle experience
                if (squad.IsInReserve)
                {
                    bool goodTeacher = false;
                    TrainingFocuses focuses = squadFocusMap[squad.Id];
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
                        if (marine.Rank != TempSpaceMarineRanks.ScoutSergeant)
                        {
                            if ((focuses & TrainingFocuses.Melee) != TrainingFocuses.None)
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
        }

        private void TrainMelee(SpaceMarine marine, float points)
        {
            float pointShare = points / 4;
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
    }
}
