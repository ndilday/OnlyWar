using Iam.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Soldiers;
using UnityEngine;

namespace Iam.Scripts.Helpers
{
    public class BattleGrid
    {
        // TODO: allow multiple friendlies in single grid location?
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        private Dictionary<int, Tuple<int, int>> _playerSoldierLocationMap;
        private Dictionary<int, Tuple<int, int>> _opposingSoldierLocationMap;

        public BattleGrid(int gridWidth, int gridHeight)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
            _playerSoldierLocationMap = new Dictionary<int, Tuple<int, int>>();
            _opposingSoldierLocationMap = new Dictionary<int, Tuple<int, int>>();
        }

        public bool PlacePlayerSquad(BattleSquad squad, int x, int y)
        {
            return PlaceSquad(squad, x, y, false);
        }

        public bool PlaceOpposingSquad(BattleSquad squad, int x, int y)
        {
            return PlaceSquad(squad, x, y, true);
        }

        public bool MoveSquad(BattleSquad squad, int xMovement, int yMovement)
        {
            Dictionary<int, Tuple<int, int>> soldierLocationMap = squad.IsPlayerSquad ? _playerSoldierLocationMap : _opposingSoldierLocationMap;
            // if any of the squad members aren't on the map, we have a problem
            if (squad.Squad.Any(s => !soldierLocationMap.ContainsKey(s.Id))) throw new ArgumentException("Soldier in squad " + squad.Name + " not on BattleGrid");
            foreach(Soldier soldier in squad.Squad)
            {
                Tuple<int, int> currentLocation = soldierLocationMap[soldier.Id];
                Tuple<int, int> newLocation = new Tuple<int, int>(currentLocation.Item1 + xMovement, currentLocation.Item2 + yMovement);
            }
            return true;
        }

        public Tuple<Tuple<int, int>, Tuple<int, int>> GetSquadBox(BattleSquad squad)
        {
            int top = int.MinValue;
            int bottom = int.MaxValue;
            int left = int.MaxValue;
            int right = int.MinValue;
            Dictionary<int, Tuple<int, int>> soldierLocationMap = squad.IsPlayerSquad ? _playerSoldierLocationMap : _opposingSoldierLocationMap;

            foreach(Soldier soldier in squad.Squad)
            {
                if(soldierLocationMap.ContainsKey(soldier.Id))
                {
                    var location = soldierLocationMap[soldier.Id];
                    if (location.Item1 < left) left = location.Item1;
                    if (location.Item1 > right) right = location.Item1;
                    if (location.Item2 > top) top = location.Item2;
                    if (location.Item2 < bottom) bottom = location.Item2;
                }
            }
            return new Tuple<Tuple<int, int>, Tuple<int, int>>(new Tuple<int, int>(left, top), new Tuple<int, int>(right, bottom));
        }

        public float GetNearestEnemy(Soldier soldier, bool isPlayerSquad, out int closestEnemy)
        {
            if (isPlayerSquad && _playerSoldierLocationMap.ContainsKey(soldier.Id))
            {
                return GetNearestTarget(_playerSoldierLocationMap[soldier.Id], _opposingSoldierLocationMap, out closestEnemy);
            }
            else if (!isPlayerSquad && _opposingSoldierLocationMap.ContainsKey(soldier.Id))
            {
                return GetNearestTarget(_opposingSoldierLocationMap[soldier.Id], _playerSoldierLocationMap, out closestEnemy);
            }
            throw new ArgumentException("Squad not found");
        }

        private bool PlaceSquad(BattleSquad squad, int x, int y, bool flipDirection)
        {
            Dictionary<int, Tuple<int, int>> soldierLocationMap = squad.IsPlayerSquad ? _playerSoldierLocationMap : _opposingSoldierLocationMap;
            // if any squad member is already on the map, we have a problem
            if (squad.Squad.Any(s => soldierLocationMap.ContainsKey(s.Id))) throw new InvalidOperationException("Soldier in squad " + squad.Name + " already on BattleGrid");
            if (squad.Squad.Length == 0) throw new InvalidOperationException(squad.Name + " has no soldiers to place");
            Tuple<int, int> startingLocation = new Tuple<int, int>(x, y);
            // numberOfRows is how many rows of soldiers make up the formation
            int numberOfRows = 1;
            if(squad.Squad.Length >= 30)
            {
                numberOfRows = 3;
            }
            if(squad.Squad.Length > 7)
            {
                numberOfRows = 2;
            }
            // membersPerRow is how many soldiers are in each row (back row may be smaller)
            int membersPerRow = Mathf.CeilToInt((float)(squad.Squad.Length) / (float)(numberOfRows));
            for(int i = 0; i < squad.Squad.Length; i++)
            {
                // 0th soldier goes in the coordinate given, then alternate to each side up to membersPerRow, then repeat in additional rows as necessary
                int yMod = i / numberOfRows * (flipDirection ? -1 : 1);
                int xMod = ((i % membersPerRow) + 1) / 2 * (i % 2 == 0 ? 1 : -1);
                if (squad.IsPlayerSquad)
                {
                    _playerSoldierLocationMap[squad.Squad[i].Id] = new Tuple<int, int>(x + xMod, y + yMod);
                }
                else
                {
                    _opposingSoldierLocationMap[squad.Squad[i].Id] = new Tuple<int, int>(x + xMod, y + yMod);
                }
            }
            return true;
        }

        private float GetNearestTarget(Tuple<int, int> location, Dictionary<int, Tuple<int, int>> soldierLocationMap, out int closestTarget)
        {
            closestTarget = -1;
            float distance = int.MaxValue;
            foreach (KeyValuePair<int, Tuple<int, int>> kvp in soldierLocationMap)
            {
                
                float tempDistance = CalculateDistance(location, kvp.Value);
                if(tempDistance < distance)
                {
                    distance = tempDistance;
                    closestTarget = kvp.Key;
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
