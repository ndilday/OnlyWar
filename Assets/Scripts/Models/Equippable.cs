
namespace Iam.Scripts.Models
{
    public enum EquipLocation
    {
        Body,
        OneHand,
        TwoHand
    }
    public class Equippable
    {
        public int Id;
        public string Name;
        public EquipLocation Location;
        
        // need some stats
    }
}