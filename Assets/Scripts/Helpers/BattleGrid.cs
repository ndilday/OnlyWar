using Iam.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Helpers
{
    public class BattleGrid
    {
        // TODO: allow multiple friendlies in single grid location?
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }
        private Dictionary<Tuple<int, int>, BattleSquad> _playerForce;
        private Dictionary<Tuple<int, int>, BattleSquad> _opposingForce;
        private Dictionary<BattleSquad, Tuple<int, int>> _playerSquadLocationMap;
        private Dictionary<BattleSquad, Tuple<int, int>> _opposingSquadLocationMap;

        public BattleGrid(int gridWidth, int gridHeight)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _playerForce = new Dictionary<Tuple<int, int>, BattleSquad>();
            _opposingForce = new Dictionary<Tuple<int, int>, BattleSquad>();
        }

        public bool PlacePlayerSquad(BattleSquad squad, int x, int y)
        {
            return PlaceSquad(squad, x, y, _playerForce, _playerSquadLocationMap);
        }

        public bool PlaceOpposingSquad(BattleSquad squad, int x, int y)
        {
            return PlaceSquad(squad, x, y, _opposingForce, _opposingSquadLocationMap);
        }

        public bool MoveSquad(BattleSquad squad, bool isPlayerSquad, int xMovement, int yMovement)
        {
            if(isPlayerSquad && !_playerSquadLocationMap.ContainsKey(squad))
            {
                return false;
            }
            else if(!isPlayerSquad && !_opposingSquadLocationMap.ContainsKey(squad))
            {
                return false;
            }
            else if(isPlayerSquad)
            {
                Tuple<int, int> currentLocation = _playerSquadLocationMap[squad];
                Tuple<int, int> newLocation = new Tuple<int, int>(currentLocation.Item1 + xMovement, currentLocation.Item2 + yMovement);
                _playerSquadLocationMap[squad] = newLocation;
                _playerForce.Remove(currentLocation);
                _playerForce[newLocation] = squad;
                return true;
            }
            else
            {
                Tuple<int, int> currentLocation = _opposingSquadLocationMap[squad];
                Tuple<int, int> newLocation = new Tuple<int, int>(currentLocation.Item1 + xMovement, currentLocation.Item2 + yMovement);
                _opposingSquadLocationMap[squad] = newLocation;
                _opposingForce.Remove(currentLocation);
                _opposingForce[newLocation] = squad;
                return true;
            }
        }


        private bool PlaceSquad(BattleSquad squad, int x, int y, Dictionary<Tuple<int, int>, BattleSquad> squadMap, Dictionary<BattleSquad, Tuple<int, int>> squadLocationMap)
        {
            Tuple<int, int> location = new Tuple<int, int>(x, y);
            if (squadMap.ContainsKey(location)) return false;
            squadMap[location] = squad;
            squadLocationMap[squad] = location;
            return true;
        }

        public float GetNearestEnemy(BattleSquad squad, bool isPlayerSquad, out KeyValuePair<Tuple<int, int>, BattleSquad> closestEnemy)
        {
            if(isPlayerSquad && _playerSquadLocationMap.ContainsKey(squad))
            {
                return GetNearestUnit(_playerSquadLocationMap[squad], _opposingForce, out closestEnemy);
            }
            else if(!isPlayerSquad && _opposingSquadLocationMap.ContainsKey(squad))
            {
                return GetNearestUnit(_opposingSquadLocationMap[squad], _playerForce, out closestEnemy);
            }
            throw new ArgumentException("Squad not found");
        }

        private float GetNearestUnit(Tuple<int, int> location, Dictionary<Tuple<int, int>, BattleSquad> locationSquadMap, out KeyValuePair<Tuple<int, int>, BattleSquad> closestUnit)
        {
            float distance = int.MaxValue;
            foreach (KeyValuePair<Tuple<int, int>, BattleSquad> kvp in locationSquadMap)
            {
                
                float tempDistance = CalculateDistance(location, kvp.Key);
                if(tempDistance < distance)
                {
                    distance = tempDistance;
                    closestUnit = kvp;
                }
            }
            return distance;
        }

        private float CalculateDistance(Tuple<int, int> pos1, Tuple<int, int> pos2)
        {
            // for now, as a quick good-enough, just look at the difference in coordinates
            return Math.Abs(pos1.Item1 - pos2.Item1) + Math.Abs(pos1.Item2 - pos2.Item2);
        }
    }
}
