using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlyWar.Helpers.Battles.Placers
{
    public class ArmyLayout
    {
        public Dictionary<int, Tuple<int, int>> SquadPositionMap { get; set; }
        public Dictionary<int,BattleSquadLayout> SquadLayoutMap { get; set; }
        public Tuple<int, int> ArmyDimension { get; set; }

        public ArmyLayout()
        {
            SquadPositionMap = new Dictionary<int, Tuple<int, int>>();
            SquadLayoutMap = new Dictionary<int, BattleSquadLayout>();
        }
    }

    public class ArmyLayoutHelper
    {
        private static ArmyLayoutHelper _instance;
        private ArmyLayoutHelper() { }
        public static ArmyLayoutHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ArmyLayoutHelper();
                }
                return _instance;
            }
        }

        public ArmyLayout LayoutArmyLine(IEnumerable<BattleSquad> squads, bool isLoose)
        {
            int lineLeft = 0;
            int lineRight = 0;
            int y = 0;
            int maxX = 0;
            int maxY = 0;

            ArmyLayout layout = new();

            List<BattleSquad> fastSquads = new();
            List<BattleSquad> heavySquads = new();
            List<BattleSquad> hqSquads = new();
            List<BattleSquad> defaultSquads = new();

            foreach(BattleSquad squad in squads)
            {
                if ((squad.Squad.SquadTemplate.SquadType & Models.Squads.SquadTypes.HQ)
                            == Models.Squads.SquadTypes.HQ)
                {
                    hqSquads.Add(squad);
                }
                // later, scouts will get to deploy closer to the enemy
                else if ((squad.Squad.SquadTemplate.SquadType & Models.Squads.SquadTypes.Fast)
                            == Models.Squads.SquadTypes.Fast ||
                        (squad.Squad.SquadTemplate.SquadType & Models.Squads.SquadTypes.Scout)
                            == Models.Squads.SquadTypes.Scout)
                {
                    fastSquads.Add(squad);
                }
                else if ((squad.Squad.SquadTemplate.SquadType & Models.Squads.SquadTypes.Heavy)
                            == Models.Squads.SquadTypes.Heavy)
                {
                    heavySquads.Add(squad);
                }
                else
                {
                    defaultSquads.Add(squad);
                }

                defaultSquads = defaultSquads.OrderByDescending(s => s.Soldiers[0].GetMoveSpeed()).ToList();
                while (fastSquads.Count + defaultSquads.Count > (heavySquads.Count + hqSquads.Count) * 4 &&
                    defaultSquads.Count > 0)
                {
                    // move the slowest default squad to the heavy list,
                    // effectively putting it in the reserve line
                    heavySquads.Add(defaultSquads[^1]);
                    defaultSquads.RemoveAt(defaultSquads.Count - 1);
                }

                // place remaining default squads in a line, a few yards apart from each other
                PlaceSquads(defaultSquads, isLoose, layout, y, ref lineLeft, ref lineRight, ref maxY);

                // place fast squads at each end of the default squads
                PlaceSquads(fastSquads, isLoose, layout, y, ref lineLeft, ref lineRight, ref maxY);

                // place heavy squads in a line, a few yards apart from each other, behind the first line
                // place remaining default squads in a line, a few yards apart from each other
                y = maxY + 3;
                PlaceSquads(heavySquads, isLoose, layout, y, ref lineLeft, ref lineRight, ref maxY);

                // place HQ
                // evenly space the HQs across the maximum width of the force
                y = maxY + 3;
                int i = 0;
                int spacing = (lineRight - lineLeft) / (hqSquads.Count + 1);
                if (i < hqSquads.Count)
                {
                    BattleSquad hqSquad = hqSquads[i];
                    BattleSquadLayout squadLayout =
                        BattleSquadLayoutHelper.Instance.LayoutBattleSquad(hqSquad, isLoose);
                    layout.SquadLayoutMap[hqSquad.Id] = squadLayout;
                    
                    layout.SquadPositionMap[squad.Id] = 
                        new Tuple<int, int>(lineLeft + (spacing * (i+1)), y);
                    if (squadLayout.SquadDimension.Item2 + y > maxY)
                    {
                        maxY = squadLayout.SquadDimension.Item2 + y;
                    }
                    i++;
                }

            }

            layout.ArmyDimension = new Tuple<int, int>(maxX, maxY);
            return layout;
        }

        private static void PlaceSquads(List<BattleSquad> squads, bool isLoose, ArmyLayout layout, int y, ref int lineLeft, ref int lineRight, ref int maxY)
        {
            int i = 0;
            while (i < squads.Count)
            {
                BattleSquad squad = squads[i];
                // place a squad to the left
                BattleSquadLayout squadLayout =
                    BattleSquadLayoutHelper.Instance.LayoutBattleSquad(squad, isLoose);
                layout.SquadLayoutMap[squad.Id] = squadLayout;
                lineLeft = lineLeft - 3 - squadLayout.SquadDimension.Item1;
                layout.SquadPositionMap[squad.Id] = new Tuple<int, int>(lineLeft, y);
                if (squadLayout.SquadDimension.Item2 + y > maxY)
                {
                    maxY = squadLayout.SquadDimension.Item2 + y;
                }
                i++;

                // place a squad to the right
                if (i < squads.Count)
                {
                    squad = squads[i];
                    squadLayout =
                        BattleSquadLayoutHelper.Instance.LayoutBattleSquad(squad, isLoose);
                    layout.SquadLayoutMap[squad.Id] = squadLayout;
                    layout.SquadPositionMap[squad.Id] =
                        new Tuple<int, int>(lineRight + 3, y);
                    lineRight += squadLayout.SquadDimension.Item1;
                    if (squadLayout.SquadDimension.Item2 + y > maxY)
                    {
                        maxY = squadLayout.SquadDimension.Item2 + y;
                    }
                    i++;
                }
            }
        }
    }
}
