using System.Collections.Generic;
using System.Linq;

using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;

namespace Iam.Scripts.Models.Units
{   
    public sealed class TempTyranidUnitTemplates
    {
        private static TempTyranidUnitTemplates _instance;
        public static TempTyranidUnitTemplates Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempTyranidUnitTemplates();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, UnitTemplate> UnitTemplates { get; }

        private TempTyranidUnitTemplates() 
        {
            UnitTemplates = new List<UnitTemplate>
            {
                new UnitTemplate(1000, "Tyrant And Warrior Army", null, new List<SquadTemplate>
                {
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TYRANT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.PRIME],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TERMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TERMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.HORMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.HORMAGAUNT]
                }),
                new UnitTemplate(1001, "Broodlord Army", null, new List<SquadTemplate>
                {
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.BROODLORD],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.GENESTEALER],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.GENESTEALER],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.GENESTEALER],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.GENESTEALER],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.GENESTEALER],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.GENESTEALER]
                }),
                new UnitTemplate(1002, "Prime Army", null, new List<SquadTemplate>
                {
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.PRIME],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.PRIME],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.WARRIOR]
                }),
                new UnitTemplate(1003, "Tyrant Army", null, new List<SquadTemplate>
                {
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TYRANT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TERMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TERMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.TERMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.HORMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.HORMAGAUNT],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[TempSoldierTypes.HORMAGAUNT],
                })
            }.ToDictionary(ut => ut.Id);
        }
    }
}
