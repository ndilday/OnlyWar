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

        private readonly Dictionary<int, List<Tuple<int, int>>> _soldierLocationsMap;
        private readonly Dictionary<Tuple<int, int>, int> _locationSoldierMap;
        private readonly HashSet<int> _playerSoldierIds;
        private readonly HashSet<int> _opposingSoldierIds;
        private readonly HashSet<Tuple<int, int>> _reservedSpaces;

        public BattleGrid()
        {
            _soldierLocationsMap = new Dictionary<int, List<Tuple<int, int>>>();
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
            List<Tuple<int, int>> locations = _soldierLocationsMap[soldierId];
            _soldierLocationsMap.Remove(soldierId);
            foreach (Tuple<int, int> tuple in locations)
            {
                _locationSoldierMap.Remove(tuple);
            }
        }

        public List<Tuple<int, int>> GetSoldierPositions(int soldierId)
        {
            return _soldierLocationsMap[soldierId];
        }

        public void MoveSoldier(BattleSoldier soldier, Tuple<int, int> newTopLeft, ushort newOrienation)
        {
            List<Tuple<int, int>> currentLocation = _soldierLocationsMap[soldier.Soldier.Id];
            List<Tuple<int, int>> newLocation = new List<Tuple<int, int>>();
            int width, depth;
            if(newOrienation % 2 == 0)
            {
                width = soldier.Soldier.Template.Species.Width;
                depth = soldier.Soldier.Template.Species.Depth;
            }
            else
            {
                width = soldier.Soldier.Template.Species.Depth;
                depth = soldier.Soldier.Template.Species.Width;
            }

            for (int w = 0; w < width; w++)
            {
                for (int d = 0; d < depth; d++)
                {
                    Tuple<int, int> location = new Tuple<int, int>(newTopLeft.Item1 + w, newTopLeft.Item2 - d);
                    newLocation.Add(location);
                }
            }
            _soldierLocationsMap[soldier.Soldier.Id] = newLocation;
            foreach(Tuple<int, int> tuple in newLocation)
            {
                if(_locationSoldierMap.ContainsKey(tuple) && _locationSoldierMap[tuple] != soldier.Soldier.Id)
                {
                    throw new InvalidOperationException($"{soldier.Soldier.Id} cannot move to {tuple.Item1},{tuple.Item2}; already occupied by {_locationSoldierMap[tuple]}");
                }
                _locationSoldierMap[tuple] = soldier.Soldier.Id;
            }
            foreach (Tuple<int, int> tuple in currentLocation)
            {
                _locationSoldierMap.Remove(tuple);
            }
        }

        public Tuple<Tuple<int, int>, Tuple<int, int>> GetSoldierBoxCorners(IEnumerable<BattleSoldier> soldiers)
        {
            int top = int.MinValue;
            int bottom = int.MaxValue;
            int left = int.MaxValue;
            int right = int.MinValue;

            foreach(BattleSoldier soldier in soldiers)
            {
                if(_soldierLocationsMap.ContainsKey(soldier.Soldier.Id))
                {
                    var location = _soldierLocationsMap[soldier.Soldier.Id];
                    foreach (Tuple<int, int> tuple in location)
                    {
                        
                        if (tuple.Item1 < left) left = tuple.Item1;
                        if (tuple.Item1 > right) right = tuple.Item1;
                        if (tuple.Item2 > top) top = tuple.Item2;
                        if (tuple.Item2 < bottom) bottom = tuple.Item2;
                    }
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

            foreach(List<Tuple<int, int>> location in _soldierLocationsMap.Values)
            {
                foreach (Tuple<int, int> tuple in location)
                {
                    if (tuple.Item1 < minX) minX = tuple.Item1;
                    if (tuple.Item1 > maxX) maxX = tuple.Item1;
                    if (tuple.Item2 < minY) minY = tuple.Item2;
                    if (tuple.Item2 > maxY) maxY = tuple.Item2;
                }
            }

            return new Vector2(maxX - minX, maxY - minY);
        }

        public float GetNearestEnemy(int id, out int closestEnemy)
        {
            if (_soldierLocationsMap.ContainsKey(id))
            {
                var targetSet = _playerSoldierIds.Contains(id) ? _opposingSoldierIds : _playerSoldierIds;
                var location = _soldierLocationsMap[id];
                closestEnemy = -1;
                float distanceSq = int.MaxValue;
                foreach (KeyValuePair<int, List<Tuple<int, int>>> kvp in _soldierLocationsMap)
                {
                    if (targetSet.Contains(kvp.Key))
                    {
                        foreach (Tuple<int, int> tuple in kvp.Value)
                        {
                            foreach (Tuple<int, int> soldierTuple in location)
                            {
                                float tempDistance = CalculateDistanceSq(soldierTuple, tuple);
                                if (tempDistance < distanceSq)
                                {
                                    distanceSq = tempDistance;
                                    closestEnemy = kvp.Key;
                                }
                            }
                        }
                    }
                }
                return Mathf.Sqrt(distanceSq);
            }
            throw new ArgumentException("Soldier not found");
        }

        public float GetDistanceBetweenSoldiers(int soldierId1, int soldierId2)
        {
            List<Tuple<int, int>> pos1 = _soldierLocationsMap[soldierId1];
            List<Tuple<int, int>> pos2 = _soldierLocationsMap[soldierId2];
            float distanceSq = int.MaxValue;
            foreach (Tuple<int, int> tuple1 in pos1)
            {
                foreach (Tuple<int, int> tuple2 in pos2)
                {
                    float tempDistance = CalculateDistanceSq(tuple1, tuple2);
                    if (tempDistance < distanceSq)
                    {
                        distanceSq = tempDistance;
                    }
                }
            }
            return distanceSq;
        }

        public void PlaceBattleSquad(BattleSquad squad, Tuple<int, int> bottomLeft, bool longHorizontal)
        {
            // if any squad member is already on the map, we have a problem
            if (squad.Soldiers.Any(s => _soldierLocationsMap.ContainsKey(s.Soldier.Id))) throw new InvalidOperationException(squad.Name + " has soldiers already on BattleGrid");
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

        public bool IsEmpty(List<Tuple<int, int>> location)
        {
            foreach(Tuple<int, int> tuple in location)
            {
                if(!IsEmpty(tuple))
                {
                    return false;
                }
            }
            return true;
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
            List<Tuple<int, int>> locations = _soldierLocationsMap[soldierId];
            foreach (Tuple<int, int> location in locations)
            {
                Tuple<int, int>[] testPositions = new Tuple<int, int>[4]
                    {
                    new Tuple<int, int>(location.Item1, location.Item2 - 1),
                    new Tuple<int, int>(location.Item1, location.Item2 + 1),
                    new Tuple<int, int>(location.Item1 - 1, location.Item2),
                    new Tuple<int, int>(location.Item1 + 1, location.Item2)
                    };
                foreach (Tuple<int, int> testPosition in testPositions)
                {
                    if (_locationSoldierMap.ContainsKey(testPosition))
                    {
                        int adjacentSoldierId = _locationSoldierMap[testPosition];
                        if ((_playerSoldierIds.Contains(soldierId) && _opposingSoldierIds.Contains(adjacentSoldierId))
                            || _opposingSoldierIds.Contains(soldierId) && _playerSoldierIds.Contains(adjacentSoldierId))
                        {
                            return true;
                        }
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
                List<Tuple<int, int>> soldierLocations = new List<Tuple<int, int>>();
                for(int w = 0; w < width; w++)
                {
                    for(int d = 0; d < depth; d++)
                    {
                        Tuple<int, int> location = new Tuple<int, int>(startingLocation.Item1 + xMod + w, startingLocation.Item2 + yMod + d);
                        _locationSoldierMap[location] = squad.Soldiers[i].Soldier.Id;
                        soldierLocations.Add(location);
                    }
                }
                _soldierLocationsMap[squad.Soldiers[i].Soldier.Id] = soldierLocations;
                
                squad.Soldiers[i].TopLeft = GetTopLeft(soldierLocations);
                squad.Soldiers[i].Orientation = 0;
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
                List<Tuple<int, int>> soldierLocations = new List<Tuple<int, int>>();
                for (int w = 0; w < width; w++)
                {
                    for (int d = 0; d < depth; d++)
                    {
                        Tuple<int, int> location = new Tuple<int, int>(startingLocation.Item1 + xMod + w, startingLocation.Item2 + yMod + d);
                        _locationSoldierMap[location] = squad.Soldiers[i].Soldier.Id;
                        soldierLocations.Add(location);
                    }
                }
                _soldierLocationsMap[squad.Soldiers[i].Soldier.Id] = soldierLocations;

                squad.Soldiers[i].TopLeft = GetTopLeft(soldierLocations);
                squad.Soldiers[i].Orientation = 1;
            }

            return startingLocation;
        }

        private Tuple<int, int> GetTopLeft(List<Tuple<int, int>> tupleList)
        {
            Tuple<int, int> topLeft = null;
            foreach(Tuple<int, int> tuple in tupleList)
            {
                if(topLeft == null || (tuple.Item1 <= topLeft.Item1 && tuple.Item2 >= topLeft.Item2))
                {
                    topLeft = tuple;
                }
            }

            return topLeft;
        }
    }
}
