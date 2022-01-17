using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

namespace OnlyWar.Helpers.Battles
{
    public class BattleGrid
    {
        // I'm not using these events yet, but I probably will
        public UnityEvent<BattleSquad, Tuple<int, int>> OnSquadPlaced;
        public UnityEvent<BattleSquad, Tuple<int, int>> OnSquadMoved;
        // TODO: allow multiple friendlies in single grid location?

        private readonly Dictionary<int, Tuple<int, int>> _soldierLocationMap;
        private readonly Dictionary<Tuple<int, int>, int> _locationSoldierMap;
        private readonly HashSet<int> _playerSoldierIds;
        private readonly HashSet<int> _opposingSoldierIds;
        private readonly HashSet<Tuple<int, int>> _reservedSpaces;

        public BattleGrid()
        {
            _soldierLocationMap = new Dictionary<int, Tuple<int, int>>();
            _locationSoldierMap = new Dictionary<Tuple<int, int>, int>();
            OnSquadPlaced = new UnityEvent<BattleSquad, Tuple<int, int>>();
            OnSquadMoved = new UnityEvent<BattleSquad, Tuple<int, int>>();
            _playerSoldierIds = new HashSet<int>();
            _opposingSoldierIds = new HashSet<int>();
            _reservedSpaces = new HashSet<Tuple<int, int>>();
        }

        public void RemoveSoldier(int soldierId)
        {
            if(_playerSoldierIds.Contains(soldierId))
            {
                _playerSoldierIds.Remove(soldierId);
            }
            else if(_opposingSoldierIds.Contains(soldierId))
            {
                _opposingSoldierIds.Remove(soldierId);
            }
            Tuple<int, int> tuple = _soldierLocationMap[soldierId];
            _soldierLocationMap.Remove(soldierId);
            _locationSoldierMap.Remove(tuple);
        }

        public Tuple<int, int> GetSoldierPosition(int soldierId)
        {
            return _soldierLocationMap[soldierId];
        }

        public void MoveSoldier(int soldierId, Tuple<int, int> newLocation)
        {
            Tuple<int, int> currentLocation = _soldierLocationMap[soldierId];
            _soldierLocationMap[soldierId] = newLocation;
            _locationSoldierMap[newLocation] = soldierId;
            _locationSoldierMap.Remove(currentLocation);
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

        public Vector2 GetCurrentGridSize()
        {
            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minY = int.MaxValue;
            int maxY = int.MinValue;

            foreach(Tuple<int, int> location in _soldierLocationMap.Values)
            {
                if (location.Item1 < minX) minX = location.Item1;
                if (location.Item1 > maxX) maxX = location.Item1;
                if (location.Item2 < minY) minY = location.Item2;
                if (location.Item2 > maxY) maxY = location.Item2;
            }

            return new Vector2(maxX - minX, maxY - minY);
        }

        public Tuple<Tuple<int, int>, Tuple<int, int>> GetSoldierBottomLeftAndSize(IEnumerable<BattleSoldier> soldiers)
        {
            int top = int.MinValue;
            int bottom = int.MaxValue;
            int left = int.MaxValue;
            int right = int.MinValue;

            foreach (BattleSoldier soldier in soldiers)
            {
                if (_soldierLocationMap.ContainsKey(soldier.Soldier.Id))
                {
                    var location = _soldierLocationMap[soldier.Soldier.Id];
                    if (location.Item1 < left) left = location.Item1;
                    if (location.Item1 > right) right = location.Item1;
                    if (location.Item2 > top) top = location.Item2;
                    if (location.Item2 < bottom) bottom = location.Item2;
                }
            }
            return new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(left, bottom), new Tuple<int, int>(right  + 1 - left, top + 1 - bottom));
        }

        public float GetNearestEnemy(int id, out int closestEnemy)
        {
            if (_soldierLocationMap.ContainsKey(id))
            {
                var targetSet = _playerSoldierIds.Contains(id) ? _opposingSoldierIds : _playerSoldierIds;
                var location = _soldierLocationMap[id];
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

        public void PlaceBattleSquad(BattleSquad squad, Tuple<int, int> bottomLeft, bool longHorizontal)
        {
            // if any squad member is already on the map, we have a problem
            if (squad.Soldiers.Any(s => _soldierLocationMap.ContainsKey(s.Soldier.Id))) throw new InvalidOperationException(squad.Name + " has soldiers already on BattleGrid");
            if (squad.Soldiers.Count == 0) throw new InvalidOperationException("No soldiers in " + squad.Name + " to place");
            Tuple<int, int> squadBoxSize = squad.GetSquadBoxSize();
            Tuple<int, int> startingLocation;
            if (longHorizontal)
            {
                startingLocation = PlaceSquadHorizontally(squad, bottomLeft, squadBoxSize);
            }
            else
            {
                startingLocation = PlaceSquadVertically(squad, bottomLeft, squadBoxSize);
            }
            OnSquadPlaced.Invoke(squad, startingLocation);
        }

        public bool IsEmpty(Tuple<int, int> location)
        {
            return !_locationSoldierMap.ContainsKey(location);
        }

        public Tuple<int, int> GetClosestOpenAdjacency(Tuple<int, int> startingPoint, Tuple<int, int> target)
        {
            Tuple<int, int> bestPosition = null;
            float bestDistance = 2500000;
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
                if (IsEmpty(testPosition) && !IsSpaceReserved(testPosition))
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

        public bool IsAdjacentToEnemy(int soldierId)
        {
            Tuple<int, int> location = _soldierLocationMap[soldierId];
            Tuple<int, int>[] testPositions = new Tuple<int, int>[4]
                {
                    new Tuple<int, int>(location.Item1, location.Item2 - 1),
                    new Tuple<int, int>(location.Item1, location.Item2 + 1),
                    new Tuple<int, int>(location.Item1 - 1, location.Item2),
                    new Tuple<int, int>(location.Item1 + 1, location.Item2)
                };
            foreach(Tuple<int, int> testPosition in testPositions)
            {
                if (_locationSoldierMap.ContainsKey(testPosition))
                {
                    int adjacentSoldierId = _locationSoldierMap[testPosition];
                    if((_playerSoldierIds.Contains(soldierId) && _opposingSoldierIds.Contains(adjacentSoldierId)) 
                        || _opposingSoldierIds.Contains(soldierId) && _playerSoldierIds.Contains(adjacentSoldierId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsSpaceReserved(Tuple<int, int> location)
        {
            return _reservedSpaces.Contains(location);
        }

        public void ReserveSpace(Tuple<int, int> location)
        {
            _reservedSpaces.Add(location);
        }

        public void ClearReservations()
        {
            _reservedSpaces.Clear();
        }

        private float CalculateDistanceSq(Tuple<int, int> pos1, Tuple<int, int> pos2)
        {
            // for now, as a quick good-enough, just look at the difference in coordinates
            return Mathf.Pow(pos1.Item1 - pos2.Item1, 2) + Mathf.Pow(pos1.Item2 - pos2.Item2, 2);
        }

        private Tuple<int, int> PlaceSquadHorizontally(BattleSquad squad, Tuple<int, int> bottomLeft, Tuple<int, int> squadBoxSize)
        {
            Tuple<int, int> startingLocation = new Tuple<int, int>(bottomLeft.Item1 + ((squadBoxSize.Item1 - 1) / 2), bottomLeft.Item2 + squadBoxSize.Item2 - 1);
            for (int i = 0; i < squad.Soldiers.Count; i++)
            {
                ushort width = squad.Soldiers[i].Soldier.Template.Species.Width;
                ushort depth = squad.Soldiers[i].Soldier.Template.Species.Depth;
                // 0th soldier goes in the coordinate given, then alternate to each side up to membersPerRow, then repeat in additional rows as necessary
                int yMod = i * depth / squadBoxSize.Item1 * (squad.IsPlayerSquad ? -1 : 1);
                int xMod = (((i * width) % squadBoxSize.Item1) + 1) / 2 * (i % 2 == 0 ? -1 : 1);
                if (squad.IsPlayerSquad)
                {
                    _playerSoldierIds.Add(squad.Soldiers[i].Soldier.Id);

                }
                else
                {
                    _opposingSoldierIds.Add(squad.Soldiers[i].Soldier.Id);
                }
                Tuple<int, int> location = new Tuple<int, int>(startingLocation.Item1 + xMod, startingLocation.Item2 + yMod);
                _soldierLocationMap[squad.Soldiers[i].Soldier.Id] = location;
                _locationSoldierMap[location] = squad.Soldiers[i].Soldier.Id;
                squad.Soldiers[i].Location = location;
            }

            return startingLocation;
        }

        private Tuple<int, int> PlaceSquadVertically(BattleSquad squad, Tuple<int, int> bottomLeft, Tuple<int, int> squadBoxSize)
        {
            Tuple<int, int> startingLocation = new Tuple<int, int>(bottomLeft.Item1 + squadBoxSize.Item2 - 1, 
                                                                   bottomLeft.Item2 + ((squadBoxSize.Item1 - 1) / 2));
            ushort width = squad.Soldiers[0].Soldier.Template.Species.Width;
            ushort depth = squad.Soldiers[0].Soldier.Template.Species.Depth;
            for (int i = 0; i < squad.Soldiers.Count; i++)
            {
                // 0th soldier goes in the coordinate given, then alternate to each side up to membersPerRow, then repeat in additional rows as necessary
                int xMod = i * depth / squadBoxSize.Item1 * (squad.IsPlayerSquad ? -1 : 1);
                int yMod = (((i * width) % squadBoxSize.Item1) + 1) / 2 * (i % 2 == 0 ? -1 : 1);
                if (squad.IsPlayerSquad)
                {
                    _playerSoldierIds.Add(squad.Soldiers[i].Soldier.Id);

                }
                else
                {
                    _opposingSoldierIds.Add(squad.Soldiers[i].Soldier.Id);
                }
                Tuple<int, int> location = new Tuple<int, int>(startingLocation.Item1 + xMod, startingLocation.Item2 + yMod);
                _soldierLocationMap[squad.Soldiers[i].Soldier.Id] = location;
                _locationSoldierMap[location] = squad.Soldiers[i].Soldier.Id;
                squad.Soldiers[i].Location = location;
            }

            return startingLocation;
        }
    }
}
