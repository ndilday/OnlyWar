using System.Collections.Generic;
using System.Linq;

using Iam.Scripts.Models.Squads;

namespace Iam.Scripts.Models.Units
{
    public sealed class TempSpaceMarineUnitTemplates
    {
        private static TempSpaceMarineUnitTemplates _instance = null;

        public static TempSpaceMarineUnitTemplates Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempSpaceMarineUnitTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, UnitTemplate> UnitTemplates { get; }

        private TempSpaceMarineUnitTemplates()
        {
            UnitTemplates = new List<UnitTemplate>
            {
                CreateChapter(CreateVeteranCompany(),
                CreateBattleCompany(),
                CreateTacticalCompany(),
                CreateAssaultCompany(),
                CreateDevastatorCompany(),
                CreateScoutCompany())
            }.ToDictionary(ut => ut.Id);
        }

        private UnitTemplate CreateVeteranCompany()
        {
            return new UnitTemplate(1, "Veteran Company", null, new List<SquadTemplate>
            {
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[14],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[16]
            });
        }

        private UnitTemplate CreateBattleCompany()
        {
            return new UnitTemplate(2, "Battle Company", null, new List<SquadTemplate>
            {
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[12],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21]
            });
        }

        private UnitTemplate CreateTacticalCompany()
        {
            return new UnitTemplate(6, "Tactical Company", null, new List<SquadTemplate>
            {
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[12],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[19]
            });
        }

        private UnitTemplate CreateAssaultCompany()
        {
            return new UnitTemplate(8, "Assault Company", null, new List<SquadTemplate>
            {
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[12],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[20]
            });
        }

        private UnitTemplate CreateDevastatorCompany()
        {
            return new UnitTemplate(9, "Devastator Company", null, new List<SquadTemplate>
            {
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[12],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[21]
            });
        }

        private UnitTemplate CreateScoutCompany()
        {
            return new UnitTemplate(8, "Assault Company", null, new List<SquadTemplate>
            {
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[12],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22],
                TempSpaceMarineSquadTemplates.Instance.SquadTemplates[22]
            });
        }

        private UnitTemplate CreateChapter(UnitTemplate veteranCompany, UnitTemplate battleCompany,
                                           UnitTemplate tactialCompany, UnitTemplate assaultCompany, 
                                           UnitTemplate devastatorCompany, UnitTemplate scoutCompany)
        {
            return new UnitTemplate(0, "Space Marine Chapter",
                                    new List<UnitTemplate>
                                    {
                                        veteranCompany,
                                        battleCompany,
                                        battleCompany,
                                        battleCompany,
                                        battleCompany,
                                        tactialCompany,
                                        tactialCompany,
                                        assaultCompany,
                                        devastatorCompany,
                                        scoutCompany
                                    },
                                    new List<SquadTemplate>
                                    {
                                        TempSpaceMarineSquadTemplates.Instance.SquadTemplates[12],
                                        TempSpaceMarineSquadTemplates.Instance.SquadTemplates[2],
                                        TempSpaceMarineSquadTemplates.Instance.SquadTemplates[3],
                                        TempSpaceMarineSquadTemplates.Instance.SquadTemplates[6],
                                        TempSpaceMarineSquadTemplates.Instance.SquadTemplates[10],
                                    });
        }
    }

}
