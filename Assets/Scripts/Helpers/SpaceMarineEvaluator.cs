using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers
{
    public class SpaceMarineEvaluator
    {
        private static SpaceMarineEvaluator _instance;
        
        public static SpaceMarineEvaluator Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SpaceMarineEvaluator();
                }
                return _instance;
            }
        }

        private SpaceMarineEvaluator() { }

        public void EvaluateMarine(SpaceMarine marine)
        {
            // Melee score = (Speed * STR * Melee)
            // Expected score = 16 * 16 * 15.5/8 = 1000
            // low-end = 15 * 15 * 14/8 = 850
            // high-end = 17 * 17 * 16/8 = 578
            marine.MeleeScore = marine.AttackSpeed * marine.Strength 
                * marine.GetTotalSkillValue(TempBaseSkillList.Instance.Sword) /
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            Skill bestRanged = marine.GetBestSkillByCategory(SkillCategory.Ranged);
            marine.RangedScore = marine.Perception * (marine.Dexterity + bestRanged.SkillBonus) / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Leadership Score = EGO * Leadership * Tactics
            marine.LeadershipScore = marine.Ego 
                * marine.GetTotalSkillValue(TempBaseSkillList.Instance.Leadership)
                * marine.GetTotalSkillValue(TempBaseSkillList.Instance.Tactics)
                / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Ancient Score = EGO * BOD
            marine.AncientScore = marine.Ego * marine.Constitution / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Medical Score = INT * Medicine
            marine.MedicalScore = marine.GetTotalSkillValue(TempBaseSkillList.Instance.Diagnosis)
                * marine.GetTotalSkillValue(TempBaseSkillList.Instance.FirstAid)
                / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Tech Score =  INT * TechRapair
            marine.TechScore = marine.GetTotalSkillValue(TempBaseSkillList.Instance.ArmorySmallArms)
                * marine.GetTotalSkillValue(TempBaseSkillList.Instance.ArmoryVehicle) 
                / (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Piety Score = Piety * Ritual * Persuade
            marine.PietyScore = marine.GetTotalSkillValue(TempBaseSkillList.Instance.Piety) 
                / UnityEngine.Random.Range(0.09f, 0.11f);
        }
    }
}
