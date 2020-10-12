using System;
using System.Collections.Concurrent;
using OnlyWar.Scripts.Helpers.Battle.Resolutions;

namespace OnlyWar.Scripts.Helpers.Battle.Actions
{
    public class MoveAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly BattleGrid _grid;
        private readonly Tuple<int, int> _newLocation;
        private readonly ConcurrentBag<MoveResolution> _resultList;
        public MoveAction(BattleSoldier soldier, BattleGrid grid, Tuple<int, int> newLocation, ConcurrentBag<MoveResolution> resultList)
        {
            _soldier = soldier;
            _grid = grid;
            _newLocation = newLocation;
            _resultList = resultList;
        }

        public void Execute()
        {
            _resultList.Add(new MoveResolution(_soldier, _grid, _newLocation));
            _soldier.TurnsShooting++;
        }
    }
}
