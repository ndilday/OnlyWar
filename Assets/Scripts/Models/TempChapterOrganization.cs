using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    public sealed class TempChapterOrganization
    {
        private static TempChapterOrganization instance = null;
        public UnitTemplate Chapter { get; private set; }
        private UnitTemplate _veteranSquad;
        private UnitTemplate _tacticalSquad;
        private UnitTemplate _assaultSquad;
        private UnitTemplate _devestatorSquad;
        private UnitTemplate _scoutSquad;
        private TempChapterOrganization()
        {
            _veteranSquad = CreateVeteranSquad();
            _tacticalSquad = CreateTacticalSquad();
            _assaultSquad = CreateAssaultSquad();
            _devestatorSquad = CreateDevestatorSquad();
            Chapter = CreateChapter();
        }

        private UnitTemplate CreateVeteranSquad()
        {
            UnitTemplate veteranSquad = new UnitTemplate(11, "Veteran Squad");
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
            UnitTemplate tacticalSquad = new UnitTemplate(12, "Tactical Squad");
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
            UnitTemplate assaultSquad = new UnitTemplate(13, "Assault Squad");
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
            UnitTemplate devestatorSquad = new UnitTemplate(14, "Devestator Squad");
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

        private UnitTemplate CreateChapter()
        {
            UnitTemplate chapter = new UnitTemplate(0, "Chapter");
            chapter.Members.Add(TempSpecialtyRanks.ChapterMaster);
            chapter.Members.Add(TempSpecialtyRanks.ChiefApothecary);
            chapter.Members.Add(TempSpecialtyRanks.ForgeMaster);
            chapter.Members.Add(TempSpecialtyRanks.ChiefLibrarian);
            chapter.Members.Add(TempSpecialtyRanks.ChiefChaplain);
            chapter.Members.Add(TempSpecialtyRanks.ChapterAncient);
            chapter.ChildUnits.Add(CreateFirstCompany());
            chapter.ChildUnits.Add(CreateBattleCompany(2, "Second Company"));
            chapter.ChildUnits.Add(CreateBattleCompany(3, "Third Company"));
            chapter.ChildUnits.Add(CreateBattleCompany(4, "Fourth Company"));
            chapter.ChildUnits.Add(CreateBattleCompany(5, "Fifth Company"));
            chapter.ChildUnits.Add(CreateTacticalCompany(6, "Sixth Company"));
            chapter.ChildUnits.Add(CreateTacticalCompany(7, "Seventh Company"));
            chapter.ChildUnits.Add(CreateAssaultCompany(8, "Eighth Company"));
            chapter.ChildUnits.Add(CreateDevestatorCompany(9, "Ninth Company"));
            chapter.ChildUnits.Add(CreateScoutCompany(10, "Tenth Company"));
            return chapter;
        }

        private UnitTemplate CreateFirstCompany()
        {
            UnitTemplate firstCompany = new UnitTemplate(1, "First Company");
            firstCompany.Members.Add(TempSpecialtyRanks.VeteranCaptain);
            firstCompany.Members.Add(TempSpecialtyRanks.Reclusiarch);
            firstCompany.Members.Add(TempSpecialtyRanks.VeteranChampion);
            firstCompany.Members.Add(TempSpecialtyRanks.VeteranAncient);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            firstCompany.ChildUnits.Add(_veteranSquad);
            return firstCompany;
        }

        private UnitTemplate CreateBattleCompany(int id, string name)
        {
            UnitTemplate battleCompany = new UnitTemplate(id, name);
            battleCompany.Members.Add(TempSpecialtyRanks.Captain);
            battleCompany.Members.Add(TempSpecialtyRanks.Chaplain);
            battleCompany.Members.Add(TempSpecialtyRanks.Champion);
            battleCompany.Members.Add(TempSpecialtyRanks.Ancient);
            battleCompany.ChildUnits.Add(_tacticalSquad);
            battleCompany.ChildUnits.Add(_tacticalSquad);
            battleCompany.ChildUnits.Add(_tacticalSquad);
            battleCompany.ChildUnits.Add(_tacticalSquad);
            battleCompany.ChildUnits.Add(_tacticalSquad);
            battleCompany.ChildUnits.Add(_tacticalSquad);
            battleCompany.ChildUnits.Add(_assaultSquad);
            battleCompany.ChildUnits.Add(_assaultSquad);
            battleCompany.ChildUnits.Add(_devestatorSquad);
            battleCompany.ChildUnits.Add(_devestatorSquad);
            return battleCompany;
        }

        private UnitTemplate CreateTacticalCompany(int id, string name)
        {
            UnitTemplate tacticalCompany = new UnitTemplate(id, name);
            tacticalCompany.Members.Add(TempSpecialtyRanks.Captain);
            tacticalCompany.Members.Add(TempSpecialtyRanks.Chaplain);
            tacticalCompany.Members.Add(TempSpecialtyRanks.Champion);
            tacticalCompany.Members.Add(TempSpecialtyRanks.Ancient);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            tacticalCompany.ChildUnits.Add(_tacticalSquad);
            return tacticalCompany;
        }

        private UnitTemplate CreateAssaultCompany(int id, string name)
        {
            UnitTemplate assaultCompany = new UnitTemplate(id, name);
            assaultCompany.Members.Add(TempSpecialtyRanks.Captain);
            assaultCompany.Members.Add(TempSpecialtyRanks.Chaplain);
            assaultCompany.Members.Add(TempSpecialtyRanks.Champion);
            assaultCompany.Members.Add(TempSpecialtyRanks.Ancient);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            assaultCompany.ChildUnits.Add(_assaultSquad);
            return assaultCompany;
        }

        private UnitTemplate CreateDevestatorCompany(int id, string name)
        {
            UnitTemplate devestatorCompany = new UnitTemplate(id, name);
            devestatorCompany.Members.Add(TempSpecialtyRanks.Captain);
            devestatorCompany.Members.Add(TempSpecialtyRanks.Chaplain);
            devestatorCompany.Members.Add(TempSpecialtyRanks.Champion);
            devestatorCompany.Members.Add(TempSpecialtyRanks.Ancient);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            devestatorCompany.ChildUnits.Add(_devestatorSquad);
            return devestatorCompany;
        }

        private UnitTemplate CreateScoutCompany(int id, string name)
        {
            UnitTemplate scoutCompany = new UnitTemplate(id, name);
            scoutCompany.Members.Add(TempSpecialtyRanks.Captain);
            scoutCompany.Members.Add(TempSpecialtyRanks.Chaplain);
            scoutCompany.Members.Add(TempSpecialtyRanks.Champion);
            scoutCompany.Members.Add(TempSpecialtyRanks.Ancient);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            scoutCompany.ChildUnits.Add(_scoutSquad);
            return scoutCompany;
        }

        public static TempChapterOrganization Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new TempChapterOrganization();
                }
                return instance;
            }
        }
    }
}
