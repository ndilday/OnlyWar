using OnlyWar.Scripts.Models.Soldiers;

namespace OnlyWar.Scripts.Helpers
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

        private static int _nextId = 0;

        public Soldier GenerateNewSoldier(SoldierTemplate template)
        {
            Soldier soldier = new Soldier(template.Species.BodyTemplate)
            {
                Id = _nextId
            };
            _nextId++;

            soldier.Strength = template.Species.Strength.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Strength.StandardDeviation);
            soldier.Dexterity = template.Species.Dexterity.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Dexterity.StandardDeviation);
            soldier.Constitution = template.Species.Constitution.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Constitution.StandardDeviation);
            soldier.Ego = template.Species.Ego.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Ego.StandardDeviation);
            soldier.Charisma = template.Species.Charisma.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Charisma.StandardDeviation);
            soldier.Perception = template.Species.Perception.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Perception.StandardDeviation);
            soldier.Intelligence = template.Species.Intelligence.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Intelligence.StandardDeviation);

            soldier.AttackSpeed = template.Species.AttackSpeed.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.AttackSpeed.StandardDeviation);
            soldier.MoveSpeed = template.Species.MoveSpeed.BaseValue
                + (float)(RNG.NextGaussianDouble() * template.Species.MoveSpeed.StandardDeviation);
            soldier.Size = template.Species.Size.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.Size.StandardDeviation);
            soldier.PsychicPower = template.Species.PsychicPower.BaseValue 
                + (float)(RNG.NextGaussianDouble() * template.Species.PsychicPower.StandardDeviation);

            foreach (SkillTemplate skillTemplate in template.SkillTemplates)
            {
                float roll = skillTemplate.BaseValue 
                    + (float)(RNG.NextGaussianDouble() * skillTemplate.StandardDeviation);
                if(roll > 0)
                {
                    soldier.AddSkillPoints(skillTemplate.BaseSkill, roll);
                }
            }

            return soldier;
        }

        public Soldier[] GenerateNewSoldiers(int count, SoldierTemplate template)
        {
            Soldier[] soldierArray = new Soldier[count];
            for(int i = 0; i < count; i++)
            {
                soldierArray[i] = GenerateNewSoldier(template);
            }
            return soldierArray;
        }
    }
}
