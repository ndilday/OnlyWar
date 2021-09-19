﻿using OnlyWar.Models.Planets;

namespace OnlyWar.Models
{
    public abstract class ARequest
    {
        Planet TargetPlanet;
        Character Requester;
        Date DateRequestMade;
        Date DateRequestFulfilled;
        public abstract bool IsRequestStarted();
        public abstract bool IsRequestCompleted();
    }

    public class PresenceRequest : ARequest
    {
        public override bool IsRequestStarted()
        {
            return false;
        }
        public override bool IsRequestCompleted()
        {
            return false;
        }
    }
}
