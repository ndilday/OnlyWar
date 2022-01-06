using OnlyWar.Models.Squads;
using System.Collections.Generic;

namespace OnlyWar.Models.Planets
{
    public class PlanetFaction
    {
        public Faction Faction { get; }
        public bool IsPublic { get; set; }
        public long Population { get; set; }
        public int PDFMembers { get; set; }
        public float PlayerReputation { get; set; }
        public int PlanetaryControl { get; set; }
        public List<Squad> LandedSquads { get; set; }
        public Character Leader { get; set; }

        public PlanetFaction(Faction faction)
        {
            Faction = faction;
            LandedSquads = new List<Squad>();
            IsPublic = true;
            Population = 0;
            PDFMembers = 0;
            PlayerReputation = 0;
            PlanetaryControl = 0;
        }
    }
}
