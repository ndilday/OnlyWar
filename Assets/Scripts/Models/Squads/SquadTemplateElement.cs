using System.Collections.Generic;

using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Squads
{
    public class SquadTemplateElement
    {
        public SoldierType SoldierType { get; }
        public byte MinimumNumber { get; }
        public byte MaximumNumber { get; }

        public SquadTemplateElement(SoldierType soldierType, byte minNumber, byte maxNumber)
        {
            SoldierType = soldierType;
            MinimumNumber = minNumber;
            MaximumNumber = maxNumber;
        }
    }
}
