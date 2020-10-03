using System.Collections.Generic;
using System.Linq;
using Iam.Scripts.Models.Squads;

namespace Iam.Scripts.Models.Units
{
    public class UnitTemplate
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public SquadTemplate HQSquad { get; private set; }

        //private UnitTemplate _parentUnit;
        private readonly IReadOnlyCollection<UnitTemplate> _childUnits;
        private readonly IReadOnlyCollection<SquadTemplate> _childSquads;

        public UnitTemplate(int id, string name, 
                            List<UnitTemplate> childUnits,
                            List<SquadTemplate> childSquads)
        {
            Id = id;
            Name = name;
            _childUnits = childUnits;
            SquadTemplate hq = childSquads.FirstOrDefault(squad => (squad.SquadType & SquadTypes.HQ) > 0);
            if (hq != null)
            {
                HQSquad = hq;
                childSquads.Remove(hq);
            }
            _childSquads = childSquads;
        }

        public IReadOnlyCollection<UnitTemplate> GetChildUnits()
        {
            return _childUnits ?? (IReadOnlyCollection<UnitTemplate>)Enumerable.Empty<UnitTemplate>();
        }

        public IReadOnlyCollection<SquadTemplate> GetChildSquads()
        {
            return _childSquads ?? (IReadOnlyCollection<SquadTemplate>)Enumerable.Empty<SquadTemplate>();
        }

        public Unit GenerateUnitFromTemplateWithoutChildren(int id, string name)
        {
            return new Unit(id, name, this);
        }
    }

}
