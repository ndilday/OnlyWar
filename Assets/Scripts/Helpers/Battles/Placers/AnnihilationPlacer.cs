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

            // TODO: determine distance between forces
            // we should probably base this on weapon ranges of the respective armies
            // for now, we'll just go with 500 yards
            // TODO: exclude crippled soldiers from being deployed
            foreach (KeyValuePair<int, BattleSquadLayout> squadLayoutMapItem in topLayout.SquadLayoutMap)
            {
            }

            foreach (KeyValuePair<int, BattleSquadLayout> squadLayoutMapItem in bottomLayout.SquadLayoutMap)
            {
            }
            // TODO: place armies based on distance calculation
            //
            return result;
        }
    }
}
