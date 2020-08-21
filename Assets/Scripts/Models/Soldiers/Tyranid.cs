using System.Linq;

namespace Iam.Scripts.Models.Soldiers
{
    public class Tyranid : Soldier
    {
        public override string JobRole
        {
            get
            {
                return "Tyranid Warrior";
            }
        }

        public override string ToString()
        {
            return JobRole + Id.ToString();
        }

        public override bool CanFireWeapon(Weapon weapon)
        {
            HitLocation leftHand = Body.HitLocations.Single(hl => hl.Template.Name == "Left Hand");
            HitLocation leftArm = Body.HitLocations.Single(hl => hl.Template.Name == "Left Arm");
            HitLocation rightHand = Body.HitLocations.Single(hl => hl.Template.Name == "Right Hand");
            HitLocation rightArm = Body.HitLocations.Single(hl => hl.Template.Name == "Right Arm");
            bool leftUsable = leftHand.Wounds < leftHand.Template.WoundLimit && leftArm.Wounds < leftArm.Template.WoundLimit;
            bool rightUsable = rightHand.Wounds < rightHand.Template.WoundLimit && rightArm.Wounds < rightArm.Template.WoundLimit;
            return (weapon.Template.Location == EquipLocation.TwoHand && leftUsable && rightUsable)
                || (weapon.Template.Location == EquipLocation.OneHand && (leftUsable || rightUsable));
        }
    }
}
