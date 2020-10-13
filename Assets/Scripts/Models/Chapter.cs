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
        private readonly Dictionary<Date, List<EventHistory>> _battleHistory;
        public ushort GeneseedStockpile { get; set; }
        public IReadOnlyDictionary<Date, List<EventHistory>> BattleHistory => _battleHistory;
        public Unit OrderOfBattle { get; }
        public List<Fleet> Fleets { get; }
        public Dictionary<int, PlayerSoldier> PlayerSoldierMap { get; }
        public Dictionary<int, Squad> SquadMap { get; private set; }
        public Chapter(Unit unit, IEnumerable<PlayerSoldier> soldiers)
        {
            GeneseedStockpile = 0;
            OrderOfBattle = unit;
            _battleHistory = new Dictionary<Date, List<EventHistory>>();
            PlayerSoldierMap = soldiers.ToDictionary(s => s.Id);
            Fleets = new List<Fleet>();
        }

        public void PopulateSquadMap()
        {
            if (SquadMap == null)
            {
                SquadMap = new Dictionary<int, Squad>
                {
                    [OrderOfBattle.HQSquad.Id] = OrderOfBattle.HQSquad
                };
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

        public void AddToBattleHistory(Date date, string title, List<string> events)
        {
            if (!_battleHistory.ContainsKey(date))
            {
                _battleHistory[date] = new List<EventHistory>();
            }
            EventHistory history = new EventHistory
            {
                EventTitle = title
            };
            history.SubEvents.AddRange(events);
            _battleHistory[date].Add(history);
        }
    }
}
