using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Fleets
{
    public class TempSpaceMarineShipTemplates
    {
        private static TempSpaceMarineShipTemplates _instance;
        private readonly Dictionary<int, ShipTemplate> _shipMap;
        private readonly Dictionary<int, BoatTemplate> _boatTemplates;

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
        public IReadOnlyDictionary<int, BoatTemplate> BoatTemplates { get => _boatTemplates; }

        private TempSpaceMarineShipTemplates()
        {
            _shipMap = new List<ShipTemplate>
            {
                new ShipTemplate(1, "Battle Barge", 300, 9, 30),
                new ShipTemplate(2, "Strike Cruiser", 100, 6, 10),
                new ShipTemplate(3, "Gladius Escort", 10, 0, 1),
                
            }.ToDictionary(st => st.Id);

            _boatTemplates = new List<BoatTemplate>
            {
                new BoatTemplate(4, "Thuderhawk Gunboat", 30)
            }.ToDictionary(bt => bt.Id);
        }
    }
}
