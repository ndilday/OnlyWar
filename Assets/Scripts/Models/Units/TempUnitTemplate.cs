using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public static class TempSpaceMarineRanks
    {
        public static SpaceMarineRank ChapterMaster = new SpaceMarineRank(0, 5, true, "Chapter Master");
        public static SpaceMarineRank ChiefApothecary = new SpaceMarineRank(1, 4, true, "Master of the Apothecarion");
        public static SpaceMarineRank ForgeMaster = new SpaceMarineRank(2, 4, true, "Master of the Forge");
        public static SpaceMarineRank ChiefLibrarian = new SpaceMarineRank(3, 4, true, "Master of the Librarium");
        public static SpaceMarineRank ChiefChaplain = new SpaceMarineRank(4, 4, true, "Master of Sanctity");
        public static SpaceMarineRank Reclusiarch = new SpaceMarineRank(5, 3, true, "Reclusiarch");
        public static SpaceMarineRank ChapterAncient = new SpaceMarineRank(6, 8, false, "Chapter Ancient");
        public static SpaceMarineRank ChapterChampion = new SpaceMarineRank(7, 8, false, "Chapter Champion");
        public static SpaceMarineRank VeteranCaptain = new SpaceMarineRank(8, 4, true, "Veteran Captain");
        public static SpaceMarineRank VeteranSquadSergeant = new SpaceMarineRank(9, 6, false, "Veteran Sergeant");
        public static SpaceMarineRank VeteranMarine = new SpaceMarineRank(10, 4, false, "Veteran");
        public static SpaceMarineRank VeteranAncient = new SpaceMarineRank(11, 7, false, "Veteran Ancient");
        public static SpaceMarineRank VeteranChampion = new SpaceMarineRank(12, 7, false, "Veteran Champion");
        public static SpaceMarineRank Captain = new SpaceMarineRank(13, 3, true, "Captain");
        public static SpaceMarineRank Chaplain = new SpaceMarineRank(14, 4, false, "Chaplain");
        public static SpaceMarineRank Ancient = new SpaceMarineRank(15, 3, false, "Ancient");
        public static SpaceMarineRank Apothecary = new SpaceMarineRank(16, 3, false, "Apothecary");
        public static SpaceMarineRank Champion = new SpaceMarineRank(17, 3, false, "Champion");
        public static SpaceMarineRank TacticalSergeant = new SpaceMarineRank(18, 3, false, "Sergeant");
        public static SpaceMarineRank TacticalMarine = new SpaceMarineRank(19, 2, false, "Tactical Marine");
        public static SpaceMarineRank AssaultSergeant = new SpaceMarineRank(20, 3, false, "Sergeant");
        public static SpaceMarineRank AssaultMarine = new SpaceMarineRank(21, 2, false, "Assault Marine");
        public static SpaceMarineRank DevestatorSergeant = new SpaceMarineRank(22, 3, false, "Sergeant");
        public static SpaceMarineRank DevestatorMarine = new SpaceMarineRank(23, 2, false, "Devestator Marine");
        public static SpaceMarineRank TechMarine = new SpaceMarineRank(24, 3, false, "Techmarine");
        public static SpaceMarineRank ScoutSergeant = new SpaceMarineRank(25, 2, false, "Scout Sergeant");
        public static SpaceMarineRank Scout = new SpaceMarineRank(26, 1, false, "Scout");
        public static SpaceMarineRank VeteranTechmarine = new SpaceMarineRank(27, 3, false, "Techmarine Supreme");
        public static SpaceMarineRank Epistolary = new SpaceMarineRank(28, 4, false, "Epistolary");
        public static SpaceMarineRank Codicier = new SpaceMarineRank(29, 3, false, "Codicier");
        public static SpaceMarineRank Lexicanius = new SpaceMarineRank(30, 2, false, "Lexicanius");
        public static SpaceMarineRank Acolyte = new SpaceMarineRank(31, 1, false, "Acolyte");
        public static SpaceMarineRank VeteranApothecary = new SpaceMarineRank(32, 2, false, "Veteran Apothecary");
    }

    public sealed class TempUnitTemplates
    {
        private static TempUnitTemplates _instance = null;
        public UnitTemplate VeteranSquadTemplate { get; private set; }
        public UnitTemplate TacticalSquadTemplate { get; private set; }
        public UnitTemplate AssaultSquadTemplate { get; private set; }
        public UnitTemplate DevestatorSquadTemplate { get; private set; }
        public UnitTemplate ScoutSquadTemplate { get; private set; }
        public UnitTemplate VeteranCompanyTemplate { get; private set; }
        public UnitTemplate BattleCompanyTemplate { get; private set; }
        public UnitTemplate TacticalCompanyTemplate { get; private set; }
        public UnitTemplate AssaultCompanyTemplate { get; private set; }
        public UnitTemplate DevestatorCompanyTemplate { get; private set; }
        public UnitTemplate ScoutCompanyTemplate { get; private set; }
        public UnitTemplate ChapterTemplate { get; private set; }

        private TempUnitTemplates()
        {
            VeteranSquadTemplate = CreateVeteranSquad();
            TacticalSquadTemplate = CreateTacticalSquad();
            AssaultSquadTemplate = CreateAssaultSquad();
            DevestatorSquadTemplate = CreateDevestatorSquad();
            ScoutSquadTemplate = CreateScoutSquad();
            VeteranCompanyTemplate = CreateVeteranCompany();
            BattleCompanyTemplate = CreateBattleCompany();
            TacticalCompanyTemplate = CreateTacticalCompany();
            AssaultCompanyTemplate = CreateAssaultCompany();
            DevestatorCompanyTemplate = CreateDevestatorCompany();
            ScoutCompanyTemplate = CreateScoutCompany();
            ChapterTemplate = CreateChapter();
        }

        public static TempUnitTemplates Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempUnitTemplates();
                }
                return _instance;
            }
        }

        private UnitTemplate CreateVeteranSquad()
        {
            UnitTemplate veteranSquad = new UnitTemplate(1, "Veteran Squad");
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranSquadSergeant);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpaceMarineRanks.VeteranMarine);
            return veteranSquad;
        }

        private UnitTemplate CreateTacticalSquad()
        {
            UnitTemplate tacticalSquad = new UnitTemplate(2, "Tactical Squad");
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalSergeant);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpaceMarineRanks.TacticalMarine);
            return tacticalSquad;
        }

        private UnitTemplate CreateAssaultSquad()
        {
            UnitTemplate assaultSquad = new UnitTemplate(3, "Assault Squad");
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultSergeant);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpaceMarineRanks.AssaultMarine);
            return assaultSquad;
        }

        private UnitTemplate CreateDevestatorSquad()
        {
            UnitTemplate devestatorSquad = new UnitTemplate(4, "Devestator Squad");
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorSergeant);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpaceMarineRanks.DevestatorMarine);
            return devestatorSquad;
        }

        private UnitTemplate CreateScoutSquad()
        {
            UnitTemplate scoutSquad = new UnitTemplate(5, "Scout Squad");
            scoutSquad.Members.Add(TempSpaceMarineRanks.ScoutSergeant);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            scoutSquad.Members.Add(TempSpaceMarineRanks.Scout);
            return scoutSquad;
        }

        private UnitTemplate CreateVeteranCompany()
        {
            UnitTemplate vetCompany = new UnitTemplate(6, "Veteran Company");
            vetCompany.Members.Add(TempSpaceMarineRanks.VeteranCaptain);
            vetCompany.Members.Add(TempSpaceMarineRanks.VeteranChampion);
            vetCompany.Members.Add(TempSpaceMarineRanks.VeteranAncient);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            vetCompany.ChildUnits.Add(VeteranSquadTemplate);
            return vetCompany;
        }

        private UnitTemplate CreateBattleCompany()
        {
            UnitTemplate battleCompany = new UnitTemplate(7, "Battle Company");
            battleCompany.Members.Add(TempSpaceMarineRanks.Captain);
            battleCompany.Members.Add(TempSpaceMarineRanks.Champion);
            battleCompany.Members.Add(TempSpaceMarineRanks.Ancient);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(TacticalSquadTemplate);
            battleCompany.ChildUnits.Add(AssaultSquadTemplate);
            battleCompany.ChildUnits.Add(DevestatorSquadTemplate);
            return battleCompany;
        }

        private UnitTemplate CreateTacticalCompany()
        {
            UnitTemplate company = new UnitTemplate(8, "Tactical Company");
            company.Members.Add(TempSpaceMarineRanks.Captain);
            company.Members.Add(TempSpaceMarineRanks.Champion);
            company.Members.Add(TempSpaceMarineRanks.Ancient);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            company.ChildUnits.Add(TacticalSquadTemplate);
            return company;
        }

        private UnitTemplate CreateAssaultCompany()
        {
            UnitTemplate company = new UnitTemplate(9, "Tactical Company");
            company.Members.Add(TempSpaceMarineRanks.Captain);
            company.Members.Add(TempSpaceMarineRanks.Champion);
            company.Members.Add(TempSpaceMarineRanks.Ancient);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            company.ChildUnits.Add(AssaultSquadTemplate);
            return company;
        }

        private UnitTemplate CreateDevestatorCompany()
        {
            UnitTemplate company = new UnitTemplate(10, "Devestator Company");
            company.Members.Add(TempSpaceMarineRanks.Captain);
            company.Members.Add(TempSpaceMarineRanks.Champion);
            company.Members.Add(TempSpaceMarineRanks.Ancient);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            company.ChildUnits.Add(DevestatorSquadTemplate);
            return company;
        }

        private UnitTemplate CreateScoutCompany()
        {
            UnitTemplate company = new UnitTemplate(11, "Scout Company");
            company.Members.Add(TempSpaceMarineRanks.Captain);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            company.ChildUnits.Add(ScoutSquadTemplate);
            return company;
        }

        private UnitTemplate CreateChapter()
        {
            UnitTemplate chapter = new UnitTemplate(16, "Chapter");
            chapter.Members.Add(TempSpaceMarineRanks.ChapterMaster);
            chapter.Members.Add(TempSpaceMarineRanks.ChiefApothecary);
            chapter.Members.Add(TempSpaceMarineRanks.ForgeMaster);
            chapter.Members.Add(TempSpaceMarineRanks.ChiefLibrarian);
            chapter.Members.Add(TempSpaceMarineRanks.ChiefChaplain);
            chapter.Members.Add(TempSpaceMarineRanks.ChapterAncient);
            chapter.ChildUnits.Add(CreateArmory());
            chapter.ChildUnits.Add(CreateLibrarius());
            chapter.ChildUnits.Add(CreateApothecarion());
            chapter.ChildUnits.Add(CreateReclusium());
            chapter.ChildUnits.Add(VeteranCompanyTemplate);
            chapter.ChildUnits.Add(BattleCompanyTemplate);
            chapter.ChildUnits.Add(BattleCompanyTemplate);
            chapter.ChildUnits.Add(BattleCompanyTemplate);
            chapter.ChildUnits.Add(BattleCompanyTemplate);
            chapter.ChildUnits.Add(TacticalCompanyTemplate);
            chapter.ChildUnits.Add(TacticalCompanyTemplate);
            chapter.ChildUnits.Add(AssaultCompanyTemplate);
            chapter.ChildUnits.Add(DevestatorCompanyTemplate);
            chapter.ChildUnits.Add(ScoutCompanyTemplate);
            return chapter;
        }

        private UnitTemplate CreateArmory()
        {
            UnitTemplate armory = new UnitTemplate(12, "Armory");
            armory.Members.Add(TempSpaceMarineRanks.VeteranTechmarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            armory.Members.Add(TempSpaceMarineRanks.TechMarine);
            return armory;
        }

        private UnitTemplate CreateLibrarius()
        {
            UnitTemplate library = new UnitTemplate(13, "Librarius");
            library.Members.Add(TempSpaceMarineRanks.Epistolary);
            library.Members.Add(TempSpaceMarineRanks.Codicier);
            library.Members.Add(TempSpaceMarineRanks.Lexicanius);
            library.Members.Add(TempSpaceMarineRanks.Acolyte);
            return library;
        }

        private UnitTemplate CreateApothecarion()
        {
            UnitTemplate apothecary = new UnitTemplate(14, "Apothecarion");
            apothecary.Members.Add(TempSpaceMarineRanks.VeteranApothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            apothecary.Members.Add(TempSpaceMarineRanks.Apothecary);
            return apothecary;
        }

        private UnitTemplate CreateReclusium()
        {
            UnitTemplate reclusium = new UnitTemplate(15, "Reclusium");
            reclusium.Members.Add(TempSpaceMarineRanks.Reclusiarch);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            reclusium.Members.Add(TempSpaceMarineRanks.Chaplain);
            return reclusium;
        }
    }
}
