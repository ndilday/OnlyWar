using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Faction
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Color Color { get; private set; }
        public Faction(int id, string name, Color color)
        {
            Id = id;
            Name = name;
            Color = color;
        }
    }

    public class TempFactions
    {
        private static TempFactions _instance;
        public static TempFactions Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempFactions();
                }
                return _instance;
            }
        }

        public Faction SpaceMarines;
        public Faction Tyranids;

        private TempFactions()
        {
            SpaceMarines = new Faction(0, "Space Marines", Color.blue);
            Tyranids = new Faction(1, "Tyranids", Color.magenta);
        }
    }
}
