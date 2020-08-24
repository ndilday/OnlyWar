using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers
{
    class SoldierFactory
    {
        private SoldierFactory() { }
        private static SoldierFactory _instance;
        public static SoldierFactory Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SoldierFactory();
                }
                return _instance;
            }
        }

        private int _nextId = 0;

        public SoldierType GenerateNewSoldier<SoldierType>(SoldierTemplate template) where SoldierType : Soldier, new()
        {
            SoldierType soldier = new SoldierType();
            soldier.InitializeBody(template.BodyTemplate);
            soldier.Id = _nextId;
            _nextId++;

            soldier.Strength = template.Strength.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Strength.StandardDeviation);
            soldier.Dexterity = template.Dexterity.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Dexterity.StandardDeviation);
            soldier.Constitution = template.Constitution.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Constitution.StandardDeviation);
            soldier.Ego = template.Ego.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Ego.StandardDeviation);
            soldier.Presence = template.Presence.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Presence.StandardDeviation);
            soldier.Perception = template.Perception.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Perception.StandardDeviation);
            soldier.Intelligence = template.Intelligence.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Intelligence.StandardDeviation);

            soldier.AttackSpeed = template.AttackSpeed.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.AttackSpeed.StandardDeviation);
            soldier.MoveSpeed = template.MoveSpeed.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.MoveSpeed.StandardDeviation);
            soldier.Size = template.Size.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Size.StandardDeviation);
            soldier.PsychicPower = template.PsychicPower.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.PsychicPower.StandardDeviation);

            foreach (SkillTemplate skillTemplate in template.SkillTemplates)
            {
                soldier.Skills[skillTemplate.BaseSkill.Id] = new Skill(skillTemplate.BaseSkill, 
                    skillTemplate.BaseValue + (float)(Gaussian.NextGaussianDouble() * skillTemplate.StandardDeviation) );
            }

            return soldier;
        }

        public SoldierType[] GenerateNewSoldiers<SoldierType>(int count, SoldierTemplate template) where SoldierType : Soldier, new()
        {
            SoldierType[] soldierArray = new SoldierType[count];
            for(int i = 0; i < count; i++)
            {
                soldierArray[i] = GenerateNewSoldier<SoldierType>(template);
            }
            return soldierArray;
        }
    }
}
