using System.Linq;



using Iam.Scripts.Models.Equippables;

namespace Iam.Scripts.Models.Soldiers
{
    public class Tyranid : Soldier
    {
        public override string JobRole
        {
            get
            {
                return "Tyranid";
            }
        }

        public override string ToString()
        {
            return JobRole + Id.ToString();
        }

        public override int FunctioningHands
        {
            get
            {
                int handsAvailable = 0;
                HitLocation leftHand = Body.HitLocations.Single(hl => hl.Template.Name == "Left Hand");
                HitLocation leftArm = Body.HitLocations.Single(hl => hl.Template.Name == "Left Arm");
                HitLocation rightHand = Body.HitLocations.Single(hl => hl.Template.Name == "Right Hand");
                HitLocation rightArm = Body.HitLocations.Single(hl => hl.Template.Name == "Right Arm");
                if (leftHand.Wounds < leftHand.Template.WoundLimit && leftArm.Wounds < leftArm.Template.WoundLimit)
                {
                    handsAvailable++;
                }
                if (rightHand.Wounds < rightHand.Template.WoundLimit && rightArm.Wounds < rightArm.Template.WoundLimit)
                {
                    handsAvailable++;
                }
                return handsAvailable;
            }
        }
    }

    public class Hormagaunt : Soldier
    {
        public override string JobRole
        {
            get
            {
                return "Hormagaunt";
            }
        }

        public override string ToString()
        {
            return JobRole + Id.ToString();
        }

        public override int FunctioningHands
        {
            get
            {
                return 0;
            }
        }
    }
}
