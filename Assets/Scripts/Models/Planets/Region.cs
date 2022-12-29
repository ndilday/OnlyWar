using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyWar.Models.Planets
{
    public class Region
    {
        public readonly Planet Planet;
        public readonly Tuple<ushort, ushort> Position;
    }
}
