using System.Linq;

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

        public void EvaluateSoldier(Date trainingStartDate)
        {
            FirstName = TempNameGenerator.GetName();
            LastName = TempNameGenerator.GetName();
            if(Skills["Psychic Power"].Value > 0)
            {
                SoldierHistory.Add(trainingStartDate + ": psychic ability detected, acolyte training initiated");
            }
            Date trainingFinishedYear = new Date(trainingStartDate.Millenium, trainingStartDate.Year + 2, trainingStartDate.Week);
            // Melee score = (Speed * STR * Melee)
            // Expected score = 20 * 20 * 20 = 1000
            // low-end = 19 * 19 * 19 = 850
            // high-end = 21 * 21 * 21 = 1150
            // elite = 22 * 22 * 22 = 1350
            MeleeScore = AttackSpeed * Strength * Melee / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (MeleeScore > 1350) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Sword of the Emperor badge during training");
            else if (MeleeScore > 1150) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Sword of the Emperor badge during training");
            else if (MeleeScore > 1000) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Sword of the Emperor badge during training");
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            RangedScore = Perception * Ranged / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (RangedScore > 120) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Marksman badge during training");
            else if (RangedScore > 110) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Marksman badge during training");
            else if (RangedScore > 100) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Marksman badge during training");
            // Leadership Score = EGO * PRE
            LeadershipScore = Ego * Presence / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f)); ;
            if (LeadershipScore > 120) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Voice of the Emperor badge during training");
            else if (LeadershipScore > 110) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Voice of the Emperor badge during training");
            else if (LeadershipScore > 100) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Voice of the Emperor badge during training");
            // Ancient Score = EGO * BOD
            AncientScore = Ego * Constitution / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (AncientScore > 120) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Gold Banner of the Emperor badge during training");
            else if (AncientScore > 110) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Silver Banner of the Emperor badge during training");
            else if (AncientScore > 100) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Bronze Banner of the Emperor badge during training");
            // Medical Score = INT * Medicine
            MedicalScore = Intelligence * Skills["Medicine"].Value / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f)); ;
            if (MedicalScore > 125) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Flagged for potential training as Apothecary");
            // Tech Score =  INT * TechRapair
            TechScore = Intelligence * Skills["TechRepair"].Value / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            if (TechScore > 125) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Flagged for potential training as Techmarine");
            // Piety Score = PRE * Piety
            PietyScore = Presence * Skills["Piety"].Value / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(0.9f, 1.1f));
            if (PietyScore > 125) SoldierHistory.Add(trainingFinishedYear.ToString() + ": Awarded Devout badge and declared a Novice");
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
