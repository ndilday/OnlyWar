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
                if(resolution.Grid.IsEmpty(resolution.NewLocation))
                {
                    resolution.Grid.MoveSoldier(resolution.Soldier.Soldier.Id, resolution.NewLocation);
                    resolution.Soldier.Location = resolution.NewLocation;
                    if(resolution.NewLocation.Item1 < 0 || 
                       resolution.NewLocation.Item1 > resolution.Grid.GridWidth || 
                       resolution.NewLocation.Item2 < 0 || 
                       resolution.NewLocation.Item2 > resolution.Grid.GridHeight)
                    {
                        OnRetreat.Invoke(resolution.Soldier);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Soldier " + resolution.Soldier.Soldier.Name + " could not move to targeted position");
                }
            }
        }
    }
}
