﻿using OnlyWar.Models;
using OnlyWar.Models.Soldiers;
using System.Collections.Generic;

namespace OnlyWar.Helpers
{
    public static class CharacterBuilder
    {
        public static Character GenerateCharacter(int id, Faction faction)
        {
            return new Character()
            {
                Id = id,
                Loyalty = faction,
                Appreciation = (float)RNG.GetLinearDouble(),
                Influence = (float)RNG.GetLinearDouble(),
                Investigation = (float)RNG.GetLinearDouble(),
                Neediness = (float)RNG.GetLinearDouble(),
                Paranoia = (float)RNG.GetLinearDouble(),
                Patience = (float)RNG.GetLinearDouble(),
                OpinionOfPlayerForce = (float)RNG.GetLinearDouble(),
                OpinionOfSoldier = new Dictionary<ISoldier, float>(),
                ActiveRequest = null
            };
        }
    }
}
