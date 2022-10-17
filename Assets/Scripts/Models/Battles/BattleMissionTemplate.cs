using OnlyWar.Builders;
using OnlyWar.Helpers;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Units;
using System.Collections.Generic;
using System;
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
            int opposingForcePoints = (int)(forcePointsPerZone / GetSkillOfLeader(attackingSquads, "Tactics"));
            List<Unit> armyList = new();
            while(opposingForcePoints > 0)
            {
                Unit army = TempArmyBuilder.GenerateArmy(targetFaction.Faction);
                opposingForcePoints -= army.Squads.Sum(s => s.SquadTemplate.BattleValue);
                armyList.Add(army);
            }
            // now that we have the oppFor, figure out the range of engagement
            ushort engagementRange = DetermineEngagementRange(attackingSquads);
            // place the two forces on opposite sides of that range
        }

        private float GetSkillOfLeader(IReadOnlyCollection<Squad> attackingSquads, string skillName)
        {
            // not just selecting the squad leaders to future-proof for when we're attaching
            // specialists to forces
            var leader = attackingSquads.SelectMany(squad => squad.Members)
                                        .OrderByDescending(soldier => soldier.Template.Rank)
                                        .ThenByDescending(soldier => soldier.Template.Subrank)
                                        .ThenBy(soldier => soldier.Id)
                                        .First();
            var skill = leader.Skills.FirstOrDefault(skill => skill.BaseSkill.Name == skillName);
            return leader.Intelligence + (skill == null ? 0 : skill.SkillBonus);
        }

        private ushort DetermineEngagementRange(IReadOnlyCollection<Squad> attackingSquads)
        {
            // TODO: make this more sophisticated
            // for now, assume marines want to engage at 25% to-hit,
            // and take the median squad distance returned
            // compare that to a roll to see where the troops are detected
            ushort forceSize = (ushort)(attackingSquads.Sum(squad => squad.SquadTemplate.BattleValue));
            ushort forceStealth = (ushort)(GetSkillOfLeader(attackingSquads, "Stealth"))
                ;
            ushort detectRange = (ushort)(-Math.Log(RNG.GetLinearDouble()) * forceStealth / (20 * forceSize));
            // use the greater of the two values
            // if the Marine value is used, they have surprise
            return 0;
        }
    }
}
