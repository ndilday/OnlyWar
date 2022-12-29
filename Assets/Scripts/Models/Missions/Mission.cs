using System.Collections.Generic;
using OnlyWar.Models.Fleets;
using OnlyWar.Models.Squads;
using OnlyWar.Models.Planets;

namespace OnlyWar.Models.Missions
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

    public interface IMission
    {
        public int Id { get; }
        public string Description { get; }
        public string Requirements { get; }
        public MissionType MissionType { get; }

        public bool IsComplete { get; }
        public void Process();
    }

    public interface ISpaceMission : IMission
    {
        public TaskForce AssignedTaskForce { get; }
    }

    public interface IGroundMission: IMission
    {
        public List<Squad> AssignedSquads { get; }
    }

    public interface IPlanetaryMission : IMission
    {
        public Planet Planet { get; }
    }

    public interface IRegionalMission: IGroundMission, IPlanetaryMission
    {
        public Region Region { get; }
    }
}
