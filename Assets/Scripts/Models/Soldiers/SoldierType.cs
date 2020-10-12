using System;
using System.Collections.Generic;

namespace OnlyWar.Scripts.Models.Soldiers
{
    public class SoldierType
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsSquadLeader { get; }
        public byte Rank { get; }
        public IReadOnlyCollection<Tuple<BaseSkill, float>> BasicTraining { get; } 

        public SoldierType(int id, string name, bool isSquadLeader, byte rank, 
                           IReadOnlyCollection<Tuple<BaseSkill, float>> basicTraining)
        {
            Id = id;
            Name = name;
            IsSquadLeader = isSquadLeader;
            Rank = rank;
            BasicTraining = basicTraining;
            if(BasicTraining == null)
            {
                BasicTraining = new List<Tuple<BaseSkill, float>>();
            }
        }
    }
}
