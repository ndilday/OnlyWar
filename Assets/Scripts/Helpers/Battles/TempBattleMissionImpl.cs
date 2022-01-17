using OnlyWar.Models.Battles;

namespace OnlyWar.Helpers.Battles
{
    public sealed class TempBattleMissionImpl
    {
        private TempBattleMissionImpl() 
        {
            _orbitalRaid = BuildOrbitalRaid();
        }
        private static TempBattleMissionImpl _instance;
        private static BattleMissionTemplate _orbitalRaid;
        public static TempBattleMissionImpl Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempBattleMissionImpl();
                }
                return _instance;
            }
        }
        public static BattleMissionTemplate OrbitalRaid { get => _orbitalRaid; }

        public BattleMissionTemplate BuildOrbitalRaid()
        {
            BattleMissionStep rootNode = null;
            //BattleMissionStepSkillChallenge
            return new BattleMissionTemplate(1, BattleMissionType.OrbitalRaid, "Orbital Raid", rootNode);
        }
    }
}
