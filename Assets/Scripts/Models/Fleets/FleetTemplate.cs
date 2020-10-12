using System.Collections.Generic;

namespace OnlyWar.Scripts.Models.Fleets
{
    public class FleetTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public IReadOnlyCollection<ShipTemplate> Ships { get; }
        public FleetTemplate(int id, string name, IReadOnlyCollection<ShipTemplate> ships)
        {
            Id = id;
            Name = name;
            Ships = ships;
        }
    }
}
