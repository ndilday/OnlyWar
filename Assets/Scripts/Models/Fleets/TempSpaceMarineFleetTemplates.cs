using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Fleets
{
    public class TempSpaceMarineFleetTemplates
    {
        private static TempSpaceMarineFleetTemplates _instance;
        private readonly Dictionary<int, FleetTemplate> _fleetTemplates;
        public static TempSpaceMarineFleetTemplates Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempSpaceMarineFleetTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, FleetTemplate> FleetTemplates { get => _fleetTemplates; }

        private TempSpaceMarineFleetTemplates()
        {
            _fleetTemplates = new List<FleetTemplate>
            {
                BuildDefaultSpaceMarineTemplate()
            }.ToDictionary(ft => ft.Id);
        }

        private FleetTemplate BuildDefaultSpaceMarineTemplate()
        {
            return new FleetTemplate(0, "Space Marine Fleet", new List<ShipTemplate>
            {
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[1],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[2],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[2],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[2],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[2],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[2],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3],
                TempSpaceMarineShipTemplates.Instance.ShipTemplates[3]
            });
        }
    }
}
