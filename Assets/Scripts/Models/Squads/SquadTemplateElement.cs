using System.Collections.Generic;

using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Squads
{
    public class SquadTemplateElement
    {
        public IReadOnlyCollection<SoldierType> AllowedSoldierTypes { get; }
        public byte MinimumNumber { get; }
        public byte MaximumNumber { get; }

        public SquadTemplateElement(List<SoldierType> soldierTypes, byte minNumber, byte maxNumber)
        {
            AllowedSoldierTypes = soldierTypes.AsReadOnly();
            MinimumNumber = minNumber;
            MaximumNumber = maxNumber;
        }
    }
}
