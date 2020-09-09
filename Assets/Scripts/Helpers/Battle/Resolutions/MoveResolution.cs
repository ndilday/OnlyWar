using System;


namespace Iam.Scripts.Helpers.Battle.Resolutions
{
    public class MoveResolution
    {
        public BattleSoldier Soldier { get; private set; }
        public BattleGrid Grid { get; private set; }
        public Tuple<int, int> Movement { get; private set; }

        public MoveResolution(BattleSoldier soldier, BattleGrid grid, Tuple<int, int> movement)
        {
            Soldier = soldier;
            Grid = grid;
            Movement = movement;
        }
    }
}
