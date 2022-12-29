using OnlyWar.Models.Fleets;
using OnlyWar.Models.Planets;
using OnlyWar.Models.Squads;
using System;
using System.Collections.Generic;


namespace OnlyWar.Models.Missions
{
    class LightningRaidMission : IRegionalMission, ISpaceMission
    {
        private List<Squad> _assignedSquads;
        private int _id;
        private TaskForce _assignedTaskForce;
        private Region _region;

        public List<Squad> AssignedSquads => _assignedSquads;

        public int Id => _id;

        public string Description => "";

        public string Requirements => "";

        public MissionType MissionType => MissionType.LightningRaid;

        public bool IsComplete => false;

        public TaskForce AssignedTaskForce => _assignedTaskForce;

        public Planet Planet => _region.Planet;

        public Region Region => _region;

        public LightningRaidMission(Region region, TaskForce taskForce, List<Squad> squads)
        {
            _assignedSquads = squads;
            _assignedTaskForce = taskForce;
            _region = region;
        }

        public void Process()
        {
            throw new NotImplementedException();
        }
    }
}
