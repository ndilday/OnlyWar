namespace OnlyWar.Models.Planets
{
    public class PlanetTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public int Probability { get; }
        public NormalizedValueTemplate PopulationRange { get; }
        public NormalizedValueTemplate ImportanceRange { get; }
        public LinearValueTemplate TaxRange { get; }

        public PlanetTemplate(int id, string name, int prob, NormalizedValueTemplate population,
                              NormalizedValueTemplate importance, LinearValueTemplate tax)
        {
            Id = id;
            Name = name;
            Probability = prob;
            PopulationRange = population;
            ImportanceRange = importance;
            TaxRange = tax;
        }
    }
}
