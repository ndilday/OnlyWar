using OnlyWar.Models;
using System.Collections.Generic;
using System.Data;

namespace OnlyWar.Helpers.Database.GameState
{
    public class PlayerFactionEventDataAccess
    {
        public Dictionary<Date, List<EventHistory>> GetHistory(IDbConnection connection)
        {
            Dictionary<Date, List<EventHistory>> history =
                new Dictionary<Date, List<EventHistory>>();
            var subEvents = GetPlayerFactionSubEvents(connection);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerFactionEvent";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int millenium = reader.GetInt32(1);
                    int year = reader.GetInt32(2);
                    int week = reader.GetInt32(3);
                    string title = reader[4].ToString();
                    Date date = new Date(millenium, year, week);
                    EventHistory historyEntry = new EventHistory
                    {
                        EventTitle = title
                    };
                    if (subEvents.ContainsKey(id))
                    {
                        historyEntry.SubEvents.AddRange(subEvents[id]);
                    }
                    if (!history.ContainsKey(date))
                    {
                        history[date] = new List<EventHistory>();
                    }
                    history[date].Add(historyEntry);
                }
            }
            return history;
        }

        private Dictionary<int, List<string>> GetPlayerFactionSubEvents(IDbConnection connection)
        {
            Dictionary<int, List<string>> subEvents = new Dictionary<int, List<string>>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM PlayerFactionSubEvent";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int playerFactionEventId = reader.GetInt32(0);
                    string entry = reader[1].ToString();
                    if (!subEvents.ContainsKey(playerFactionEventId))
                    {
                        subEvents[playerFactionEventId] = new List<string>();
                    }
                    subEvents[playerFactionEventId].Add(entry);
                }
            }
            return subEvents;
        }

        public void SaveData(IDbTransaction transaction, 
                             IReadOnlyDictionary<Date, List<EventHistory>> history)
        {
            int i = 0;
            foreach(KeyValuePair<Date, List<EventHistory>> kvp in history)
            {
                foreach(EventHistory entry in kvp.Value)
                {
                    string insert = $@"INSERT INTO PlayerFactionEvent VALUES ({i}, 
                        {kvp.Key.Millenium}, {kvp.Key.Year}, {kvp.Key.Week}, 
                        '{entry.EventTitle}');";
                    using (var command = transaction.Connection.CreateCommand())
                    {
                        command.CommandText = insert;
                        command.ExecuteNonQuery();
                    }
                    foreach (string subentry in entry.SubEvents)
                    {
                        insert = $@"INSERT INTO PlayerFactionSubEvent VALUES 
                            ({i}, '{subentry}');";
                        using (var command = transaction.Connection.CreateCommand())
                        {
                            command.CommandText = insert;
                            command.ExecuteNonQuery();
                        }
                    }
                    i++;
                }
            }
        }
    }
}
