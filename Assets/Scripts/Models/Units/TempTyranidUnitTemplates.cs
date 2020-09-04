
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

        private TempTyranidUnitTemplates() 
        {
            TyranidArmy = new UnitTemplate(1000, "Tyranid Ranged Army");
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TermagauntSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TyranidWarriorSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TermagauntSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TyranidWarriorSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TermagauntSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TyranidWarriorSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TermagauntSquadTemplate);
            TyranidArmy.AddSquad(TempTyranidSquadTemplates.Instance.TyranidWarriorSquadTemplate);
        }

        public UnitTemplate TyranidArmy { get; private set; }
    }
}
