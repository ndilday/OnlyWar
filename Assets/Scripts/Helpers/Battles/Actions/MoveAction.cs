using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using OnlyWar.Helpers.Battles.Resolutions;

namespace OnlyWar.Helpers.Battles.Actions
{
    public class MoveAction : IAction
    {
        private readonly BattleSoldier _soldier;
        private readonly BattleGrid _grid;
        private readonly Tuple<int, int> _newTopLeft;
        private readonly ushort _orientation;
        private readonly ConcurrentBag<MoveResolution> _resultList;
        public MoveAction(BattleSoldier soldier, BattleGrid grid, Tuple<int, int> newTopLeft, ushort orientation, ConcurrentBag<MoveResolution> resultList)
        {
            _soldier = soldier;
            _grid = grid;
            _newTopLeft = newTopLeft;
            _orientation = orientation;
            _resultList = resultList;
        }

        public void Execute()
        {
            _resultList.Add(new MoveResolution(_soldier, _grid, _newTopLeft, _orientation));
            _soldier.TurnsShooting++;
        }
    }
}
