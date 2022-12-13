
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlyWar.Helpers.Battles
{
    public class BattleSquadLayout
    {
        public Dictionary<int, Tuple<int, int>> BattleSoldierPositionMap { get; set; }
        public Tuple<int, int> SquadDimension { get; set; }
    }

    public class BattleSquadLayoutHelper
    {
        private static BattleSquadLayoutHelper _instance;
        private BattleSquadLayoutHelper() { }
        public static BattleSquadLayoutHelper Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new BattleSquadLayoutHelper();
                }
                return _instance;
            }
        }

        public BattleSquadLayout LayoutBattleSquad(BattleSquad squad, bool isLoose)
        {
            int x = 0;
            int y = 0;
            int i = 0;
            int row = 0;
            int maxX = 0;
            int maxY = 0;

            BattleSquadLayout layout = new();
            List<BattleSoldier> ableSoldiers = squad.AbleSoldiers;
            int rows = ((ableSoldiers.Count - 1) / 10) + 1;
            int soldiersPerRow = ableSoldiers.Count / rows;
            foreach (BattleSoldier soldier in ableSoldiers.OrderBy(s => RNG.GetLinearDouble()))
            {
                Tuple<int, int> position = new(x, y);
                layout.BattleSoldierPositionMap[soldier.Soldier.Id] = position;

                if(x > maxX)
                {
                    maxX = x;
                }
                if(y > maxY)
                {
                    maxY = y;
                }


                i++;
                if (i == soldiersPerRow)
                {
                    row++;
                    i = 0;
                    y += soldier.Soldier.Template.Species.Depth;
                    if (isLoose)
                    {
                        y++;
                        x = (!isLoose || row % 2 == 0 ? 0 : 1);
                    }
                }
                else
                {
                    x += soldier.Soldier.Template.Species.Width;
                    if (isLoose) x++;
                }
            }

            layout.SquadDimension = new Tuple<int, int>(maxX, maxY);
            return layout;
        }
    }
}
