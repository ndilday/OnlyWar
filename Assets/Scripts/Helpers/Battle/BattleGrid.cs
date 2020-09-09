using System;
using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Soldiers;

using UnityEngine;
using UnityEngine.Events;

namespace Iam.Scripts.Helpers.Battle
{
    public class BattleGrid
    {
        // I'm not using these events yet, but I probably will
        public UnityEvent<BattleSquad, Tuple<int, int>> OnSquadPlaced;
        public UnityEvent<BattleSquad, Tuple<int, int>> OnSquadMoved;
        // TODO: allow multiple friendlies in single grid location?
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        private readonly Dictionary<int, Tuple<int, int>> _soldierLocationMap;
        private readonly HashSet<int> _playerSoldierIds;
        private readonly HashSet<int> _opposingSoldierIds;

        public BattleGrid(int gridWidth, int gridHeight)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _soldierLocationMap = new Dictionary<int, Tuple<int, int>>();
        }

        public void RemoveSoldier(int soldierId)
        {
            _soldierLocationMap.Remove(soldierId);
        }

        public Tuple<int, int> GetSoldierPosition(int soldierId)
        {
            return _soldierLocationMap[soldierId];
        }

        public Tuple<int, int> MoveSoldier(int soldierId, Tuple<int, int> movement)
        {
            Tuple<int, int> currentLocation = _soldierLocationMap[soldierId];
            Tuple<int, int> newLocation = new Tuple<int, int>(currentLocation.Item1 + movement.Item1, currentLocation.Item2 + movement.Item2);
            _soldierLocationMap[soldierId] = newLocation;
            return newLocation;
        }

        public Tuple<Tuple<int, int>, Tuple<int, int>> GetSoldierBoxCorners(IEnumerable<BattleSoldier> soldiers)
        {
            int top = int.MinValue;
            int bottom = int.MaxValue;
            int left = int.MaxValue;
            int right = int.MinValue;

            foreach(BattleSoldier soldier in soldiers)
            {
                if(_soldierLocationMap.ContainsKey(soldier.Soldier.Id))
                {
                    var location = _soldierLocationMap[soldier.Soldier.Id];
                    if (location.Item1 < left) left = location.Item1;
                    if (location.Item1 > right) right = location.Item1;
                    if (location.Item2 > top) top = location.Item2;
                    if (location.Item2 < bottom) bottom = location.Item2;
                }
            }
            return new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(left, top), new Tuple<int, int>(right, bottom));
        }

        public float GetNearestEnemy(Soldier soldier, out int closestEnemy)
        {
            if (_soldierLocationMap.ContainsKey(soldier.Id))
            {
                var targetSet = _playerSoldierIds.Contains(soldier.Id) ? _opposingSoldierIds : _playerSoldierIds;
                var location = _soldierLocationMap[soldier.Id];
                closestEnemy = -1;
                float distanceSq = int.MaxValue;
                foreach (KeyValuePair<int, Tuple<int, int>> kvp in _soldierLocationMap)
                {
                    if (targetSet.Contains(kvp.Key))
                    {
                        float tempDistance = CalculateDistanceSq(location, kvp.Value);
                        if (tempDistance < distanceSq)
                        {
                            distanceSq = tempDistance;
                            closestEnemy = kvp.Key;
                        }
                    }
                }
                return Mathf.Sqrt(distanceSq);
            }
            throw new ArgumentException("Soldier not found");
        }

        public float GetDistanceBetweenSoldiers(int soldierId1, int soldierId2)
        {
            Tuple<int, int> pos1 = _soldierLocationMap[soldierId1];
            Tuple<int, int> pos2 = _soldierLocationMap[soldierId2];
            return Mathf.Sqrt(Mathf.Pow(pos1.Item1 - pos2.Item1, 2) + Mathf.Pow(pos1.Item2 - pos2.Item2, 2));
        }

        public void PlaceBattleSquad(BattleSquad squad, Tuple<int, int> bottomLeft)
        {
            // if any squad member is already on the map, we have a problem
            if (squad.Soldiers.Any(s => _soldierLocationMap.ContainsKey(s.Soldier.Id))) throw new InvalidOperationException(squad.Name + " has soldiers already on BattleGrid");
            if (squad.Soldiers.Count == 0) throw new InvalidOperationException("No soldiers in " + squad.Name + " to place");
            Tuple<int, int> squadBoxSize = squad.GetSquadBoxSize();
            Tuple<int, int> startingLocation = new Tuple<int, int>(bottomLeft.Item1 + ((squadBoxSize.Item1 - 1) / 2), bottomLeft.Item2 + squadBoxSize.Item2 - 1);
            for (int i = 0; i < squad.Soldiers.Count; i++)
            {
                // 0th soldier goes in the coordinate given, then alternate to each side up to membersPerRow, then repeat in additional rows as necessary
                int yMod = i / squadBoxSize.Item1 * (squad.IsPlayerSquad ? -1 : 1);
                int xMod = ((i % squadBoxSize.Item1) + 1) / 2 * (i % 2 == 0 ? -1 : 1);
                if (squad.IsPlayerSquad)
                {
                    _soldierLocationMap[squad.Soldiers[i].Soldier.Id] = new Tuple<int, int>(startingLocation.Item1 + xMod, startingLocation.Item2 + yMod);
                }
                else
                {
                    _soldierLocationMap[squad.Soldiers[i].Soldier.Id] = new Tuple<int, int>(startingLocation.Item1 + xMod, startingLocation.Item2 + yMod);
                }
            }
            OnSquadPlaced.Invoke(squad, startingLocation);
        }

        public bool IsEmpty(Tuple<int, int> location)
        {
            return !_soldierLocationMap.Values.Any(l => l == location);
        }

        public Tuple<int, int> GetClosestOpenAdjacency(Tuple<int, int> startingPoint, Tuple<int, int> target)
        {
            Tuple<int, int> bestPosition = null;
            float bestDistance = 10000;
            float disSq;
            Tuple<int, int>[] testPositions = new Tuple<int, int>[4]
                {
                    new Tuple<int, int>(target.Item1, target.Item2 - 1),
                    new Tuple<int, int>(target.Item1, target.Item2 + 1),
                    new Tuple<int, int>(target.Item1 - 1, target.Item2),
                    new Tuple<int, int>(target.Item1 + 1, target.Item2)
                };
            foreach (Tuple<int, int> testPosition in testPositions)
            {
                if (IsEmpty(testPosition))
                {
                    disSq = CalculateDistanceSq(startingPoint, testPosition);
                    if (disSq < bestDistance)
                    {
                        bestDistance = disSq;
                        bestPosition = testPosition;
                    }
                }
            }
            return bestPosition;
        }

        private float CalculateDistanceSq(Tuple<int, int> pos1, Tuple<int, int> pos2)
        {
            // for now, as a quick good-enough, just look at the difference in coordinates
            return Mathf.Pow(pos1.Item1 - pos2.Item1, 2) + Mathf.Pow(pos1.Item2 - pos2.Item2, 2);
        }
    }
}
