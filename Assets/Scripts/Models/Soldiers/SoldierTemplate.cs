using System;
using System.Collections.Generic;

namespace OnlyWar.Models.Soldiers
{
    public class SoldierTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsSquadLeader { get; }
        public byte Rank { get; }
        public Species Species { get; }
        public IReadOnlyCollection<Tuple<BaseSkill, float>> MosTraining { get; } 

        public SoldierTemplate(int id, Species species, string name, 
                               bool isSquadLeader, byte rank, 
                               IReadOnlyCollection<Tuple<BaseSkill, float>> mosTraining)
        {
            Id = id;
            Species = species;
            Name = name;
            IsSquadLeader = isSquadLeader;
            Rank = rank;
            MosTraining = mosTraining;
        }
    }
}
