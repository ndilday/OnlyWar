using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine.Rendering;

namespace Iam.Scripts.Models
{
    public static class TempSpecialtyRanks
    {
        public static SpecialtyRank ChapterMaster = new SpecialtyRank(0, 5, true, "Chapter Master");
        public static SpecialtyRank ChiefApothecary = new SpecialtyRank(1, 4, true, "Master of the Apothecarion");
        public static SpecialtyRank ForgeMaster = new SpecialtyRank(2, 4, true, "Master of the Forge");
        public static SpecialtyRank ChiefLibrarian = new SpecialtyRank(3, 4, true, "Master of the Librarium");
        public static SpecialtyRank ChiefChaplain = new SpecialtyRank(4, 4, true, "Master of Sanctity");
        public static SpecialtyRank Reclusiarch = new SpecialtyRank(5, 3, true, "Reclusiarch");
        public static SpecialtyRank ChapterAncient = new SpecialtyRank(6, 8, false, "Chapter Ancient");
        public static SpecialtyRank ChapterChampion = new SpecialtyRank(7, 8, false, "Chapter Champion");
        public static SpecialtyRank VeteranCaptain = new SpecialtyRank(8, 4, true, "Veteran Captain");
        public static SpecialtyRank VeteranSquadSergeant = new SpecialtyRank(9, 6, false, "Veteran Sergeant");
        public static SpecialtyRank VeteranMarine = new SpecialtyRank(10, 4, false, "Veteran");
        public static SpecialtyRank VeteranAncient = new SpecialtyRank(11, 7, false, "Veteran Ancient");
        public static SpecialtyRank VeteranChampion = new SpecialtyRank(12, 7, false, "Veteran Champion");
        public static SpecialtyRank Captain = new SpecialtyRank(13, 3, true, "Captain");
        public static SpecialtyRank Chaplain = new SpecialtyRank(14, 4, false, "Chaplain");
        public static SpecialtyRank Ancient = new SpecialtyRank(15, 3, false, "Ancient");
        public static SpecialtyRank Apothecary = new SpecialtyRank(16, 3, false, "Apothecary");
        public static SpecialtyRank Champion = new SpecialtyRank(17, 3, false, "Champion");
        public static SpecialtyRank TacticalSergeant = new SpecialtyRank(18, 3, false, "Sergeant");
        public static SpecialtyRank TacticalMarine = new SpecialtyRank(19, 2, false, "Tactical Marine");
        public static SpecialtyRank AssaultSergeant = new SpecialtyRank(20, 3, false, "Sergeant");
        public static SpecialtyRank AssaultMarine = new SpecialtyRank(21, 2, false, "Assault Marine");
        public static SpecialtyRank DevestatorSergeant = new SpecialtyRank(22, 3, false, "Sergeant");
        public static SpecialtyRank DevestatorMarine = new SpecialtyRank(23, 2, false, "Devestator Marine");
        public static SpecialtyRank TechMarine = new SpecialtyRank(24, 3, false, "Techmarine");
        public static SpecialtyRank ScoutSergeant = new SpecialtyRank(25, 2, false, "Scout Sergeant");
        public static SpecialtyRank Scout = new SpecialtyRank(26, 1, false, "Scout");
        public static SpecialtyRank VeteranTechmarine = new SpecialtyRank(27, 3, false, "Techmarine Supreme");
        public static SpecialtyRank Epistolary = new SpecialtyRank(28, 4, false, "Epistolary");
        public static SpecialtyRank Codicier = new SpecialtyRank(29, 3, false, "Codicier");
        public static SpecialtyRank Lexicanius = new SpecialtyRank(30, 2, false, "Lexicanius");
        public static SpecialtyRank Acolyte = new SpecialtyRank(31, 1, false, "Acolyte");
        public static SpecialtyRank VeteranApothecary = new SpecialtyRank(32, 2, false, "Veteran Apothecary");
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
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranSquadSergeant);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            veteranSquad.Members.Add(TempSpecialtyRanks.VeteranMarine);
            return veteranSquad;
        }

        private UnitTemplate CreateTacticalSquad()
        {
            UnitTemplate tacticalSquad = new UnitTemplate(2, "Tactical Squad");
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalSergeant);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            tacticalSquad.Members.Add(TempSpecialtyRanks.TacticalMarine);
            return tacticalSquad;
        }

        private UnitTemplate CreateAssaultSquad()
        {
            UnitTemplate assaultSquad = new UnitTemplate(3, "Assault Squad");
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultSergeant);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            assaultSquad.Members.Add(TempSpecialtyRanks.AssaultMarine);
            return assaultSquad;
        }

        private UnitTemplate CreateDevestatorSquad()
        {
            UnitTemplate devestatorSquad = new UnitTemplate(4, "Devestator Squad");
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorSergeant);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            devestatorSquad.Members.Add(TempSpecialtyRanks.DevestatorMarine);
            return devestatorSquad;
        }

        private UnitTemplate CreateScoutSquad()
        {
            UnitTemplate scoutSquad = new UnitTemplate(5, "Scout Squad");
            scoutSquad.Members.Add(TempSpecialtyRanks.ScoutSergeant);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            scoutSquad.Members.Add(TempSpecialtyRanks.Scout);
            return scoutSquad;
        }

        private UnitTemplate CreateVeteranCompany()
        {
            UnitTemplate vetCompany = new UnitTemplate(6, "Veteran Company");
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
            chapter.Members.Add(TempSpecialtyRanks.ChapterMaster);
            chapter.Members.Add(TempSpecialtyRanks.ChiefApothecary);
            chapter.Members.Add(TempSpecialtyRanks.ForgeMaster);
            chapter.Members.Add(TempSpecialtyRanks.ChiefLibrarian);
            chapter.Members.Add(TempSpecialtyRanks.ChiefChaplain);
            chapter.Members.Add(TempSpecialtyRanks.ChapterAncient);
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
            armory.Members.Add(TempSpecialtyRanks.VeteranTechmarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            armory.Members.Add(TempSpecialtyRanks.TechMarine);
            return armory;
        }

        private UnitTemplate CreateLibrarius()
        {
            UnitTemplate library = new UnitTemplate(13, "Librarius");
            library.Members.Add(TempSpecialtyRanks.Epistolary);
            library.Members.Add(TempSpecialtyRanks.Codicier);
            library.Members.Add(TempSpecialtyRanks.Lexicanius);
            library.Members.Add(TempSpecialtyRanks.Acolyte);
            return library;
        }

        private UnitTemplate CreateApothecarion()
        {
            UnitTemplate apothecary = new UnitTemplate(14, "Apothecarion");
            apothecary.Members.Add(TempSpecialtyRanks.VeteranApothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            apothecary.Members.Add(TempSpecialtyRanks.Apothecary);
            return apothecary;
        }

        private UnitTemplate CreateReclusium()
        {
            UnitTemplate reclusium = new UnitTemplate(15, "Reclusium");
            reclusium.Members.Add(TempSpecialtyRanks.Reclusiarch);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            reclusium.Members.Add(TempSpecialtyRanks.Chaplain);
            return reclusium;
        }
    }
}
