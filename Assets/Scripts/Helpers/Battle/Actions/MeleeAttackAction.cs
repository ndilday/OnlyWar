using System;

namespace Iam.Scripts.Helpers.Battle.Actions
{
    public class MeleeAttackAction : IAction
    {
        BattleSoldier _attacker;
        BattleSoldier _target;
        bool _didMove;
        public MeleeAttackAction(BattleSoldier attacker, BattleSoldier target, bool didMove)
        {
            _attacker = attacker;
            _target = target;
            _didMove = didMove;
        }
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
