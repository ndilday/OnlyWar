using System;
using System.Collections.Concurrent;

using Iam.Scripts.Helpers.Battle.Resolutions;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    public class MeleeAttackAction : IAction
    {
        private readonly BattleSoldier _attacker;
        private readonly BattleSoldier _target;
        private readonly ConcurrentBag<WoundResolution> _resultList;
        private readonly bool _didMove;
        public MeleeAttackAction(BattleSoldier attacker, BattleSoldier target, bool didMove, ConcurrentBag<WoundResolution> resultList)
        {
            _attacker = attacker;
            _target = target;
            _didMove = didMove;
            _resultList = resultList;
        }
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
