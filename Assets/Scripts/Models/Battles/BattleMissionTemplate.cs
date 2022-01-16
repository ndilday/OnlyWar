using OnlyWar.Builders;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using System.Collections.Generic;
using System.Linq;

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
            int opposingForcePoints = (int)(forcePointsPerZone / GetTacticsSkillOfLeader(attackingSquads));
            List<Unit> armyList = new List<Unit>();
            while(opposingForcePoints > 0)
            {
                Unit army = TempArmyBuilder.GenerateArmy(targetFaction.Faction);
                opposingForcePoints -= army.Squads.Sum(s => s.SquadTemplate.BattleValue);
                armyList.Add(army);
            }
            // now that we have the oppFor, figure out a reasonable size of battlefield
            // 10x10 per squad?
            int opSquadsCount = armyList.Sum(al => al.Squads.Count);
            int spaceNeeded = (opSquadsCount + attackingSquads.Count) * 100;
        }

        private float GetTacticsSkillOfLeader(IReadOnlyCollection<Squad> attackingSquads)
        {
            // not just selecting the squad leaders to future-proof for when we're attaching
            // specialists to forces
            var leader = attackingSquads.SelectMany(squad => squad.Members)
                                        .OrderByDescending(soldier => soldier.Template.Rank)
                                        .ThenByDescending(soldier => soldier.Template.Subrank)
                                        .ThenBy(soldier => soldier.Id)
                                        .First();
            var skill = leader.Skills.FirstOrDefault(skill => skill.BaseSkill.Name == "Tactics");
            return leader.Intelligence + (skill == null ? 0 : skill.SkillBonus);
        }
    }
}
