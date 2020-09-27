using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Fleets;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Models
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
        public Dictionary<int, PlayerSoldier> ChapterPlayerSoldierMap { get; }
        public Chapter(Unit unit, IEnumerable<PlayerSoldier> soldiers)
        {
            GeneseedStockpile = 0;
            OrderOfBattle = unit;
            BattleHistory = new Dictionary<Date, List<EventHistory>>();
            ChapterPlayerSoldierMap = soldiers.ToDictionary(s => s.Id);
            Fleets = new List<Fleet>();
        }
    }
}
