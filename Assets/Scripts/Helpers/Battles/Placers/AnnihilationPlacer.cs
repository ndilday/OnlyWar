using System;
using System.Collections.Generic;

using UnityEngine;

namespace OnlyWar.Helpers.Battles.Placers
{
    class AnnihilationPlacer
    {
        private readonly BattleGrid _grid;

        public AnnihilationPlacer(BattleGrid grid)
        {
            _grid = grid;
        }
        public Dictionary<BattleSquad, Vector2> PlaceSquads(IEnumerable<BattleSquad> bottomSquads, 
                                                            IEnumerable<BattleSquad> topSquads)
        {
            Dictionary<BattleSquad, Vector2> result = new Dictionary<BattleSquad, Vector2>();

            int currentBottom =  5;
            int currentLeft = _grid.GridWidth / 2;
            int currentTop = currentBottom;
            int currentRight = currentLeft;
            foreach (BattleSquad squad in bottomSquads)
            {
                result[squad] = PlaceBottomSquad(squad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }

            currentBottom = _grid.GridHeight - 5;
            currentLeft = _grid.GridWidth / 2;
            currentTop = currentBottom;
            currentRight = currentLeft;
            foreach (BattleSquad squad in topSquads)
            {
                result[squad] = PlaceTopSquad(squad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
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
                top += squadSize.Item2 + 2;
                left = (_grid.GridWidth / 2) - 2;
                right = left + squadSize.Item1 + 2;

                placeLeft = left;
                placeBottom = bottom;
            }
            else if (spaceRight > left)
            {
                // place to the right of the current box
                placeLeft = right + 2;
                placeBottom = bottom;
                right += squadSize.Item1 + 2;
                if (top < bottom + squadSize.Item2 + 2) top = bottom + squadSize.Item2 + 2;
            }
            else
            {
                // place to the left of the current box
                left -= squadSize.Item1 + 2;
                placeLeft = left;
                placeBottom = bottom;
                if (top < bottom + squadSize.Item2 + 2) top = bottom + squadSize.Item2 + 2;
            }


            _grid.PlaceBattleSquad(squad, new Tuple<int, int>(placeLeft, placeBottom), true);
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

            _grid.PlaceBattleSquad(squad, new Tuple<int, int>(placeLeft, placeBottom), true);
            return new Vector2(placeLeft, placeBottom);
        }
    }
}
