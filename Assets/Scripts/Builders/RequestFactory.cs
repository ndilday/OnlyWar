using OnlyWar.Models;
using OnlyWar.Models.Planets;

namespace OnlyWar.Builders
{
    class RequestFactory
    {
        private RequestFactory() { }
        private static RequestFactory _instance;
        private static int _nextId = 0;
        public static RequestFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RequestFactory();
                }
                return _instance;
            }
        }

        public void SetCurrentHighestRequestId(int highestId)
        {
            _nextId = highestId + 1;
        }

        public IRequest GenerateNewRequest(Planet planet, Character requester, GameSettings gameSettings, 
                                           Date dateRequestMade, Date fulfilledDate = null)
        {
            return new PresenceRequest(_nextId++, planet, requester, gameSettings, 
                                       dateRequestMade, fulfilledDate);
        }
    }
}
