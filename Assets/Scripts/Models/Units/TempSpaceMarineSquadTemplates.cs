using System.Collections.Generic;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public sealed class TempSpaceMarineWeaponSets
    {
        private static TempSpaceMarineWeaponSets _instance = null;
        public static TempSpaceMarineWeaponSets Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempSpaceMarineWeaponSets();
                }
                return _instance;
            }
        }
        private TempSpaceMarineWeaponSets()
        {
            ImperialEquippables eq = ImperialEquippables.Instance;
            Bolter = new WeaponSet { Name = "Boltgun", PrimaryRangedWeapon = eq.Boltgun };
            BoltPistolChainSword = new WeaponSet { Name = "Bolt Pistol & Chainsword", PrimaryRangedWeapon = eq.BoltPistol, PrimaryMeleeWeapon = eq.Chainsword };
            BolterPlusPistol = new WeaponSet { Name = "Boltgun & Bolt Pistol", PrimaryRangedWeapon = eq.Boltgun, SecondaryRangedWeapon = eq.BoltPistol };
            Flamer = new WeaponSet { Name = "Flamer", PrimaryRangedWeapon = eq.Flamer };
            HeavyBolter = new WeaponSet { Name = "Heavy Bolter", PrimaryRangedWeapon = eq.HeavyBolter };
            Lascannon = new WeaponSet { Name = "Lascannon", PrimaryRangedWeapon = eq.Lascannon };
            MeltaGun = new WeaponSet { Name = "Meltagun", PrimaryRangedWeapon = eq.MeltaGun };
            Missile = new WeaponSet { Name = "Missile Launcher", PrimaryRangedWeapon = eq.MissileLauncher };
            MultiMelta = new WeaponSet { Name = "Multi-melta", PrimaryRangedWeapon = eq.MultiMelta };
            PlasmaCannon = new WeaponSet { Name = "Plasma Cannon", PrimaryRangedWeapon = eq.PlasmaCannon };
            PlasmaGun = new WeaponSet { Name = "Plasma Gun", PrimaryRangedWeapon = eq.PlasmaGun };
            PlasmaPistolChainSword = new WeaponSet { Name = "Plasma Pistol & Chainsword", PrimaryRangedWeapon = eq.PlasmaPistol, PrimaryMeleeWeapon = eq.Chainsword };
            Shotgun = new WeaponSet { Name = "Shotgun", PrimaryRangedWeapon = eq.Shotgun };
            SniperRifle = new WeaponSet { Name = "Sniper Rifle", PrimaryRangedWeapon = eq.SniperRifle };
            Eviscerator = new WeaponSet { Name = "Eviscerator", PrimaryMeleeWeapon = eq.Eviscerator };
        }

        public WeaponSet Bolter { get; private set; }
        public WeaponSet BoltPistolChainSword { get; private set; }
        public WeaponSet BolterPlusPistol { get; private set; }
        public WeaponSet Flamer { get; private set; }
        public WeaponSet HeavyBolter { get; private set; }
        public WeaponSet Lascannon { get; private set; }
        public WeaponSet MeltaGun { get; private set; }
        public WeaponSet Missile { get; private set; }
        public WeaponSet MultiMelta { get; private set; }
        public WeaponSet PlasmaCannon { get; private set; }
        public WeaponSet PlasmaGun { get; private set; }
        public WeaponSet PlasmaPistolChainSword { get; private set; }
        public WeaponSet Shotgun { get; private set; }
        public WeaponSet SniperRifle { get; private set; }

        public WeaponSet Eviscerator { get; private set; }
    }

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
            SquadTemplate veteranSquad = new SquadTemplate(1, "Veteran Squad", TempSpaceMarineWeaponSets.Instance.BoltPistolChainSword, null, ImperialEquippables.Instance.PowerArmor);
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
            List<WeaponSet> options1 = new List<WeaponSet>
            {
                ws.Flamer,
                ws.PlasmaGun,
                ws.MeltaGun
            };
            UnitWeaponOption uwo1 = new UnitWeaponOption("Specialty Weapons", 0, 1, options1);

            List<WeaponSet> options2 = new List<WeaponSet>
            {
                ws.HeavyBolter,
                ws.Lascannon,
                ws.Missile,
                ws.MultiMelta,
                ws.PlasmaCannon
            };
            UnitWeaponOption uwo2 = new UnitWeaponOption("Heavy Weapons", 0, 1, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>
            {
                uwo1,
                uwo2
            };

            SquadTemplate tacticalSquad = new SquadTemplate(2, "Tactical Squad", ws.Bolter, options, ImperialEquippables.Instance.PowerArmor);
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
            List<WeaponSet> options1 = new List<WeaponSet>
            {
                ws.Flamer,
                ws.PlasmaPistolChainSword
            };

            UnitWeaponOption uwo1 = new UnitWeaponOption("Specialty Weapons", 0, 2, options1);

            List<WeaponSet> options2 = new List<WeaponSet>
            {
                ws.Eviscerator
            };
            UnitWeaponOption uwo2 = new UnitWeaponOption("Two-handed Sword", 0, 2, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>
            {
                uwo1,
                uwo2
            };

            SquadTemplate assaultSquad = new SquadTemplate(3, "Assault Squad", ws.BoltPistolChainSword, options, ImperialEquippables.Instance.PowerArmor);
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
            List<WeaponSet> options2 = new List<WeaponSet>
            {
                ws.HeavyBolter,
                ws.Lascannon,
                ws.Missile,
                ws.MultiMelta,
                ws.PlasmaCannon
            };
            UnitWeaponOption uwo2 = new UnitWeaponOption("Heavy Weapons", 0, 4, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>
            {
                uwo2
            };

            SquadTemplate devastatorSquad = new SquadTemplate(4, "Devastator Squad", ws.Bolter, options, ImperialEquippables.Instance.PowerArmor);
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
            List<WeaponSet> options1 = new List<WeaponSet>
            {
                ws.Shotgun,
                ws.SniperRifle
            };
            UnitWeaponOption uwo1 = new UnitWeaponOption("Optional Armament", 0, 9, options1);

            List<WeaponSet> options2 = new List<WeaponSet>
            {
                ws.HeavyBolter,
                ws.Missile
            };
            UnitWeaponOption uwo2 = new UnitWeaponOption("Heavy Weapons", 0, 1, options2);

            List<UnitWeaponOption> options = new List<UnitWeaponOption>
            {
                uwo1,
                uwo2
            };


            SquadTemplate scoutSquad = new SquadTemplate(5, "Scout Squad", ws.BolterPlusPistol, options, ImperialEquippables.Instance.CarapaceArmor);
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
            SquadTemplate hqSquad = new SquadTemplate(6, "HQ Squad", TempSpaceMarineWeaponSets.Instance.BolterPlusPistol, null, ImperialEquippables.Instance.PowerArmor);
            hqSquad.AddSquadLeader(TempSpaceMarineTemplate.Instance);
            hqSquad.Members.Add(TempSpaceMarineTemplate.Instance);
            hqSquad.Members.Add(TempSpaceMarineTemplate.Instance);

            return hqSquad;
        }

        private SquadTemplate CreateArmory()
        {
            SquadTemplate armory = new SquadTemplate(7, "Armory", TempSpaceMarineWeaponSets.Instance.Bolter, null, ImperialEquippables.Instance.PowerArmor);
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
            SquadTemplate library = new SquadTemplate(8, "Librarius", TempSpaceMarineWeaponSets.Instance.Bolter, null, ImperialEquippables.Instance.PowerArmor);
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
            SquadTemplate apothecary = new SquadTemplate(14, "Apothecarion", TempSpaceMarineWeaponSets.Instance.Bolter, null, ImperialEquippables.Instance.PowerArmor);
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
            SquadTemplate reclusium = new SquadTemplate(15, "Reclusium", TempSpaceMarineWeaponSets.Instance.Bolter, null, ImperialEquippables.Instance.PowerArmor);
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
