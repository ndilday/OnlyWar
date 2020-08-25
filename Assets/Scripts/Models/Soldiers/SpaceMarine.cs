using System.Linq;

using Iam.Scripts.Models.Equippables;

namespace Iam.Scripts.Models.Soldiers
{
    public class SpaceMarine : Soldier
    {
        public SpaceMarineRank Rank;
        public string FirstName;
        public string LastName;

        // TODO: break out weapon specializations
        public float MeleeScore;
        public float RangedScore;
        public float LeadershipScore;
        public float MedicalScore;
        public float TechScore;
        public float PietyScore;
        public float AncientScore;

        public void EvaluateSoldier(Date trainingFinishedYear)
        {
            // Melee score = (Speed * STR * Melee)
            // Expected score = 16 * 16 * 15.5/8 = 1000
            // low-end = 15 * 15 * 14/8 = 850
            // high-end = 17 * 17 * 16/8 = 578
            MeleeScore = AttackSpeed * Strength * (Dexterity + Skills[TempBaseSkillList.Instance.Sword.Id].SkillBonus) / 
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if(MeleeScore > 700) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Adamantium Sword of the Emperor badge during training");
            if (MeleeScore > 600) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Sword of the Emperor badge during training");
            else if (MeleeScore > 500) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Sword of the Emperor badge during training");
            else if (MeleeScore > 400) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Sword of the Emperor badge during training");
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            Skill bestRanged = GetBestRangedSkill();
            RangedScore = Perception * (Dexterity + GetBestRangedSkill().SkillBonus) / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (RangedScore > 75) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Marksman badge during training with " + bestRanged.BaseSkill.Name);
            else if (RangedScore > 65) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Marksman badge during training with " + bestRanged.BaseSkill.Name);
            else if (RangedScore > 60) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Marksman badge during training with " + bestRanged.BaseSkill.Name);
            // Leadership Score = EGO * Leadership * Tactics
            LeadershipScore = Ego * (Presence + Skills[TempBaseSkillList.Instance.Leadership.Id].SkillBonus) * (Intelligence + Skills[TempBaseSkillList.Instance.Tactics.Id].SkillBonus) / 
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (LeadershipScore > 235) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Voice of the Emperor badge during training");
            else if (LeadershipScore > 160) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Voice of the Emperor badge during training");
            else if (LeadershipScore > 135) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Voice of the Emperor badge during training");
            // Ancient Score = EGO * BOD
            AncientScore = Ego * Constitution / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (AncientScore > 72) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Banner of the Emperor badge during training");
            else if (AncientScore > 65) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Banner of the Emperor badge during training");
            else if (AncientScore > 57) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Banner of the Emperor badge during training");
            // Medical Score = INT * Medicine
            MedicalScore = (Intelligence + Skills[TempBaseSkillList.Instance.Diagnosis.Id].SkillBonus) * (Intelligence + Skills[TempBaseSkillList.Instance.FirstAid.Id].SkillBonus) / 
                (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f)); ;
            if (MedicalScore > 100) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Flagged for potential training as Apothecary");
            // Tech Score =  INT * TechRapair
            TechScore = (Intelligence + Skills[TempBaseSkillList.Instance.ArmorySmallArms.Id].SkillBonus) * (Intelligence + Skills[TempBaseSkillList.Instance.ArmoryVehicle.Id].SkillBonus) / 
                (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            if (TechScore > 100) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Flagged for potential training as Techmarine");
            // Piety Score = Piety * Ritual * Persuade
            PietyScore = (Presence + Skills[TempBaseSkillList.Instance.Piety.Id].SkillBonus) / UnityEngine.Random.Range(0.09f, 0.11f);
            if (PietyScore > 110) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Devout badge and declared a Novice");
        }

        public Skill GetBestMeleeSkill()
        {
            return Skills.Values.Where(s => s.BaseSkill.Category == SkillCategory.Melee).OrderByDescending(s => s.SkillBonus).First();
        }

        public Skill GetBestRangedSkill()
        {
            return Skills.Values.Where(s => s.BaseSkill.Category == SkillCategory.Ranged).OrderByDescending(s => s.SkillBonus).First();
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public override string JobRole
        {
            get
            {
                return Rank.Name;
            }
        }

        public override bool CanFireWeapon(Weapon weapon)
        {
            HitLocation leftHand = Body.HitLocations.Single(hl => hl.Template.Name == "Left Hand");
            HitLocation leftArm = Body.HitLocations.Single(hl => hl.Template.Name == "Left Arm");
            HitLocation rightHand = Body.HitLocations.Single(hl => hl.Template.Name == "Right Hand");
            HitLocation rightArm = Body.HitLocations.Single(hl => hl.Template.Name == "Right Arm");
            bool leftUsable = leftHand.Wounds < leftHand.Template.WoundLimit && leftArm.Wounds < leftArm.Template.WoundLimit;
            bool rightUsable = rightHand.Wounds < rightHand.Template.WoundLimit && rightArm.Wounds < rightArm.Template.WoundLimit;
            return (weapon.Template.Location == EquipLocation.TwoHand && leftUsable && rightUsable)
                || (weapon.Template.Location == EquipLocation.OneHand && (leftUsable || rightUsable));
        }
    }
}
