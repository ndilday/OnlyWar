using OnlyWar.Models.Soldiers;
using System.Collections.Generic;

namespace OnlyWar.Models
{
    public class Character
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        // how likely to discover hidden activity
        public float Investigation { get; set; }

        // how likely to see false hidden activity
        public float Paranoia { get; set; }
        
        // how likely to request aid
        public float Neediness { get; set; }

        // how much ignored requests irk them
        public float Patience { get; set; }

        // how much responding to requests pleases them
        public float Appreciation { get; set; }

        // how much this character's opinion impacts others
        public float Influence { get; set; }

        // TODO: the possibility that the planetary leader is a traitor
        public Faction Loyalty { get; set; }

        // how the character feels about the player's force
        public float OpinionOfPlayerForce { get; set; }

        // how the character feels about individual soliders of the player
        public Dictionary<ISoldier, float> OpinionOfSoldier { get; set; }

        // requests this character has made of the player
        public IRequest ActiveRequest { get; set; }
    }
}
