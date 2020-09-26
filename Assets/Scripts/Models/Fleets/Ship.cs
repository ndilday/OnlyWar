using Iam.Scripts.Models.Soldiers;
using System;
using System.Collections.Generic;

namespace Iam.Scripts.Models.Fleets
{
    public class Ship
    {
        public int Id { get; }
        private readonly List<ISoldier> _loadedSoldiers;
        public ShipTemplate Template { get; }
        public IReadOnlyCollection<ISoldier> LoadedSoldiers { get => _loadedSoldiers; } 

        public Ship(int id, ShipTemplate template)
        {
            Id = id;
            _loadedSoldiers = new List<ISoldier>();
            Template = template;
        }

        public void LoadSoldiers(IReadOnlyCollection<ISoldier> soldiers)
        {
            if(soldiers.Count + _loadedSoldiers.Count > Template.SoldierCapacity)
            {
                throw new InvalidOperationException("Trying to load too many soldiers onto the ship");
            }
            _loadedSoldiers.AddRange(soldiers);
        }

        public void UnloadAllSoldiers()
        {
            _loadedSoldiers.Clear();
        }
    }
}
