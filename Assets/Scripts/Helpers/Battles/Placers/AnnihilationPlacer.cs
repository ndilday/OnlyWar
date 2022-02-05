using System;
using System.Collections.Generic;

using UnityEngine;

namespace OnlyWar.Helpers.Battles.Placers
{
    class AnnihilationPlacer
    {
        private readonly BattleGrid _grid;

        public AnnihilationPlacer(BattleGrid grid)
        {
            _grid = grid;
        }
        public Dictionary<BattleSquad, Vector2> PlaceSquads(IEnumerable<BattleSquad> bottomSquads, 
                                                            IEnumerable<BattleSquad> topSquads)
        {
            Dictionary<BattleSquad, Vector2> result = new();

            ArmyLayout bottomLayout = ArmyLayoutHelper.Instance.LayoutArmyLine(bottomSquads, true);
            ArmyLayout topLayout = ArmyLayoutHelper.Instance.LayoutArmyLine(topSquads, true);

            // determine distance between forces
            return result;
        }
    }
}
