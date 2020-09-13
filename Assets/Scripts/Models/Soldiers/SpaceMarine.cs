using System.Linq;

using Iam.Scripts.Models.Equippables;

namespace Iam.Scripts.Models.Soldiers
{
    public static class TempSpaceMarineRanks
    {
        public static SpaceMarineRank ChapterMaster = new SpaceMarineRank(0, 5, true, "Chapter Master");
        public static SpaceMarineRank ChiefApothecary = new SpaceMarineRank(1, 4, true, "Master of the Apothecarion");
        public static SpaceMarineRank ForgeMaster = new SpaceMarineRank(2, 4, true, "Master of the Forge");
        public static SpaceMarineRank ChiefLibrarian = new SpaceMarineRank(3, 4, true, "Master of the Librarium");
        public static SpaceMarineRank ChiefChaplain = new SpaceMarineRank(4, 4, true, "Master of Sanctity");
        public static SpaceMarineRank Reclusiarch = new SpaceMarineRank(5, 3, true, "Reclusiarch");
        public static SpaceMarineRank ChapterAncient = new SpaceMarineRank(6, 8, false, "Chapter Ancient");
        public static SpaceMarineRank ChapterChampion = new SpaceMarineRank(7, 8, false, "Chapter Champion");
        public static SpaceMarineRank VeteranCaptain = new SpaceMarineRank(8, 4, true, "Veteran Captain");
        public static SpaceMarineRank VeteranSquadSergeant = new SpaceMarineRank(9, 6, false, "Veteran Sergeant");
        public static SpaceMarineRank VeteranMarine = new SpaceMarineRank(10, 4, false, "Veteran");
        public static SpaceMarineRank VeteranAncient = new SpaceMarineRank(11, 7, false, "Veteran Ancient");
        public static SpaceMarineRank VeteranChampion = new SpaceMarineRank(12, 7, false, "Veteran Champion");
        public static SpaceMarineRank Captain = new SpaceMarineRank(13, 3, true, "Captain");
        public static SpaceMarineRank Chaplain = new SpaceMarineRank(14, 4, false, "Chaplain");
        public static SpaceMarineRank Ancient = new SpaceMarineRank(15, 3, false, "Ancient");
        public static SpaceMarineRank Apothecary = new SpaceMarineRank(16, 3, false, "Apothecary");
        public static SpaceMarineRank Champion = new SpaceMarineRank(17, 3, false, "Champion");
        public static SpaceMarineRank TacticalSergeant = new SpaceMarineRank(18, 3, false, "Sergeant");
        public static SpaceMarineRank TacticalMarine = new SpaceMarineRank(19, 2, false, "Tactical Marine");
        public static SpaceMarineRank AssaultSergeant = new SpaceMarineRank(20, 3, false, "Sergeant");
        public static SpaceMarineRank AssaultMarine = new SpaceMarineRank(21, 2, false, "Assault Marine");
        public static SpaceMarineRank DevastatorSergeant = new SpaceMarineRank(22, 3, false, "Sergeant");
        public static SpaceMarineRank DevastatorMarine = new SpaceMarineRank(23, 2, false, "Devastator Marine");
        public static SpaceMarineRank TechMarine = new SpaceMarineRank(24, 3, false, "Techmarine");
        public static SpaceMarineRank RecruitmentCaptain = new SpaceMarineRank(25, 2, false, "Conquisitor");
        public static SpaceMarineRank ScoutSergeant = new SpaceMarineRank(26, 2, false, "Scout Sergeant");
        public static SpaceMarineRank Scout = new SpaceMarineRank(27, 1, false, "Scout");
        public static SpaceMarineRank VeteranTechmarine = new SpaceMarineRank(28, 3, false, "Techmarine Supreme");
        public static SpaceMarineRank Epistolary = new SpaceMarineRank(29, 4, false, "Epistolary");
        public static SpaceMarineRank Codicier = new SpaceMarineRank(30, 3, false, "Codicier");
        public static SpaceMarineRank Lexicanius = new SpaceMarineRank(31, 2, false, "Lexicanius");
        public static SpaceMarineRank Acolyte = new SpaceMarineRank(32, 1, false, "Acolyte");
        public static SpaceMarineRank VeteranApothecary = new SpaceMarineRank(33, 2, false, "Veteran Apothecary");
    }

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

        public Date ProgenoidImplantDate;

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
}
