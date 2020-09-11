using System;
using System.Collections.Concurrent;

using UnityEngine;
using UnityEngine.Events;

namespace Iam.Scripts.Helpers.Battle.Resolutions
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
                Tuple<int, int> currentPosition = resolution.Grid.GetSoldierPosition(resolution.Soldier.Soldier.Id);
                Tuple<int, int> newPosition = new Tuple<int, int>(currentPosition.Item1 + resolution.Movement.Item1, currentPosition.Item2 + resolution.Movement.Item2);
                if(resolution.Grid.IsEmpty(newPosition))
                {
                    resolution.Grid.MoveSoldier(resolution.Soldier, resolution.Movement);
                    Tuple<int, int> newLocation = resolution.Soldier.Location;
                    if(newLocation.Item1 < 0 || newLocation.Item1 > resolution.Grid.GridWidth || newLocation.Item2 < 0 || newLocation.Item2 > resolution.Grid.GridHeight)
                    {
                        OnRetreat.Invoke(resolution.Soldier);
                    }
                }
                else
                {
                    // move one less until we find an empty spot
                    Debug.Log("Soldier " + resolution.Soldier.Soldier.ToString() + " could not move to targeted position");
                    if(resolution.Movement.Item1 > resolution.Movement.Item2)
                    {
                        do
                        {
                            Debug.Log("Soldier " + resolution.Soldier.Soldier.ToString() + " STILL could not move to targeted position");
                            newPosition = new Tuple<int, int>(newPosition.Item1 - 1, newPosition.Item2);
                        } while (!resolution.Grid.IsEmpty(newPosition));
                    }
                    else
                    {
                        do
                        {
                            Debug.Log("Soldier " + resolution.Soldier.Soldier.ToString() + " STILL could not move to targeted position");
                            newPosition = new Tuple<int, int>(newPosition.Item1, newPosition.Item2 - 1);
                        } while (!resolution.Grid.IsEmpty(newPosition));
                    }
                    Tuple<int, int> finalMovement = new Tuple<int, int>(newPosition.Item1 - currentPosition.Item1, newPosition.Item2 - currentPosition.Item2);
                    resolution.Grid.MoveSoldier(resolution.Soldier, finalMovement);
                    Tuple<int, int> newLocation = resolution.Soldier.Location;
                    if (newLocation.Item1 < 0 || newLocation.Item1 > resolution.Grid.GridWidth || newLocation.Item2 < 0 || newLocation.Item2 > resolution.Grid.GridHeight)
                    {
                        OnRetreat.Invoke(resolution.Soldier);
                    }
                }
            }
        }
    }
}
