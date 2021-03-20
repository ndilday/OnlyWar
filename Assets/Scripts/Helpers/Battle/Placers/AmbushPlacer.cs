using System;
using System.Collections.Generic;

using UnityEngine;

namespace OnlyWar.Scripts.Helpers.Battle.Placers
{
    public class AmbushPlacer
    {
        private readonly BattleGrid _grid;

        public AmbushPlacer(BattleGrid grid)
        {
            _grid = grid;
        }

        public Dictionary<BattleSquad, Vector2> PlaceSquads(IReadOnlyList<BattleSquad> ambushedSquads,
                                                            IReadOnlyList<BattleSquad> ambushingSquads)
        {
            Dictionary<BattleSquad, Vector2> result = new Dictionary<BattleSquad, Vector2>();
            int xMid = _grid.GridWidth / 2;
            int yMid = _grid.GridHeight / 2;
            int startingTop = yMid + ((ambushedSquads.Count * 4) / 2);
            var killZone = PlaceAmbushedSquads(ambushedSquads, xMid, startingTop, result);
            PlaceAmbushingSquads(ambushingSquads, killZone, result);
            return result;
        }

        private Tuple<Vector2, Vector2> PlaceAmbushedSquads(IEnumerable<BattleSquad> squads, int xMid, int top,
                                                              Dictionary<BattleSquad, Vector2> squadPositionMap)
        {
            // assume a depth of four yards per squad
            // center them all on the midpoint, one behind the other
            int topLimit = top;
            int bottomLimit = top;
            int leftLimit = xMid;
            int rightLimit = xMid;
            foreach (BattleSquad squad in squads)
            {
                Tuple<int, int> squadSize = squad.GetSquadBoxSize();
                int left = xMid - squadSize.Item1 / 2;
                int right = xMid + squadSize.Item1 - squadSize.Item1 / 2;
                bottomLimit = top - squadSize.Item2;
                squadPositionMap[squad] = new Vector2(left, bottomLimit);
                _grid.PlaceBattleSquad(squad, new Tuple<int, int>(left, bottomLimit), true);

                top -= squadSize.Item2 + 1;

                if (left < leftLimit)
                {
                    leftLimit = left;
                }
                if (right > rightLimit)
                {
                    rightLimit = right;
                }
            }

            return new Tuple<Vector2, Vector2>(new Vector2(leftLimit, topLimit), new Vector2(rightLimit, bottomLimit));
        }
        private void PlaceAmbushingSquads(IReadOnlyList<BattleSquad> ambushingSquads, 
                                             Tuple<Vector2, Vector2> killZone, 
                                             Dictionary<BattleSquad, Vector2> squadPositionMap)
        {
            // ambushing forces start 40 yards from the target to the top and left
            int currentY = (int)killZone.Item1.y;
            int currentX = (int)killZone.Item1.x - 40;
            int bottomLimit = (int)killZone.Item2.y - 50;
            int rightLimit = (int)killZone.Item2.x;
            bool onLeft = true;
            int iteration = 0;
            foreach (BattleSquad squad in ambushingSquads)
            {
                Tuple<int, int> squadSize = squad.GetSquadBoxSize();
                if (onLeft)
                {
                    // start at top left of killzone, fill downward
                    currentY -= squadSize.Item1;
                    int left = currentX - squadSize.Item2;
                    _grid.PlaceBattleSquad(squad, new Tuple<int, int>(left, currentY), false);
                    if(currentY <= bottomLimit)
                    {
                        onLeft = false;
                        currentY = (int)killZone.Item1.y + 40 + (iteration * 4);
                        currentX = (int)killZone.Item1.x;
                    }
                }
                else
                {
                    // start at top left of killzone, fill right
                    squadPositionMap[squad] = new Vector2(currentX, currentY);
                    _grid.PlaceBattleSquad(squad, new Tuple<int, int>(currentX, currentY), true);
                    currentX += squadSize.Item1;
                    if(currentX >= rightLimit)
                    {
                        onLeft = true;
                        iteration++;
                        currentX = (int)killZone.Item1.x - 40 - (iteration * 4);
                        currentY = (int)killZone.Item1.y;
                    }
                }
            }
        }
    }
}
