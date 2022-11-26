using System;
using System.Collections.Generic;


namespace OnlyWar.Helpers.Battles.Resolutions
{
    public class MoveResolution
    {
        public BattleSoldier Soldier { get; private set; }
        public BattleGrid Grid { get; private set; }
        public List<Tuple<int, int>> NewLocation { get; private set; }

        public MoveResolution(BattleSoldier soldier, BattleGrid grid, List<Tuple<int, int>> newLocation)
        {
            Soldier = soldier;
            Grid = grid;
            NewLocation = newLocation;
        }
    }
}
