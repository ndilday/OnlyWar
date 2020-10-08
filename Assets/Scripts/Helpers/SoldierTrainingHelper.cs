using System;
using System.Collections.Generic;

using UnityEngine;

using OnlyWar.Scripts.Models;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;

namespace OnlyWar.Scripts.Helpers
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

    public class SoldierTrainingHelper
    {
        public void EvaluateSoldier(PlayerSoldier soldier, Date trainingFinishedYear)
        {
            soldier.UpdateRatings();

            if (soldier.MeleeRating > 700) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Adamantium Sword of the Emperor badge during training");
            else if (soldier.MeleeRating > 600) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Sword of the Emperor badge during training");
            else if (soldier.MeleeRating > 500) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Sword of the Emperor badge during training");
            else if (soldier.MeleeRating > 400) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Sword of the Emperor badge during training");

            if (soldier.RangedRating > 75) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Marksman badge during training with " + soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill.Name);
            else if (soldier.RangedRating > 65) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Marksman badge during training with " + soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill.Name);
            else if (soldier.RangedRating > 60) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Marksman badge during training with " + soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill.Name);

            if (soldier.LeadershipRating > 235) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Voice of the Emperor badge during training");
            else if (soldier.LeadershipRating > 160) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Voice of the Emperor badge during training");
            else if (soldier.LeadershipRating > 135) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Voice of the Emperor badge during training");

            if (soldier.AncientRating > 72) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Banner of the Emperor badge during training");
            else if (soldier.AncientRating > 65) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Banner of the Emperor badge during training");
            else if (soldier.AncientRating > 57) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Banner of the Emperor badge during training");

            if (soldier.MedicalRating > 100) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Flagged for potential training as Apothecary");

            if (soldier.TechRating > 100) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Flagged for potential training as Techmarine");

            if (soldier.PietyRating > 110) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Devout badge and declared a Novice");
        }

        public void ApplySoldierWorkExperience(ISoldier soldier, float points)
        {
            float powerArmorSkill = soldier.GetTotalSkillValue(TempBaseSkillList.Instance.PowerArmor);
            // if any gunnery, ranged, melee, or vehicle skill is below the PA skill, focus on improving PA
            float gunnerySkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Gunnery).BaseSkill);
            float meleeSkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Melee).BaseSkill);
            float rangedSkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill);
            float vehicleSkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Vehicle).BaseSkill);
            float[] floatArray = { gunnerySkill, meleeSkill, rangedSkill, vehicleSkill };
            float totalMax = Mathf.Max(floatArray);
            if (totalMax > powerArmorSkill)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, points);
            }
            else
            {
                ApplyMarineWorkExperienceByType(soldier, points);
            }
        }

        public void ApplyMarineWorkExperienceByType(ISoldier soldier, float points)
        {
            switch(soldier.Type.Name)
            {
                case "Chapter Master":
                case "Captain":
                case "Sergeant":
                    break;
                case "Master of the Apothecarion":
                case "Apothecary":
                    break;
                case "Master of the Forge":
                case "Master Techmarine":
                case "TechMarine":
                    break;
                case "Master of the Librarium":
                case "Epistolary":
                case "Codiciers":
                case "Lexicanium":
                    break;
                case "Master of Sanctity":
                case "Reclusiarch":
                case "Chaplain":
                    break;
                case "Ancient":
                    break;
                case "Champion":
                    break;
                case "Veteran":
                    ApplyVeteranWorkExperience(soldier, points);
                    break;
                case "Tactical Marine":
                    ApplyTacticalWorkExperience(soldier, points);
                    break;
                case "Assault Marine":
                    ApplyAssaultWorkExperience(soldier, points);
                    break;
                case "Devastator Marine":
                    ApplyDevastatorWorkExperience(soldier, points);
                    break;
                case "Scout Marine":
                    // scouts are handled via the recruitment controller
                    break;
            }
        }

        public void ApplyVeteranWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 7.0f;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, pointShare);
            if (soldier.Type.IsSquadLeader)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare);
            }
            else
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);
            }
        }

        public void ApplyTacticalWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);

            if (soldier.Type.IsSquadLeader)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare * 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare * 2);
            }
            else
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.MissileLauncher, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Plasma, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Flamer, pointShare);
            }
        }

        public void ApplyAssaultWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.JumpPack, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);

            if (soldier.Type.IsSquadLeader)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare);
            }
            else
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);
            }
        }

        public void ApplyDevastatorWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);

            if (soldier.Type.IsSquadLeader)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare * 2);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare * 2);
            }
            else
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Plasma, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Flamer, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.MissileLauncher, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Lascannon, pointShare);
            }
        }

        public void ApplyScoutWorkExperience(ISoldier soldier, float points)
        {
            // scouts in reserve get training, not work experience
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Marine, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.PowerArmor, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.ArmorySmallArms, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Stealth, pointShare);

            if (soldier.Type.IsSquadLeader)
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Tactics, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Leadership, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Teaching, pointShare);
            }
            else
            {
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Sniper, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, pointShare);
                soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
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
                    foreach (ISoldier soldier in squad.Members)
                    {
                        if ((focuses & TrainingFocuses.Melee) != TrainingFocuses.None)
                        {
                            TrainMelee(soldier, baseLearning / numberOfAreas);
                        }
                        if ((focuses & TrainingFocuses.Physical) != TrainingFocuses.None)
                        {
                            TrainPhysical(soldier, baseLearning / numberOfAreas);
                        }
                        if ((focuses & TrainingFocuses.Ranged) != TrainingFocuses.None)
                        {
                            TrainRanged(soldier, baseLearning / numberOfAreas);
                        }
                        if ((focuses & TrainingFocuses.Vehicles) != TrainingFocuses.None)
                        {
                            TrainVehicles(soldier, baseLearning / numberOfAreas);
                        }
                    }
                }
            }
        }

        private void TrainMelee(ISoldier soldier, float points)
        {
            float pointShare = points / 4;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sword, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Shield, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Axe, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Fist, pointShare);
        }

        private void TrainPhysical(ISoldier soldier, float points)
        {
            // Traits are 10 for 11, 20 for 12, 40 for 13, 80 for 14, 160 for 15, 320 for 16
            // y = (2^(x-11))*10
            // y = 
            float pointShare = points / 3;
            soldier.AddAttributePoints(Models.Soldiers.Attribute.Strength, pointShare);
            soldier.AddAttributePoints(Models.Soldiers.Attribute.Dexterity, pointShare);
            soldier.AddAttributePoints(Models.Soldiers.Attribute.Constitution, pointShare);
        }

        private void TrainRanged(ISoldier soldier, float points)
        {
            float pointShare = points / 5;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bolter, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Lascannon, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Flamer, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Sniper, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Shotgun, pointShare);
        }

        private void TrainVehicles(ISoldier soldier, float points)
        {
            float pointShare = points / 4;
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Bike, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.LandSpeeder, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.Rhino, pointShare);
            soldier.AddSkillPoints(TempBaseSkillList.Instance.GunneryBolter, pointShare);
        }
    }
}
