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
        public bool Population { get; set; }
    }
}
