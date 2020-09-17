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
            float powerArmorSkill = marine.GetTotalSkillValue(TempBaseSkillList.Instance.PowerArmor);
            // if any gunnery, ranged, melee, or vehicle skill is below the PA skill, focus on improving PA
            float gunnerySkill = marine.GetTotalSkillValue(marine.GetBestSkillByCategory(SkillCategory.Gunnery).BaseSkill);
            float meleeSkill = marine.GetTotalSkillValue(marine.GetBestSkillByCategory(SkillCategory.Melee).BaseSkill);
            float rangedSkill = marine.GetTotalSkillValue(marine.GetBestSkillByCategory(SkillCategory.Ranged).BaseSkill);
            float vehicleSkill = marine.GetTotalSkillValue(marine.GetBestSkillByCategory(SkillCategory.Vehicle).BaseSkill);
            float[] floatArray = { gunnerySkill, meleeSkill, rangedSkill, vehicleSkill };
            float totalMax = Mathf.Max(floatArray);
            if (totalMax > powerArmorSkill)
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, points);
            }
            else
            {
                ApplyMarineWorkExperienceBySquadType(marine, points);
            }
        }

        public void ApplyMarineWorkExperienceBySquadType(SpaceMarine marine, float points)
        {
            switch(marine.AssignedSquad.SquadTemplate.Name)
            {
                case "Veteran Squad":
                    ApplyVeteranWorkExperience(marine, points);
                    break;
                case "Tactical Squad":
                    ApplyTacticalWorkExperience(marine, points);
                    break;
                case "Assault Squad":
                    ApplyAssaultWorkExperience(marine, points);
                    break;
                case "Devastator Squad":
                    ApplyDevastatorWorkExperience(marine, points);
                    break;
            }
        }

        public void ApplyVeteranWorkExperience(SpaceMarine marine, float points)
        {
            float pointShare = points / 7.0f;
            marine.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bike, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, pointShare);
            if (marine.Rank == TempSpaceMarineRanks.VeteranSquadSergeant)
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare);
            }
            else
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);
            }
        }

        public void ApplyTacticalWorkExperience(SpaceMarine marine, float points)
        {
            float pointShare = points / 9.0f;
            marine.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);

            if (marine.Rank == TempSpaceMarineRanks.TacticalSergeant)
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare * 2);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare * 2);
            }
            else
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.MissileLauncher, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Plasma, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Flamer, pointShare);
            }
        }

        public void ApplyAssaultWorkExperience(SpaceMarine marine, float points)
        {
            float pointShare = points / 9.0f;
            marine.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bike, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);

            if (marine.Rank == TempSpaceMarineRanks.VeteranSquadSergeant)
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare);
            }
            else
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);
            }
        }

        public void ApplyDevastatorWorkExperience(SpaceMarine marine, float points)
        {
            float pointShare = points / 9.0f;
            marine.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);

            if (marine.Rank == TempSpaceMarineRanks.TacticalSergeant)
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare * 2);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare * 2);
            }
            else
            {
                marine.AddSkillPoints(TempBaseSkillList.Instance.Plasma, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Flamer, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.MissileLauncher, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Lascannon, pointShare);
            }
        }

        public void ApplyScoutWorkExperience(SpaceMarine marine, float points)
        {
            // scouts in reserve get training, not work experience
            if (!marine.AssignedSquad.IsInReserve)
            {
                float pointShare = points / 9.0f;
                marine.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
                marine.AddSkillPoints(TempBaseSkillList.Instance.Stealth, pointShare);

                if (marine.Rank == TempSpaceMarineRanks.TacticalSergeant)
                {
                    marine.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare);
                    marine.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare);
                    marine.AddSkillPoints(TempBaseSkillList.Instance.Teaching, pointShare);
                }
                else
                {
                    marine.AddSkillPoints(TempBaseSkillList.Instance.Sniper, pointShare);
                    marine.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, pointShare);
                    marine.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
                }
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
                    squad.SquadLeader.AddSkillPoints(TempBaseSkillList.Instance.Teaching, 0.05f);
                    if (squad.SquadLeader.GetTotalSkillValue(TempBaseSkillList.Instance.Teaching) >= 12.0f)
                    {
                        goodTeacher = true;
                    }
                    if (!goodTeacher)
                    {
                        // with a sub-par teacher, learning is halfway between teaching and practicing
                        baseLearning *= 0.75f;
                    }
                    foreach (Soldier soldier in squad.Members)
                    {
                        SpaceMarine marine = (SpaceMarine)soldier;
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

        private void TrainMelee(SpaceMarine marine, float points)
        {
            float pointShare = points / 4;
            marine.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Shield, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Axe, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Fist, pointShare);
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
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Lascannon, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Flamer, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Sniper, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, pointShare);
        }

        private void TrainVehicles(SpaceMarine marine, float points)
        {
            float pointShare = points / 4;
            marine.AddSkillPoints(TempBaseSkillList.Instance.Bike, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.LandSpeeder, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.Rhino, pointShare);
            marine.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
        }

        private float GetModifiedStat(float currentValue, float points)
        {
            float curPoints = Mathf.Pow(2, currentValue - 11) * 10;
            return Mathf.Log((curPoints + points) / 10.0f, 2) + 11;

        }
    }
}
