using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Fleets
{
    public class TempSpaceMarineShipTemplates
    {
        private static TempSpaceMarineShipTemplates _instance;
        private readonly Dictionary<int, ShipTemplate> _shipMap;

        public static TempSpaceMarineShipTemplates Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempSpaceMarineShipTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, ShipTemplate> ShipTemplates { get => _shipMap;}

        private TempSpaceMarineShipTemplates()
        {
            _shipMap = new List<ShipTemplate>
            {
                new ShipTemplate(1, "Battle Barge", 300),
                new ShipTemplate(2, "Strike Cruiser", 100),
                new ShipTemplate(3, "Gladius Escort", 10),
                new ShipTemplate(4, "Thunderhawk Guship", 30),
                new ShipTemplate(5, "Drop Pod", 12),
                new ShipTemplate(6, "Caestus Assault Ram", 10)
            }.ToDictionary(st => st.Id);
        }
    }
}
