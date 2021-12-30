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
        public int Id { get; private set; }
        public Planet TargetPlanet { get; private set; }

        public Character Requester { get; private set; }

        public Date DateRequestMade { get; private set; }

        public Date DateRequestFulfilled { get; private set; }

        public PresenceRequest(int id, Planet planet, Character requester, 
                               Date dateRequestMade,  Date fulfilledDate = null)
        {
            Id = id;
            TargetPlanet = planet;
            Requester = requester;
            DateRequestMade = dateRequestMade;
            DateRequestFulfilled = fulfilledDate;
        }
        public bool IsRequestStarted()
        {
            return false;
        }
        public bool IsRequestCompleted()
        {
            return false;
        }
    }
}
