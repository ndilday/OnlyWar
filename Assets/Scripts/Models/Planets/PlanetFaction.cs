using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyWar.Scripts.Models.Planets
{
    public class PlanetFaction
    {
        public Faction Faction { get; }
        public bool IsPublic { get; set; }
        public long Population { get; set; }
        public int PDFMembers { get; set; }
        public float PlayerReputation { get; set; }

        public PlanetFaction(Faction faction)
        {
            Faction = faction;
        }
    }
}
