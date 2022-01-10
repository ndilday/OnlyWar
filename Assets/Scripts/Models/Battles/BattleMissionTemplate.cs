using OnlyWar.Models.Planets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using System.Collections.Generic;

namespace OnlyWar.Models.Battles
{
    public enum BattleMissionType
    {
        OrbitalRaid,
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
        BattleMissionType BattleMissionType;
        string MissionName;
        //object SquadEligibilityFilter;
        BattleMissionStep BattleMissionTree;

        public BattleMissionTemplate(int id, BattleMissionType type, string name, BattleMissionStep missionRoot)
        {
            Id = id;
            BattleMissionType = type;
            MissionName = name;
            BattleMissionTree = missionRoot;
        }
    }

    public class BattleMissionStep
    {
        int Id;
        string BattleMissionStepName;
        IBattleMissionStepChallenge Challenge;
        BattleMissionStep NextStepIfSuccess;
        BattleMissionStep NextStepIfFail;
        object Effects;

        public BattleMissionStep(int id, string stepName, IBattleMissionStepChallenge challenge,
                                 BattleMissionStep successNextStep, BattleMissionStep failNextStep,
                                 object effects)
        {
            Id = id;
            BattleMissionStepName = stepName;
            Challenge = challenge;
            NextStepIfSuccess = successNextStep;
            NextStepIfFail = failNextStep;
            Effects = effects;
        }
    }

    public interface IBattleMissionStepChallenge
    {
        bool RunChallenge(IReadOnlyCollection<Squad> assignedSquads);
    }

    public class BattleMissionStepSkillChallenge: IBattleMissionStepChallenge
    {
        BaseSkill SkillForChallenge;
        int DifficultyLevel;
        bool IsLeaderOnly;
        public BattleMissionStepSkillChallenge(BaseSkill skill, int difficulty, bool isLeaderOnly)
        {
            SkillForChallenge = skill;
            DifficultyLevel = difficulty;
            IsLeaderOnly = isLeaderOnly;
        }
        public bool RunChallenge(IReadOnlyCollection<Squad> assignedSquads)
        {
            return true;
        }
    }

    public class BattleMissionStepBattleChallenge: IBattleMissionStepChallenge
    {
        public bool RunChallenge(IReadOnlyCollection<Squad> assignedSquads)
        {
            return true;
        }
    }

    public class TacticalBattleMissionStepChallenge: IBattleMissionStepChallenge
    {
        public bool RunChallenge(IReadOnlyCollection<Squad> assignedSquads)
        {
            // leader's int + tactics
            return true;
        }
    }

    public class OrbitalRaidMission
    {
        public void RunMission(PlanetFaction targetFaction, IReadOnlyCollection<Squad> attackingSquads)
        {
            int zonesHeld = targetFaction.PlanetaryControl;
            // since population is in thousands, and we want 1% of the population in points of soldiers
            // multiply stored population by 10 and divide by number of zones to see points per zone
            long forcePointsPerZone = targetFaction.Population * 10 / zonesHeld;
            // divide the force per zone by the INT + Tactics of the Mission Leader
            // to determine the size of force the landing troops will face

        }

        private float GetTacticsSkillOfLeader(IReadOnlyCollection<Squad> attackingSquads)
        {
            int 
        }
    }
}
