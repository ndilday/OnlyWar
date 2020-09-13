
namespace Iam.Scripts.Models.Units
{
    public class Chapter
    {
        public ushort GeneseedStockpile { get; private set; }
        public Unit OrderOfBattle { get; private set; }
        public Chapter(Unit unit)
        {
            GeneseedStockpile = 0;
            OrderOfBattle = unit;
        }
    }
}
