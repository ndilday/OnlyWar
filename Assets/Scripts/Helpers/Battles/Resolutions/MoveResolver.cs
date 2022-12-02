using System;
using System.Collections.Concurrent;

using UnityEngine;
using UnityEngine.Events;

namespace OnlyWar.Helpers.Battles.Resolutions
{
    public class MoveResolver : IResolver
    {
        public UnityEvent<BattleSoldier> OnRetreat;
        public ConcurrentBag<MoveResolution> MoveQueue { get; private set; }

        public MoveResolver()
        {
            MoveQueue = new ConcurrentBag<MoveResolution>();
            OnRetreat = new UnityEvent<BattleSoldier>();
        }

        public void Resolve()
        {
            while(!MoveQueue.IsEmpty)
            {
                MoveQueue.TryTake(out MoveResolution resolution);
                if(resolution.Grid.IsEmpty(resolution.TopLeft))
                {
                    resolution.Grid.MoveSoldier(resolution.Soldier, resolution.TopLeft, resolution.Orientation);
                    resolution.Soldier.TopLeft = resolution.TopLeft;
                    // TODO: need new retreat logic
                }
                else
                {
                    throw new InvalidOperationException("Soldier " + resolution.Soldier.Soldier.Name + " could not move to targeted position");
                }
            }
        }
    }
}
