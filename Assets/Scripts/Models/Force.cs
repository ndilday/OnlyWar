using System.Collections.Generic;
using System.Linq;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;

namespace OnlyWar.Models
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

    public class MilitaryTopLevel
    {
        public string ForceName { get; }
        public Character Leader { get; }
        public string LeaderTitle { get; }
        public MilitaryTopLevel(string forceName, Character leader, string title)
        {
            ForceName = forceName;
            Leader = leader;
            LeaderTitle = title;
        }
    }

    public class Fleet : MilitaryTopLevel
    {
        public List<TaskForce> TaskForces { get; }

        public Fleet(string fleetName, Character leader, string title)
            : base(fleetName, leader, title)
        {
            TaskForces = new List<TaskForce>();
        }
    }

    public class Army : MilitaryTopLevel
    {
        public Unit OrderOfBattle { get; }
        public Dictionary<int, PlayerSoldier> PlayerSoldierMap { get; }
        public Dictionary<int, Squad> SquadMap { get; private set; }

        public Army(string armyName, Character leader, string title, Unit unit, IEnumerable<PlayerSoldier> soldiers)
            : base(armyName, leader, title)
        {
            PlayerSoldierMap = soldiers.ToDictionary(s => s.Id);
            OrderOfBattle = unit;
        }

        public void PopulateSquadMap()
        {
            if (SquadMap == null)
            {
                SquadMap = new Dictionary<int, Squad>();
                foreach (Squad squad in OrderOfBattle.Squads)
                {
                    SquadMap[squad.Id] = squad;
                }
                foreach (Unit company in OrderOfBattle.ChildUnits)
                {
                    foreach (Squad squad in company.Squads)
                    {
                        SquadMap[squad.Id] = squad;
                    }
                }
            }
        }
    }

    public class SectorForce
    {
        private readonly Dictionary<Date, List<EventHistory>> _battleHistory;
        public IReadOnlyDictionary<Date, List<EventHistory>> BattleHistory => _battleHistory;
        public Faction Faction { get; }
        public Army Army { get; }
        public Character Leader { get; }
        public Fleet Fleet { get; }
        public List<IRequest> Requests { get; }
        public SectorForce(Faction faction, Character leader, Army army, Fleet fleet)
        {
            Faction = faction;
            Leader = leader;
            Army = army;
            Fleet = fleet;
            _battleHistory = new Dictionary<Date, List<EventHistory>>();
            Requests = new List<IRequest>();
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

    public class PlayerForce : SectorForce
    {
        public ushort GeneseedStockpile { get; set; }

        public PlayerForce(Faction faction, Army army, Fleet fleet) 
            : base(faction, null, army, fleet)
        {
            GeneseedStockpile = 0;
        }
    }
}
