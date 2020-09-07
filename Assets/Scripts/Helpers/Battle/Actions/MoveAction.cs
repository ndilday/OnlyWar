using System;

using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    public class MoveAction : IAction
    {
        private readonly Soldier _soldier;
        private readonly BattleGrid _grid;
        private readonly Tuple<int, int> _movement;
        private readonly bool _isPlayerSquad;
        public MoveAction(Soldier soldier, bool isPlayerSquad, BattleGrid grid, Tuple<int, int> movement)
        {
            _soldier = soldier;
            _grid = grid;
            _movement = movement;
            _isPlayerSquad = isPlayerSquad;
        }

        public void Execute()
        {
            _grid.MoveSoldier(_soldier.Id, _isPlayerSquad, _movement.Item1, _movement.Item2);
        }
    }
}
