using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlyWar.Helpers.Battles.Placers
{
    public class ArmyLayout
    {
        public Dictionary<int, Tuple<int, int>> SquadPositionMap { get; set; }
        public Tuple<int, int> ArmyDimension { get; set; }
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
            int x = 0;
            int y = 0;
            int i = 0;
            int row = 0;
            int maxX = 0;
            int maxY = 0;

            ArmyLayout layout = new ArmyLayout();

            List<BattleSquad> fastSquads = new List<BattleSquad>();
            List<BattleSquad> heavySquads = new List<BattleSquad>();
            List<BattleSquad> hqSquads = new List<BattleSquad>();
            List<BattleSquad> defaultSquads = new List<BattleSquad>();

            foreach(BattleSquad squad in squads)
            {
                if((squad.Squad.SquadTemplate.SquadType & Models.Squads.SquadTypes.HQ)
                            == Models.Squads.SquadTypes.HQ)
                {
                    hqSquads.Add(squad);
                }
                // later, scouts will get to deploy closer to the enemy
                else if((squad.Squad.SquadTemplate.SquadType & Models.Squads.SquadTypes.Fast) 
                            == Models.Squads.SquadTypes.Fast||
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
                while(fastSquads.Count + defaultSquads.Count > (heavySquads.Count + hqSquads.Count) * 4 &&
                    defaultSquads.Count > 0)
                {
                    // move the slowest default squad to the heavy list,
                    // effectively putting it in the reserve line
                    heavySquads.Add(defaultSquads[defaultSquads.Count - 1]);
                    defaultSquads.RemoveAt(defaultSquads.Count - 1);
                }

                // place remaining default squads in a line, a few yards apart from each other

                // place fast squads at each end of the default squads
                // place heavy squads in a line, a few yards apart from each other, behind the first line
                // place HQ 
            }

            layout.ArmyDimension = new Tuple<int, int>(maxX, maxY);
            return layout;
        }
    }
}
