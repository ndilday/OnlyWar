using OnlyWar.Models;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyWar.Helpers
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

    public class SoldierTrainingCalculator
    {
        private readonly IReadOnlyDictionary<string, BaseSkill> _skillsByName;

        public SoldierTrainingCalculator(IEnumerable<BaseSkill> baseSkills)
        {
            _skillsByName = baseSkills.ToDictionary(bs => bs.Name);
        }

        public void UpdateRatings(PlayerSoldier soldier)
        {
            // Melee score = (Speed * STR * Melee)
            // Expected score = 16 * 16 * 15.5/8 = 1000
            // low-end = 15 * 15 * 14/8 = 850
            // high-end = 17 * 17 * 16/8 = 578
            soldier.MeleeRating = 
                soldier.Strength * soldier.GetTotalSkillValue(_skillsByName["Sword"]) 
                / (UnityEngine.Random.Range(1.44f, 1.76f) * UnityEngine.Random.Range(1.44f, 1.76f));
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            Skill bestRanged = soldier.GetBestSkillInCategory(SkillCategory.Ranged);
            soldier.RangedRating =
                    (soldier.Dexterity + bestRanged.SkillBonus)
                    / UnityEngine.Random.Range(0.144f, 0.176f); 
            // Leadership Score = CHA * Leadership * Tactics
            soldier.LeadershipRating = soldier.Ego
                * soldier.GetTotalSkillValue(_skillsByName["Leadership"])
                * soldier.GetTotalSkillValue(_skillsByName["Tactics"])
                / (UnityEngine.Random.Range(12.6f, 15.4f) * UnityEngine.Random.Range(1.26f, 1.54f) * UnityEngine.Random.Range(1.26f, 1.54f));
            // Ancient Score = EGO * BOD
            soldier.AncientRating = soldier.Ego * soldier.Constitution
                / (UnityEngine.Random.Range(1.26f, 1.54f) * UnityEngine.Random.Range(2.88f, 3.52f));
            // Medical Score = INT * Medicine
            soldier.MedicalRating = 
                soldier.GetTotalSkillValue(_skillsByName["Diagnosis"])
                * soldier.GetTotalSkillValue(_skillsByName["First Aid"])
                / (UnityEngine.Random.Range(0.99f, 1.21f) * UnityEngine.Random.Range(1.17f, 1.43f));
            // Tech Score =  INT * TechRapair
            soldier.TechRating = 
                soldier.GetTotalSkillValue(_skillsByName["Armory (Small Arms)"])
                * soldier.GetTotalSkillValue(_skillsByName["Armory (Vehicle)"])
                / (UnityEngine.Random.Range(1.17f, 1.43f) * UnityEngine.Random.Range(1.17f, 1.43f));
            // Piety Score = Piety * Ritual * Persuade
            soldier.PietyRating = 
                soldier.GetTotalSkillValue(_skillsByName["Theology (Emperor of Man)"])
                / UnityEngine.Random.Range(0.108f, 0.132f);
        }

        public void EvaluateSoldier(PlayerSoldier soldier, Date trainingFinishedYear)
        {
            UpdateRatings(soldier);

            //if (soldier.MeleeRating > 115) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Adamantium Sword of the Emperor badge during training");
            if (soldier.MeleeRating > 100) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Sword of the Emperor badge during training");
            else if (soldier.MeleeRating > 95) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Sword of the Emperor badge during training");
            else if (soldier.MeleeRating > 86) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Sword of the Emperor badge during training");

            if (soldier.RangedRating > 110) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Marksman badge during training with " + soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill.Name);
            else if (soldier.RangedRating > 105) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Marksman badge during training with " + soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill.Name);
            else if (soldier.RangedRating > 98) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Marksman badge during training with " + soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill.Name);

            if (soldier.LeadershipRating > 100) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Voice of the Emperor badge during training");
            else if (soldier.LeadershipRating > 70) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Voice of the Emperor badge during training");
            else if (soldier.LeadershipRating > 50) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Voice of the Emperor badge during training");

            if (soldier.AncientRating > 125) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Gold Banner of the Emperor badge during training");
            else if (soldier.AncientRating > 110) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Silver Banner of the Emperor badge during training");
            else if (soldier.AncientRating > 95) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Bronze Banner of the Emperor badge during training");

            if (soldier.MedicalRating > 75) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Flagged for potential training as Apothecary");

            if (soldier.TechRating > 50) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Flagged for potential training as Techmarine");

            if (soldier.PietyRating > 90) soldier.AddEntryToHistory(trainingFinishedYear.ToString() + ": Awarded Devout badge and declared a Novice");
        }

        public void ApplySoldierWorkExperience(ISoldier soldier, float points)
        {
            float powerArmorSkill = soldier.GetTotalSkillValue(_skillsByName["Power Armor"]);
            // if any gunnery, ranged, melee, or vehicle skill is below the PA skill, focus on improving PA
            float gunnerySkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Gunnery).BaseSkill);
            float meleeSkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Melee).BaseSkill);
            float rangedSkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Ranged).BaseSkill);
            float vehicleSkill = soldier.GetTotalSkillValue(soldier.GetBestSkillInCategory(SkillCategory.Vehicle).BaseSkill);
            float[] floatArray = { gunnerySkill, meleeSkill, rangedSkill, vehicleSkill };
            float totalMax = Mathf.Max(floatArray);
            if (totalMax > powerArmorSkill)
            {
                soldier.AddSkillPoints(_skillsByName["Power Armor"], points);
            }
            else
            {
                ApplyMarineWorkExperienceByType(soldier, points);
            }
        }

        public void ApplyMarineWorkExperienceByType(ISoldier soldier, float points)
        {
            switch(soldier.Template.Name)
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
            soldier.AddSkillPoints(_skillsByName["Marine"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Power Armor"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Armory (Small Arms)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Drive (Bike)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Jump Pack"], pointShare);
            if (soldier.Template.IsSquadLeader)
            {
                soldier.AddSkillPoints(_skillsByName["Tactics"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Leadership"], pointShare);
            }
            else
            {
                soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Sword"], pointShare);
            }
        }

        public void ApplyTacticalWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(_skillsByName["Marine"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Power Armor"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Armory (Small Arms)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Sword"], pointShare);

            if (soldier.Template.IsSquadLeader)
            {
                soldier.AddSkillPoints(_skillsByName["Tactics"], pointShare * 2);
                soldier.AddSkillPoints(_skillsByName["Leadership"], pointShare * 2);
            }
            else
            {
                soldier.AddSkillPoints(_skillsByName["Gunnery (Rocket)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gunnery (Bolter)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gun (Plasma)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gun (Flamer)"], pointShare);
            }
        }

        public void ApplyAssaultWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(_skillsByName["Marine"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Power Armor"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Armory (Small Arms)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Drive (Bike)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Jump Pack"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Sword"], pointShare);

            if (soldier.Template.IsSquadLeader)
            {
                soldier.AddSkillPoints(_skillsByName["Tactics"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Leadership"], pointShare);
            }
            else
            {
                soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Sword"], pointShare);
            }
        }

        public void ApplyDevastatorWorkExperience(ISoldier soldier, float points)
        {
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(_skillsByName["Marine"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Power Armor"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Armory (Small Arms)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gunnery (Bolter)"], pointShare);

            if (soldier.Template.IsSquadLeader)
            {
                soldier.AddSkillPoints(_skillsByName["Tactics"], pointShare * 2);
                soldier.AddSkillPoints(_skillsByName["Leadership"], pointShare * 2);
            }
            else
            {
                soldier.AddSkillPoints(_skillsByName["Gun (Plasma)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gun (Flamer)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gunnery (Rocket)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gunnery (Laser)"], pointShare);
            }
        }

        public void ApplyScoutWorkExperience(ISoldier soldier, float points)
        {
            // scouts in reserve get training, not work experience
            float pointShare = points / 9.0f;
            soldier.AddSkillPoints(_skillsByName["Marine"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Power Armor"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Armory (Small Arms)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gunnery (Bolter)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Stealth"], pointShare);

            if (soldier.Template.IsSquadLeader)
            {
                soldier.AddSkillPoints(_skillsByName["Tactics"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Leadership"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Teaching"], pointShare);
            }
            else
            {
                soldier.AddSkillPoints(_skillsByName["Gun (Sniper)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gun (Shotgun)"], pointShare);
                soldier.AddSkillPoints(_skillsByName["Gunnery (Bolter)"], pointShare);
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
                    squad.SquadLeader.AddSkillPoints(_skillsByName["Teaching"], 0.05f);
                    if (squad.SquadLeader.GetTotalSkillValue(_skillsByName["Teaching"]) >= 12.0f)
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
            soldier.AddSkillPoints(_skillsByName["Sword"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Shield"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Axe"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Fist"], pointShare);
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
            soldier.AddSkillPoints(_skillsByName["Gun (Bolter)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gunnery (Laser)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Flamer)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Sniper)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gun (Shotgun)"], pointShare);
        }

        private void TrainVehicles(ISoldier soldier, float points)
        {
            float pointShare = points / 4;
            soldier.AddSkillPoints(_skillsByName["Drive (Bike)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Pilot (Land Speeder)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Drive (Rhino)"], pointShare);
            soldier.AddSkillPoints(_skillsByName["Gunnery (Bolter)"], pointShare);
        }
    }
}
