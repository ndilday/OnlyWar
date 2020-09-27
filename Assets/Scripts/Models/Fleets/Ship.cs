using Iam.Scripts.Models.Squads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Fleets
{
    public class Boat
    {
        private readonly List<Squad> _loadedSquads;
        private static int _idGenerator = 10000;
        public int Id { get; }
        public string Name { get; }
        public BoatTemplate Template { get; }
        public IReadOnlyCollection<Squad> LoadedSoldiers { get => _loadedSquads; }

        public Boat(BoatTemplate template)
        {
            Id = _idGenerator++;
            Name = $"{template.ClassName}-{Id}";
            _loadedSquads = new List<Squad>();
            Template = template;
        }

        public void LoadSquad(Squad squad)
        {
            int loadedCount = _loadedSquads.Sum(ls => ls.Members.Count);
            if (squad.Members.Count + loadedCount > Template.SoldierCapacity)
            {
                throw new InvalidOperationException("Trying to load too many soldiers onto the ship");
            }
            _loadedSquads.Add(squad);
        }
    }

    public class Ship
    {
        private readonly List<Squad> _loadedSquads;

        public int Id { get; }
        public string Name { get; }
        public ShipTemplate Template { get; }
        public IReadOnlyCollection<Squad> LoadedSquads { get => _loadedSquads; } 
        public List<Boat> Boats { get; }
        public int LoadedSoldierCount { get; private set; }

        public Ship(int id, string name, ShipTemplate template)
        {
            Id = id;
            Name = name;
            _loadedSquads = new List<Squad>();
            LoadedSoldierCount = 0;
            Template = template;
            BoatTemplate boatTemplate = TempSpaceMarineShipTemplates.Instance.BoatTemplates[4];
            Boats = new List<Boat>();
            for (byte i = 0; i < Template.BoatCapacity; i++)
            {
                Boats.Add(new Boat(boatTemplate));
            }
        }

        public void LoadSquad(Squad squad)
        {
            int count = squad.Members.Count;
            if (count + LoadedSoldierCount > Template.SoldierCapacity)
            {
                throw new InvalidOperationException("Trying to load too many soldiers onto the ship");
            }
            _loadedSquads.Add(squad);
            LoadedSoldierCount += count;
        }

        public void RemoveSquad(Squad squad)
        {
            _loadedSquads.Remove(squad);
            LoadedSoldierCount -= squad.Members.Count;
        }

        public void UnloadAllSquads()
        {
            LoadedSoldierCount = 0;
            _loadedSquads.Clear();
        }
    }
}
