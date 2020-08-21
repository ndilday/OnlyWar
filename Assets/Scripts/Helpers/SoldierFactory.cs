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

            soldier.Melee = template.Melee.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Melee.StandardDeviation);
            soldier.Ranged = template.Ranged.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.Ranged.StandardDeviation);
            soldier.AttackSpeed = template.AttackSpeed.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.AttackSpeed.StandardDeviation);
            soldier.MoveSpeed = template.MoveSpeed.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.MoveSpeed.StandardDeviation);
            soldier.Size = template.MoveSpeed.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.MoveSpeed.StandardDeviation);

            foreach (SkillTemplate skillTemplate in template.SkillTemplates)
            {
                soldier.Skills[skillTemplate.Name] = new Skill 
                { 
                    Id = skillTemplate.Id, 
                    Name = skillTemplate.Name, 
                    Value = skillTemplate.BaseValue + (float)(Gaussian.NextGaussianDouble() * template.MoveSpeed.StandardDeviation) 
                };
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
