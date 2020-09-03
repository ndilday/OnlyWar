using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public sealed class TempSpaceMarineSquadTemplates
    {
        public SquadTemplate VeteranSquadTemplate { get; private set; }
        public SquadTemplate TacticalSquadTemplate { get; private set; }
        public SquadTemplate AssaultSquadTemplate { get; private set; }
        public SquadTemplate DevastatorSquadTemplate { get; private set; }
        public SquadTemplate ScoutSquadTemplate { get; private set; }

        public SquadTemplate CompanyHQSquadTemplate { get; private set; }
        public SquadTemplate Armory { get; private set; }
        public SquadTemplate Librarius { get; private set; }
        public SquadTemplate Apothecarion { get; private set; }
        public SquadTemplate Reclusium { get; private set; }

        private static TempSpaceMarineSquadTemplates _instance;
        public static TempSpaceMarineSquadTemplates Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempSpaceMarineSquadTemplates();
                }
                return _instance;
            }
        }

        private TempSpaceMarineSquadTemplates()
        {
            VeteranSquadTemplate = CreateVeteranSquad();
            TacticalSquadTemplate = CreateTacticalSquad();
            AssaultSquadTemplate = CreateAssaultSquad();
            DevastatorSquadTemplate = CreateDevastatorSquad();
            ScoutSquadTemplate = CreateScoutSquad();
            CompanyHQSquadTemplate = CreateCompanyHQSquad();
            Armory = CreateArmory();
            Librarius = CreateLibrarius();
            Apothecarion = CreateApothecarion();
            Reclusium = CreateReclusium();
        }

        private SquadTemplate CreateVeteranSquad()
        {
            SquadTemplate veteranSquad = new SquadTemplate(1, "Veteran Squad", TempSpaceMarineWeaponSets.Instance.BoltPistolChainSword, null);
            veteranSquad.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            veteranSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            return veteranSquad;
        }

        private SquadTemplate CreateTacticalSquad()
        {
            TempSpaceMarineWeaponSets ws = TempSpaceMarineWeaponSets.Instance;
            List<WeaponSet> options1 = new List<WeaponSet>();
            options1.Add(ws.Flamer);
            options1.Add(ws.PlasmaGun);
            options1.Add(ws.MeltaGun);
            UnitWeaponOption uwo1 = new UnitWeaponOption("Specialty Weapons", 0, 1, options1);

            List<WeaponSet> options2 = new List<WeaponSet>();
            options2.Add(ws.HeavyBolter);
            options2.Add(ws.Lascannon);
            options2.Add(ws.Missile);
            options2.Add(ws.MultiMelta);
            options2.Add(ws.PlasmaCannon);
            UnitWeaponOption uwo2 = new UnitWeaponOption("Heavy Weapons", 0, 1, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>();
            options.Add(uwo1);
            options.Add(uwo2);

            SquadTemplate tacticalSquad = new SquadTemplate(2, "Tactical Squad", ws.Bolter, options);
            tacticalSquad.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            tacticalSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            return tacticalSquad;
        }

        private SquadTemplate CreateAssaultSquad()
        {
            TempSpaceMarineWeaponSets ws = TempSpaceMarineWeaponSets.Instance;
            List<WeaponSet> options1 = new List<WeaponSet>();
            options1.Add(ws.Flamer);
            options1.Add(ws.PlasmaPistolChainSword);

            UnitWeaponOption uwo1 = new UnitWeaponOption("Specialty Weapons", 0, 2, options1);

            List<WeaponSet> options2 = new List<WeaponSet>();
            options2.Add(ws.Eviscerator);
            UnitWeaponOption uwo2 = new UnitWeaponOption("Two-handed Sword", 0, 2, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>();
            options.Add(uwo1);
            options.Add(uwo2);

            SquadTemplate assaultSquad = new SquadTemplate(3, "Assault Squad", ws.BoltPistolChainSword, options);
            assaultSquad.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            assaultSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            return assaultSquad;
        }

        private SquadTemplate CreateDevastatorSquad()
        {
            TempSpaceMarineWeaponSets ws = TempSpaceMarineWeaponSets.Instance;
            List<WeaponSet> options2 = new List<WeaponSet>();
            options2.Add(ws.HeavyBolter);
            options2.Add(ws.Lascannon);
            options2.Add(ws.Missile);
            options2.Add(ws.MultiMelta);
            options2.Add(ws.PlasmaCannon);
            UnitWeaponOption uwo2 = new UnitWeaponOption("Heavy Weapons", 0, 4, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>();
            options.Add(uwo2);

            SquadTemplate devastatorSquad = new SquadTemplate(4, "Devastator Squad", ws.Bolter, options);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            devastatorSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            return devastatorSquad;
        }

        private SquadTemplate CreateScoutSquad()
        {
            TempSpaceMarineWeaponSets ws = TempSpaceMarineWeaponSets.Instance;
            List<WeaponSet> options1 = new List<WeaponSet>();
            options1.Add(ws.Shotgun);
            options1.Add(ws.SniperRifle);
            UnitWeaponOption uwo1 = new UnitWeaponOption("Optional Armament", 0, 9, options1);

            List<WeaponSet> options2 = new List<WeaponSet>();
            options2.Add(ws.HeavyBolter);
            options2.Add(ws.Missile);
            UnitWeaponOption uwo2 = new UnitWeaponOption("Heavy Weapons", 0, 1, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>();
            options.Add(uwo1);
            options.Add(uwo2);


            SquadTemplate scoutSquad = new SquadTemplate(5, "Scout Squad", ws.BolterPlusPistol, options);
            scoutSquad.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            scoutSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            return scoutSquad;
        }
    
        private SquadTemplate CreateCompanyHQSquad()
        {
            SquadTemplate hqSquad = new SquadTemplate(6, "HQ Squad", TempSpaceMarineWeaponSets.Instance.BolterPlusPistol, null);
            hqSquad.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            hqSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            hqSquad.Members.Add(TempSpaceMarineTemplate.Instance);

            return hqSquad;
        }

        private SquadTemplate CreateArmory()
        {
            SquadTemplate armory = new SquadTemplate(7, "Armory", TempSpaceMarineWeaponSets.Instance.Bolter, null);
            armory.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            armory.Members.Add(TempSpaceMarineTemplate.Instance);
            return armory;
        }

        private SquadTemplate CreateLibrarius()
        {
            SquadTemplate library = new SquadTemplate(8, "Librarius", TempSpaceMarineWeaponSets.Instance.Bolter, null);
            library.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            library.Members.Add(TempSpaceMarineTemplate.Instance);
            return library;
        }

        private SquadTemplate CreateApothecarion()
        {
            SquadTemplate apothecary = new SquadTemplate(14, "Apothecarion", TempSpaceMarineWeaponSets.Instance.Bolter, null);
            apothecary.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            apothecary.Members.Add(TempSpaceMarineTemplate.Instance);
            return apothecary;
        }

        private SquadTemplate CreateReclusium()
        {
            SquadTemplate reclusium = new SquadTemplate(15, "Reclusium", TempSpaceMarineWeaponSets.Instance.Bolter, null);
            reclusium.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            reclusium.Members.Add(TempSpaceMarineTemplate.Instance);
            return reclusium;
        }
    }
}
