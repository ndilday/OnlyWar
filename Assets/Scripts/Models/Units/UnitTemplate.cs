using System.Collections.Generic;
using System.Linq;
using OnlyWar.Models.Squads;

namespace OnlyWar.Models.Units
{
    public class UnitTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public bool IsTopLevelUnit { get; }
        public Faction Faction { get; set; }

        public SquadTemplate HQSquad { get; private set; }

        //private UnitTemplate _parentUnit;
        private IReadOnlyCollection<UnitTemplate> _childUnits;
        private readonly IReadOnlyCollection<SquadTemplate> _childSquads;

        public UnitTemplate(int id, string name, bool isTopLevel,
                            List<SquadTemplate> childSquads,
                            List<UnitTemplate> childUnits)
        {
            Id = id;
            Name = name;
            IsTopLevelUnit = isTopLevel;
            _childUnits = childUnits;
            SquadTemplate hq = childSquads.FirstOrDefault(squad => (squad.SquadType & SquadTypes.HQ) > 0);
            if (hq != null)
            {
                HQSquad = hq;
                childSquads.Remove(hq);
            }
            _childSquads = childSquads;
        }

        public UnitTemplate(int id, string name, bool isTopLevel,
                            SquadTemplate hqSquadTemplate,
                            List<SquadTemplate> childSquads)
        {
            Id = id;
            Name = name;
            IsTopLevelUnit = isTopLevel;
            _childSquads = childSquads;
            HQSquad = hqSquadTemplate;
        }

        public void SetChildUnits(IReadOnlyCollection<UnitTemplate> childUnits)
        {
            _childUnits = childUnits;
        }

        public IReadOnlyCollection<UnitTemplate> GetChildUnits()
        {
            return _childUnits ?? (IReadOnlyCollection<UnitTemplate>)Enumerable.Empty<UnitTemplate>();
        }

        public IReadOnlyCollection<SquadTemplate> GetChildSquads()
        {
            return _childSquads ?? (IReadOnlyCollection<SquadTemplate>)Enumerable.Empty<SquadTemplate>();
        }

        public Unit GenerateUnitFromTemplateWithoutChildren(string name)
        {
            return new Unit(name, this);
        }

        public int GetMaximumSoldierCount()
        {
            int maxSoldierCount = 0;
            foreach(UnitTemplate childUnit in GetChildUnits())
            {
                maxSoldierCount += childUnit.GetMaximumSoldierCount();
            }
            foreach(SquadTemplate childSquad in GetChildSquads())
            {
                maxSoldierCount += childSquad.Elements.Sum(e => e.MaximumNumber);
            }
            return maxSoldierCount;
        }
    }

}
