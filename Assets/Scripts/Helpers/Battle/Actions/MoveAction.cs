using System;
using System.Collections.Concurrent;
using Iam.Scripts.Helpers.Battle.Resolutions;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    public class MoveAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly BattleGrid _grid;
        private readonly Tuple<int, int> _movement;
        private readonly ConcurrentBag<MoveResolution> _resultList;
        public MoveAction(BattleSoldier soldier, BattleGrid grid, Tuple<int, int> movement, ConcurrentBag<MoveResolution> resultList)
        {
            _soldier = soldier;
            _grid = grid;
            _movement = movement;
            _resultList = resultList;
        }

        public void Execute()
        {
            _resultList.Add(new MoveResolution(_soldier, _grid, _movement));
        }
    }
}
