using System;

using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    public class MoveAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly BattleGrid _grid;
        private readonly Tuple<int, int> _movement;
        public MoveAction(BattleSoldier soldier, BattleGrid grid, Tuple<int, int> movement)
        {
            _soldier = soldier;
            _grid = grid;
            _movement = movement;
        }

        public void Execute()
        {
            _grid.MoveSoldier(_soldier.Soldier.Id, _movement.Item1, _movement.Item2);
        }
    }
}
