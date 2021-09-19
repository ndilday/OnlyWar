using OnlyWar.Models.Soldiers;

namespace OnlyWar.Models.Squads
{
    public class SquadTemplateElement
    {
        public SoldierTemplate SoldierTemplate { get; }
        public byte MinimumNumber { get; }
        public byte MaximumNumber { get; }

        public SquadTemplateElement(SoldierTemplate soldierTemplate, byte minNumber, byte maxNumber)
        {
            SoldierTemplate = soldierTemplate;
            MinimumNumber = minNumber;
            MaximumNumber = maxNumber;
        }
    }
}
