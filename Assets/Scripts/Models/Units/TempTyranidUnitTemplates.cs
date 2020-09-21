
using Iam.Scripts.Models.Squads;
using System.Collections.Generic;
using System.Linq;

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
                new UnitTemplate(1000, "Tyranid Melee Army", null, new List<SquadTemplate>
                {
                    TempTyranidSquadTemplates.Instance.SquadTemplates[105],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[105],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[105],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[105],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[104],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[104],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[104],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[104],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[102],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[102],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[102],
                    TempTyranidSquadTemplates.Instance.SquadTemplates[102]
                })
            }.ToDictionary(ut => ut.Id);
        }
    }
}
