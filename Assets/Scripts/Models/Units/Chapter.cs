
using System.Collections.Generic;

namespace Iam.Scripts.Models.Units
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
        public ushort GeneseedStockpile { get; private set; }
        public Dictionary<Date, List<EventHistory>> BattleHistory { get; private set; }
        public Unit OrderOfBattle { get; private set; }
        public Chapter(Unit unit)
        {
            GeneseedStockpile = 0;
            OrderOfBattle = unit;
            BattleHistory = new Dictionary<Date, List<EventHistory>>();
        }
    }
}
