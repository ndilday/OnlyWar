namespace OnlyWar.Models.Fleets
{
    public class BoatTemplate
    {
        public int Id { get; }
        public string ClassName { get; }
        public ushort SoldierCapacity { get; }

        public BoatTemplate(int id, string className, ushort soldierCapacity)
        {
            Id = id;
            ClassName = className;
            SoldierCapacity = soldierCapacity;
        }
    }

    public class ShipTemplate : BoatTemplate
    {
        public ushort BoatCapacity { get; }
        public ushort LanderCapacity { get; }

        public ShipTemplate(int id, string className, ushort soldierCapacity, 
                            ushort boatCapacity, ushort landerCapacity)
            : base(id, className, soldierCapacity)
        {
            BoatCapacity = boatCapacity;
            LanderCapacity = landerCapacity;
        }
    }
}
