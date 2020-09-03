using System.Collections.Generic;

namespace Iam.Scripts.Models.Units
{
    public sealed class TempSpaceMarineUnitTemplates
    {
        private static TempSpaceMarineUnitTemplates _instance = null;
        public UnitTemplate VeteranCompanyTemplate { get; private set; }
        public UnitTemplate BattleCompanyTemplate { get; private set; }
        public UnitTemplate TacticalCompanyTemplate { get; private set; }
        public UnitTemplate AssaultCompanyTemplate { get; private set; }
        public UnitTemplate DevestatorCompanyTemplate { get; private set; }
        public UnitTemplate ScoutCompanyTemplate { get; private set; }
        public UnitTemplate ChapterTemplate { get; private set; }

        private TempSpaceMarineUnitTemplates()
        {
            VeteranCompanyTemplate = CreateVeteranCompany();
            BattleCompanyTemplate = CreateBattleCompany();
            TacticalCompanyTemplate = CreateTacticalCompany();
            AssaultCompanyTemplate = CreateAssaultCompany();
            DevestatorCompanyTemplate = CreateDevestatorCompany();
            ScoutCompanyTemplate = CreateScoutCompany();
            ChapterTemplate = CreateChapter();
        }

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

        private UnitTemplate CreateVeteranCompany()
        {
            TempSpaceMarineSquadTemplates squads = TempSpaceMarineSquadTemplates.Instance;
            UnitTemplate vetCompany = new UnitTemplate(1, "Veteran Company");
            vetCompany.AddHQSquad(squads.CompanyHQSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            vetCompany.AddSquad(squads.VeteranSquadTemplate);
            return vetCompany;
        }

        private UnitTemplate CreateBattleCompany()
        {
            TempSpaceMarineSquadTemplates squads = TempSpaceMarineSquadTemplates.Instance;
            UnitTemplate battleCompany = new UnitTemplate(2, "Battle Company");
            battleCompany.AddHQSquad(squads.CompanyHQSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.TacticalSquadTemplate);
            battleCompany.AddSquad(squads.AssaultSquadTemplate);
            battleCompany.AddSquad(squads.DevastatorSquadTemplate);
            return battleCompany;
        }

        private UnitTemplate CreateTacticalCompany()
        {
            TempSpaceMarineSquadTemplates squads = TempSpaceMarineSquadTemplates.Instance;
            UnitTemplate company = new UnitTemplate(3, "Tactical Company");
            company.AddHQSquad(squads.CompanyHQSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            company.AddSquad(squads.TacticalSquadTemplate);
            return company;
        }

        private UnitTemplate CreateAssaultCompany()
        {
            TempSpaceMarineSquadTemplates squads = TempSpaceMarineSquadTemplates.Instance;
            UnitTemplate company = new UnitTemplate(4, "Tactical Company");
            company.AddHQSquad(squads.CompanyHQSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            company.AddSquad(squads.AssaultSquadTemplate);
            return company;
        }

        private UnitTemplate CreateDevestatorCompany()
        {
            TempSpaceMarineSquadTemplates squads = TempSpaceMarineSquadTemplates.Instance;
            UnitTemplate company = new UnitTemplate(5, "Devestator Company");
            company.AddHQSquad(squads.CompanyHQSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            company.AddSquad(squads.DevastatorSquadTemplate);
            return company;
        }

        private UnitTemplate CreateScoutCompany()
        {
            TempSpaceMarineSquadTemplates squads = TempSpaceMarineSquadTemplates.Instance;
            UnitTemplate company = new UnitTemplate(6, "Scout Company");
            company.AddHQSquad(squads.CompanyHQSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            company.AddSquad(squads.ScoutSquadTemplate);
            return company;
        }

        private UnitTemplate CreateChapter()
        {
            UnitTemplate chapter = new UnitTemplate(16, "Chapter");
            chapter.AddHQSquad(TempSpaceMarineSquadTemplates.Instance.CompanyHQSquadTemplate);
            chapter.AddSquad(TempSpaceMarineSquadTemplates.Instance.Armory);
            chapter.AddSquad(TempSpaceMarineSquadTemplates.Instance.Librarius);
            chapter.AddSquad(TempSpaceMarineSquadTemplates.Instance.Apothecarion);
            chapter.AddSquad(TempSpaceMarineSquadTemplates.Instance.Reclusium);
            chapter.AddChildUnit(VeteranCompanyTemplate);
            chapter.AddChildUnit(BattleCompanyTemplate);
            chapter.AddChildUnit(BattleCompanyTemplate);
            chapter.AddChildUnit(BattleCompanyTemplate);
            chapter.AddChildUnit(BattleCompanyTemplate);
            chapter.AddChildUnit(TacticalCompanyTemplate);
            chapter.AddChildUnit(TacticalCompanyTemplate);
            chapter.AddChildUnit(AssaultCompanyTemplate);
            chapter.AddChildUnit(DevestatorCompanyTemplate);
            chapter.AddChildUnit(ScoutCompanyTemplate);
            return chapter;
        }
    }

}
