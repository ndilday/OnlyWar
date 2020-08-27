using System.Linq;

using Iam.Scripts.Models.Equippables;

namespace Iam.Scripts.Models.Soldiers
{
    public class SpaceMarine : Soldier
    {
        public SpaceMarineRank Rank;
        public string FirstName;
        public string LastName;

        // TODO: break out weapon specializations
        public float MeleeScore;
        public float RangedScore;
        public float LeadershipScore;
        public float MedicalScore;
        public float TechScore;
        public float PietyScore;
        public float AncientScore;

        public Skill GetBestMeleeSkill()
        {
            return Skills.Values.Where(s => s.BaseSkill.Category == SkillCategory.Melee).OrderByDescending(s => s.SkillBonus).First();
        }

        public Skill GetBestRangedSkill()
        {
            return Skills.Values.Where(s => s.BaseSkill.Category == SkillCategory.Ranged).OrderByDescending(s => s.SkillBonus).First();
        }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

        public override string JobRole
        {
            get
            {
                return Rank.Name;
            }
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
