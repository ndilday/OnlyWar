using System.Collections.Generic;
using System.Linq;
using OnlyWar.Scripts.Models.Fleets;
using OnlyWar.Scripts.Models.Soldiers;
using OnlyWar.Scripts.Models.Squads;
using OnlyWar.Scripts.Models.Units;

namespace OnlyWar.Scripts.Models
{
    public class EventHistory
    {
        public string EventTitle { get; set; }
        public List<string> SubEvents { get; private set; }
        public EventHistory()
        {
            SubEvents = new List<string>();
        }
    }

    public class Chapter
    {
        public ushort GeneseedStockpile { get; }
        public Dictionary<Date, List<EventHistory>> BattleHistory { get; }
        public Unit OrderOfBattle { get; }
        public List<Fleet> Fleets { get; }
        public Dictionary<int, PlayerSoldier> PlayerSoldierMap { get; }
        public Dictionary<int, Squad> SquadMap { get; }
        public Chapter(Unit unit, IEnumerable<PlayerSoldier> soldiers)
        {
            GeneseedStockpile = 0;
            OrderOfBattle = unit;
            BattleHistory = new Dictionary<Date, List<EventHistory>>();
            PlayerSoldierMap = soldiers.ToDictionary(s => s.Id);
            Fleets = new List<Fleet>();
            PopulateSquadMap();
        }

        private void PopulateSquadMap()
        {
            SquadMap[OrderOfBattle.HQSquad.Id] = OrderOfBattle.HQSquad;
            foreach (Squad squad in OrderOfBattle.Squads)
            {
                SquadMap[squad.Id] = squad;
            }
            foreach (Unit company in OrderOfBattle.ChildUnits)
            {
                if (company.HQSquad != null)
                {
                    SquadMap[company.HQSquad.Id] = company.HQSquad;
                }
                foreach (Squad squad in company.Squads)
                {
                    SquadMap[squad.Id] = squad;
                }
            }
        }
    }
}
