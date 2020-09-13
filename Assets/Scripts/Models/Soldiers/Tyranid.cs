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
                int functioningHands = 2;
                if (Body.HitLocations.Any(hl => hl.Template.IsMeleeWeaponHolder && hl.IsCrippled))
                {
                    functioningHands--;
                }
                if (Body.HitLocations.Any(hl => hl.Template.IsRangedWeaponHolder && hl.IsCrippled))
                {
                    functioningHands--;
                }
                return functioningHands;
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
