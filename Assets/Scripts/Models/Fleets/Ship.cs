using Iam.Scripts.Models.Soldiers;
using System;
using System.Collections.Generic;

namespace Iam.Scripts.Models.Fleets
{
    public class Boat
    {
        private readonly List<ISoldier> _loadedSoldiers;
        private static int _idGenerator = 10000;
        public int Id { get; }
        public string Name { get; }
        public BoatTemplate Template { get; }
        public IReadOnlyCollection<ISoldier> LoadedSoldiers { get => _loadedSoldiers; }

        public Boat(BoatTemplate template)
        {
            Id = _idGenerator++;
            Name = $"{template.ClassName}-{Id}";
            _loadedSoldiers = new List<ISoldier>();
            Template = template;
        }

        public void LoadSoldiers(IReadOnlyCollection<ISoldier> soldiers)
        {
            if (soldiers.Count + _loadedSoldiers.Count > Template.SoldierCapacity)
            {
                throw new InvalidOperationException("Trying to load too many soldiers onto the ship");
            }
            _loadedSoldiers.AddRange(soldiers);
        }
    }

    public class Ship
    {
        private readonly List<ISoldier> _loadedSoldiers;

        public int Id { get; }
        public ShipTemplate Template { get; }
        public IReadOnlyCollection<ISoldier> LoadedSoldiers { get => _loadedSoldiers; } 
        public List<Boat> Boats { get; }

        public Ship(int id, ShipTemplate template)
        {
            Id = id;
            _loadedSoldiers = new List<ISoldier>();
            Template = template;
            BoatTemplate boatTemplate = TempSpaceMarineShipTemplates.Instance.BoatTemplates[4];
            Boats = new List<Boat>();
            for (byte i = 0; i < Template.BoatCapacity; i++)
            {
                Boats.Add(new Boat(boatTemplate));
            }
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
