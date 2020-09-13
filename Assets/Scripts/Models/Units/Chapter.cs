
namespace Iam.Scripts.Models.Units
{
    public class Chapter : Unit
    {
        public ushort GeneseedStockpile;
        public Chapter(int id, string name, UnitTemplate template) : base(id, name, template)
        {
            GeneseedStockpile = 0;
        }
    }
}
