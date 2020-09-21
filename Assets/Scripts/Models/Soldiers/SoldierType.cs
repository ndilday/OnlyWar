namespace Iam.Scripts.Models.Soldiers
{
    public class SoldierType
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsSquadLeader { get; }
        public byte Rank { get; }
        public SoldierType(int id, string name, bool isSquadLeader, byte rank)
        {
            Id = id;
            Name = name;
            IsSquadLeader = isSquadLeader;
            Rank = rank;
        }
    }
}
