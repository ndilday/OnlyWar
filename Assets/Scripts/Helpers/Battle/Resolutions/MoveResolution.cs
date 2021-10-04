using System;


namespace OnlyWar.Helpers.Battle.Resolutions
{
    public class MoveResolution
    {
        public BattleSoldier Soldier { get; private set; }
        public BattleGrid Grid { get; private set; }
        public Tuple<int, int> NewLocation { get; private set; }

        public MoveResolution(BattleSoldier soldier, BattleGrid grid, Tuple<int, int> newLocation)
        {
            Soldier = soldier;
            Grid = grid;
            NewLocation = newLocation;
        }
    }
}
