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
            marine.MeleeScore = marine.AttackSpeed * marine.Strength * (marine.Dexterity + marine.Skills[TempBaseSkillList.Instance.Sword.Id].SkillBonus) /
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // marksman, sharpshooter, sniper
            // Ranged Score = PER * Ranged
            Skill bestRanged = marine.GetBestRangedSkill();
            marine.RangedScore = marine.Perception * (marine.Dexterity + bestRanged.SkillBonus) / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Leadership Score = EGO * Leadership * Tactics
            marine.LeadershipScore = marine.Ego * (marine.Presence + marine.Skills[TempBaseSkillList.Instance.Leadership.Id].SkillBonus) *
                (marine.Intelligence + marine.Skills[TempBaseSkillList.Instance.Tactics.Id].SkillBonus) /
                (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Ancient Score = EGO * BOD
            marine.AncientScore = marine.Ego * marine.Constitution / (UnityEngine.Random.Range(1.8f, 2.2f) * UnityEngine.Random.Range(1.8f, 2.2f));
            // Medical Score = INT * Medicine
            marine.MedicalScore = (marine.Intelligence + marine.Skills[TempBaseSkillList.Instance.Diagnosis.Id].SkillBonus) *
                (marine.Intelligence + marine.Skills[TempBaseSkillList.Instance.FirstAid.Id].SkillBonus) /
                (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f)); ;
            // Tech Score =  INT * TechRapair
            marine.TechScore = (marine.Intelligence + marine.Skills[TempBaseSkillList.Instance.ArmorySmallArms.Id].SkillBonus) *
                (marine.Intelligence + marine.Skills[TempBaseSkillList.Instance.ArmoryVehicle.Id].SkillBonus) /
                (UnityEngine.Random.Range(0.9f, 1.1f) * UnityEngine.Random.Range(0.9f, 1.1f));
            // Piety Score = Piety * Ritual * Persuade
            marine.PietyScore = (marine.Presence + marine.Skills[TempBaseSkillList.Instance.Piety.Id].SkillBonus) / UnityEngine.Random.Range(0.09f, 0.11f);
        }
    }
}
