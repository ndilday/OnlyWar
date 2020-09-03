using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Helpers
{
    public sealed class TempTyranidArmyGenerator
    {
        public static Unit GenerateTyranidArmy()
        {
            Unit root = TempTyranidUnitTemplates.Instance.TyranidArmy.GenerateUnitFromTemplateWithoutChildren(666, "Tyranid Challenge Force");
            foreach(Squad squad in root.Squads)
            {
                if(squad.SquadTemplate.SquadLeader != null)
                {
                    squad.SquadLeader = SoldierFactory.Instance.GenerateNewSoldier<Tyranid>(squad.SquadTemplate.SquadLeader);
                }
                foreach(SoldierTemplate soldier in squad.SquadTemplate.Members)
                {
                    squad.Members.Add(SoldierFactory.Instance.GenerateNewSoldier<Tyranid>(soldier));
                }
            }
            return root;
        }
    }
}
