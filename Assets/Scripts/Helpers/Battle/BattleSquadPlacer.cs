using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Iam.Scripts.Helpers.Battle
{
    class BattleSquadPlacer
    {
        BattleGrid _grid;

        public BattleSquadPlacer(BattleGrid grid)
        {
            _grid = grid;
        }
        public Dictionary<BattleSquad, Vector2> PlaceSquads(IEnumerable<BattleSquad> squads)
        {
            Dictionary<BattleSquad, Vector2> result = new Dictionary<BattleSquad, Vector2>();
            if (squads.Any(s => s.IsPlayerSquad))
            {
                int currentBottom =  5;
                int currentLeft = _grid.GridWidth / 2;
                int currentTop = currentBottom;
                int currentRight = currentLeft;
                foreach (BattleSquad squad in squads)
                {
                    result[squad] = PlaceBottomSquad(squad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
                }
            }
            else
            {
                int currentBottom = _grid.GridHeight - 5;
                int currentLeft = _grid.GridWidth / 2;
                int currentTop = currentBottom;
                int currentRight = currentLeft;
                foreach (BattleSquad squad in squads)
                {
                    result[squad] = PlaceTopSquad(squad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
                }
            }
            return result;
        }

        private Vector2 PlaceBottomSquad(BattleSquad squad, ref int left, ref int bottom, ref int right, ref int top)
        {
            Tuple<int, int> squadSize = squad.GetSquadBoxSize();

            // determine if there's more space to the left or right of the current limits
            int spaceRight = _grid.GridWidth - right;
            int placeLeft, placeBottom;
            if (squadSize.Item1 > spaceRight && squadSize.Item1 > left)
            {
                // there's not enough room; move "up"
                bottom = top + 2;
                top += squadSize.Item2 + 4;
                left = (_grid.GridWidth / 2) - 2;
                right = left + squadSize.Item1 + 2;

                placeLeft = left;
                placeBottom = bottom;
            }
            else if (spaceRight > left)
            {
                // place to the right of the current box
                placeLeft = right + 2;
                placeBottom = bottom + 2;
                right += squadSize.Item1 + 5;
                if (top < bottom + squadSize.Item2 + 2) top = bottom + squadSize.Item2 + 2;
            }
            else
            {
                // place to the left of the current box
                left -= squadSize.Item1 + 2;
                placeLeft = left;
                placeBottom = bottom + 2;
                if (top < bottom + squadSize.Item2 + 2) top = bottom + squadSize.Item2 + 2;
            }


            _grid.PlaceSquad(squad, new Tuple<int, int>(placeLeft, placeBottom));
            return new Vector2(placeLeft, placeBottom);
        }

        private Vector2 PlaceTopSquad(BattleSquad squad, ref int left, ref int bottom, ref int right, ref int top)
        {
            Tuple<int, int> squadSize = squad.GetSquadBoxSize();

            // determine if there's more space to the left or right of the current limits
            int spaceRight = _grid.GridWidth - right;
            int placeLeft, placeBottom;
            if (squadSize.Item1 > spaceRight && squadSize.Item1 > left)
            {
                // there's not enough room; move "up"
                top = bottom;
                bottom += squadSize.Item2;
                left = _grid.GridWidth / 2;
                right = left + squadSize.Item1;

                placeLeft = left;
                placeBottom = bottom;
            }
            else if (spaceRight > left)
            {
                // place to the right of the current box
                placeLeft = right;
                placeBottom = top - squadSize.Item2;
                right += squadSize.Item1;
                if (bottom > top - squadSize.Item2) bottom = top - squadSize.Item2;
            }
            else
            {
                // place to the left of the current box
                left -= squadSize.Item1;
                placeLeft = left;
                placeBottom = top - squadSize.Item2;
                if (top < bottom + squadSize.Item2) bottom = top - squadSize.Item2;
            }

            _grid.PlaceSquad(squad, new Tuple<int, int>(placeLeft, placeBottom));
            return new Vector2(placeLeft, placeBottom);
        }
    }
}
