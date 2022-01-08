using System.Collections.Generic;
using OnlyWar.Helpers.Battle;
using OnlyWar.Models.Planets;

namespace OnlyWar.Models.Battles
{
    public class BattleConfiguration
    {
        public IReadOnlyList<BattleSquad> PlayerSquads;
        public IReadOnlyList<BattleSquad> OpposingSquads;
        public Planet Planet;
        public BattleGrid Grid;
    }
}
