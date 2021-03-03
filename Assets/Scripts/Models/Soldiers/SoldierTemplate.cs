using System;
using System.Collections.Generic;

namespace OnlyWar.Scripts.Models.Soldiers
{
    public class SoldierTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsSquadLeader { get; }
        public byte Rank { get; }
        public Species Species { get; }
        public IReadOnlyCollection<SkillTemplate> SkillTemplates { get; }
        public IReadOnlyCollection<Tuple<BaseSkill, float>> BasicTraining { get; } 

        public SoldierTemplate(int id, Species species, string name, 
                               bool isSquadLeader, byte rank, 
                               IReadOnlyCollection<Tuple<BaseSkill, float>> basicTraining)
        {
            Id = id;
            Species = species;
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
