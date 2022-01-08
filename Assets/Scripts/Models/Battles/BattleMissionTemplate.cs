using OnlyWar.Models.Soldiers;
namespace OnlyWar.Models.Battles
{
    public enum MissionType
    {
        LightningRaid,
        Infiltrate,
        EstablishAirhead,
        CloseAirSupport,
        HitAndRun,
        Recon,
        Patrol,
        Advance,
        DeepStrike,
        Fortify,
        DefenseInDepth,
        LastStand,
        Assassination,
        ObjectiveRaid,
        Ambush,
        Extermination
    }

    public class BattleMissionTemplate
    {
        int Id;
        MissionType MissionType;
        object SquadEligibilityFilter;
        BattleMissionStep BattleMissionTree;
    }

    public class BattleMissionStep
    {
        int Id;
        IBattleMissionStepChallenge Challenge;
        BattleMissionStep NextStepIfSuccess;
        BattleMissionStep NextStepIfFail;
    }

    public interface IBattleMissionStepChallenge
    {
        bool RunChallenge();
    }

    public class BattleMissionStepSkillChallenge: IBattleMissionStepChallenge
    {
        BaseSkill SkillForChallenge;
        public bool RunChallenge()
        {
            return true;
        }
    }

    public class BattleMissionStepBattleChallenge: IBattleMissionStepChallenge
    {
        public bool RunChallenge()
        {
            return true;
        }
    }
}
