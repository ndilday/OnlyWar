namespace Iam.Scripts.Models.Fleets
{
    public class ShipTemplate
    {
        public int Id { get; }
        public string ClassName { get; }
        public ushort SoldierCapacity { get; }

        public ShipTemplate(int id, string className, ushort soldierCapacity)
        {
            Id = id;
            ClassName = className;
            SoldierCapacity = soldierCapacity;
        }
    }
}
