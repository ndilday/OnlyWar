using OnlyWar.Models.Planets;

namespace OnlyWar.Models
{
    public interface IRequest
    {
        int Id { get; }
        Planet TargetPlanet { get; }
        Character Requester { get; }
        Date DateRequestMade { get; }
        Date DateRequestFulfilled { get; }
        bool IsRequestStarted();
        bool IsRequestCompleted();
    }

    public class PresenceRequest : IRequest
    {
        private bool _completed;
        public int Id { get; private set; }
        public Planet TargetPlanet { get; private set; }

        public Character Requester { get; private set; }

        public Date DateRequestMade { get; private set; }

        public Date DateRequestFulfilled { get; private set; }
        public int PlayerFactionId{ get; private set; }

        public PresenceRequest(int id, Planet planet, Character requester, int playerFactionId,
                               Date dateRequestMade,  Date fulfilledDate = null)
        {
            Id = id;
            TargetPlanet = planet;
            PlayerFactionId = playerFactionId;
            Requester = requester;
            DateRequestMade = dateRequestMade;
            DateRequestFulfilled = fulfilledDate;
            _completed = false;
        }
        public bool IsRequestStarted()
        {
            return false;
        }
        public bool IsRequestCompleted()
        {
            if (_completed) return true;
            if(TargetPlanet.FactionSquadListMap[PlayerFactionId].Count > 0)
            {
                _completed = true;
                return true;
            }
            return false;
        }
    }
}
