using System;
using Iam.Scripts.Models;

namespace Iam.Scripts.Helpers
{
    class SoldierFactory
    {
        public Soldier GenerateNewSoldier(int id, string yearString)
        {
            Soldier soldier = new Soldier();
            soldier.Id = id;
            soldier.FirstName = TempNameGenerator.GetName();
            soldier.LastName = TempNameGenerator.GetName();
            // let's assume, for now, something HERO system-ish
            soldier.Strength = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Dexterity = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Constitution = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Ego = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Presence = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Perception = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Intelligence = 10 + (float)(Gaussian.NextGaussianDouble() * 2);

            soldier.Melee = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Ranged = 20 + (float)(Gaussian.NextGaussianDouble());
            soldier.Speed = 20 + (float)(Gaussian.NextGaussianDouble());

            soldier.Piety = 10 + (float)(Gaussian.NextGaussianDouble() * 2);

            soldier.Piloting = 15 + (float)(Gaussian.NextGaussianDouble());
            soldier.Medicine = 10 + (float)(Gaussian.NextGaussianDouble());
            soldier.TechRepair = 10 + (float)(Gaussian.NextGaussianDouble());

            soldier.Body = new Body();

            double psychic = Gaussian.NextGaussianDouble();
            if (psychic > 2.5)
            {
                soldier.PsychicAbility = 10 + (float)(Gaussian.NextGaussianDouble());
                soldier.SoldierHistory.Add(yearString + ": psychic ability detected, acolyte training initiated");
            }
            else
            {
                soldier.PsychicAbility = 0;
            }

            EvaluateSoldier(soldier, yearString);

            return soldier;
        }

        public Soldier[] GenerateNewSoldiers(int count, string yearString)
        {
            Soldier[] soldierArray = new Soldier[count];
            for(int i = 0; i < count; i++)
            {
                soldierArray[i] = GenerateNewSoldier(i, yearString);
            }
            return soldierArray;
        }

        public void EvaluateSoldier(Soldier soldier, string yearString)
        {
            // Melee score = (Speed * STR * Melee)
            // Expected score = 20 * 20 * 20 = 1000
            // low-end = 19 * 19 * 19 = 850
            // high-end = 21 * 21 * 21 = 1150
            // elite = 22 * 22 * 22 = 1350
            // .729-1.331
            // shield of the emperor, sword of the emperor, Fury of the Emperor
            soldier.MeleeScore = soldier.Speed * soldier.Strength * soldier.Melee / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (soldier.MeleeScore > 1350) soldier.SoldierHistory.Add(yearString + ": Awarded Gold Sword of the Emperor badge during training");
            else if (soldier.MeleeScore > 1150) soldier.SoldierHistory.Add(yearString + ": Awarded Silver Sword of the Emperor badge during training");
            else if (soldier.MeleeScore > 1000) soldier.SoldierHistory.Add(yearString + ": Awarded Bronze Sword of the Emperor badge during training");
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            soldier.RangedScore = soldier.Perception * soldier.Ranged / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (soldier.RangedScore > 120) soldier.SoldierHistory.Add(yearString + ": Awarded Gold Marksman badge during training");
            else if (soldier.RangedScore > 110) soldier.SoldierHistory.Add(yearString + ": Awarded Silver Marksman badge during training");
            else if (soldier.RangedScore > 100) soldier.SoldierHistory.Add(yearString + ": Awarded Bronze Marksman badge during training");
            // Leadership Score = EGO * PRE
            soldier.LeadershipScore = soldier.Ego * soldier.Presence / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f)); ;
            if (soldier.LeadershipScore > 120) soldier.SoldierHistory.Add(yearString + ": Awarded Gold Voice of the Emperor badge during training");
            else if (soldier.LeadershipScore > 110) soldier.SoldierHistory.Add(yearString + ": Awarded Silver Voice of the Emperor badge during training");
            else if (soldier.LeadershipScore > 100) soldier.SoldierHistory.Add(yearString + ": Awarded Bronze Voice of the Emperor badge during training");
            // Ancient Score = EGO * BOD
            soldier.AncientScore = soldier.Ego * soldier.Constitution / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            if (soldier.AncientScore > 120) soldier.SoldierHistory.Add(yearString + ": Awarded Gold Banner of the Emperor badge during training");
            else if (soldier.AncientScore > 110) soldier.SoldierHistory.Add(yearString + ": Awarded Silver Banner of the Emperor badge during training");
            else if (soldier.AncientScore > 100) soldier.SoldierHistory.Add(yearString + ": Awarded Bronze Banner of the Emperor badge during training");
            // Medical Score = INT * Medicine
            soldier.MedicalScore = soldier.Intelligence * soldier.Medicine / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f)); ;
            if (soldier.MedicalScore > 125) soldier.SoldierHistory.Add(yearString + ": Flagged for potential training as Apothecary");
            // Tech Score =  INT * TechRapair
            soldier.TechScore = soldier.Intelligence * soldier.TechRepair / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            if (soldier.TechScore > 125) soldier.SoldierHistory.Add(yearString + ": Flagged for potential training as Techmarine");
            // Piety Score = PRE * Piety
            soldier.PietyScore = soldier.Presence * soldier.Piety / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(0.9f, 1.1f));
            if (soldier.PietyScore > 125) soldier.SoldierHistory.Add(yearString + ": Awarded Devout badge and declared a Novice");
        }
    }
}
