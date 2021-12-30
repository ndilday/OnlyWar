using OnlyWar.Models;
using OnlyWar.Models.Planets;
using System;
using System.Collections.Generic;
using System.Data;

namespace OnlyWar.Helpers.Database.GameState
{
    public class RequestDataAccess
    {
        public List<IRequest> GetRequests(IDbConnection connection,
                                          IReadOnlyDictionary<int, Character> characterMap,
                                          List<Planet> planetList,
                                          GameSettings gameSettings)
        {
            List<IRequest> requests = new List<IRequest>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Request";
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int characterId = reader.GetInt32(1);
                    int planetId = reader.GetInt32(2);
                    int requestDate = reader.GetInt32(3);
                    Date fulfillDate;
                    if (reader[4].GetType() != typeof(DBNull))
                    {
                        fulfillDate = new Date(reader.GetInt32(4));
                    }
                    else
                    {
                        fulfillDate = null;
                    }
                    PresenceRequest request =
                        new PresenceRequest(id, planetList[planetId], characterMap[characterId], 
                                            gameSettings, new Date(requestDate), fulfillDate);
                    requests.Add(request);
                    if(request.DateRequestFulfilled == null)
                    {
                        characterMap[characterId].ActiveRequest = request;
                    }
                }
            }
            return requests;
        }

        public void SaveRequest(IDbTransaction transaction, IRequest request)
        {
            object fulfillDate = request.DateRequestFulfilled != null ?
                    (object)request.DateRequestFulfilled.GetTotalWeeks() :
                    "null";
            string insert = $@"INSERT INTO Request 
                (Id, CharacterId, PlanetId, RequestDate, FulfillmentDate) VALUES 
                ({request.Id}, {request.Requester.Id}, {request.TargetPlanet.Id}, 
                {request.DateRequestMade.GetTotalWeeks()}, {fulfillDate});";
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = insert;
                command.ExecuteNonQuery();
            }
        }
    }
}
