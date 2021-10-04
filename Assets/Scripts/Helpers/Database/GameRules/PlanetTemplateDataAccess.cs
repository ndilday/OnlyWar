using OnlyWar.Models;
using OnlyWar.Models.Planets;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OnlyWar.Helpers.Database.GameRules
{
    public class PlanetTemplateDataAccess
    {
        public Dictionary<int, PlanetTemplate> GetData(IDbConnection connection)
        {
            Dictionary<int, PlanetTemplate> templateMap = new Dictionary<int, PlanetTemplate>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlanetTemplate";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader[1].ToString();
                    int probability = reader.GetInt32(2);
                    int popBase = reader.GetInt32(3);
                    float popStdDev = (float)reader[4];
                    int importanceBase = reader.GetInt32(5);
                    float importanceStdDev = (float)reader[6];
                    int taxMin = reader.GetInt32(7);
                    int taxMax = reader.GetInt32(8);

                    PlanetTemplate template = new PlanetTemplate(id, name, probability,
                        new NormalizedValueTemplate
                        {
                            BaseValue = popBase,
                            StandardDeviation = popStdDev
                        },
                        new NormalizedValueTemplate
                        {
                            BaseValue = importanceBase,
                            StandardDeviation = importanceStdDev
                        },
                        new LinearValueTemplate
                        {
                            MinValue = taxMin,
                            MaxValue = taxMax
                        });

                    templateMap[id] = template;
                }
            }
            return templateMap;
        }
    }
}
