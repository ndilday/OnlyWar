using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using System.Collections.Generic;
using System.Xml;

namespace Iam.Scripts.Models.Units
{
    public sealed class TempSpaceMarineWeaponSets
    {
        private static TempSpaceMarineWeaponSets _instance = null;
        public static TempSpaceMarineWeaponSets Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempSpaceMarineWeaponSets();
                }
                return _instance;
            }
        }
        private TempSpaceMarineWeaponSets() 
        {
            ImperialEquippables eq = ImperialEquippables.Instance;
            Bolter = new WeaponSet { Name="Boltgun", MainWeapon = eq.Boltgun, SecondaryWeapon = null };
            BoltPistolChainSword = new WeaponSet { Name="Bolt Pistol & Chainsword", MainWeapon = eq.BoltPistol, SecondaryWeapon = eq.Chainsword };
            BolterPlusPistol = new WeaponSet { Name = "Boltgun & Bolt Pistol", MainWeapon = eq.Boltgun, SecondaryWeapon = eq.BoltPistol };
            Flamer = new WeaponSet { Name="Flamer", MainWeapon = eq.Flamer, SecondaryWeapon = null };
            HeavyBolter = new WeaponSet { Name="Heavy Bolter", MainWeapon = eq.HeavyBolter, SecondaryWeapon = null };
            Lascannon = new WeaponSet { Name="Lascannon", MainWeapon = eq.Lascannon, SecondaryWeapon = null };
            MeltaGun = new WeaponSet { Name="Meltagun", MainWeapon = eq.MeltaGun, SecondaryWeapon = null };
            Missile = new WeaponSet { Name="Missile Launcher", MainWeapon = eq.MissileLauncher, SecondaryWeapon = null };
            MultiMelta = new WeaponSet { Name="Multi-melta", MainWeapon = eq.MultiMelta, SecondaryWeapon = null };
            PlasmaCannon = new WeaponSet { Name="Plasma Cannon", MainWeapon = eq.PlasmaCannon, SecondaryWeapon = null };
            PlasmaGun = new WeaponSet { Name="Plasma Gun", MainWeapon = eq.PlasmaGun, SecondaryWeapon = null };
            PlasmaPistolChainSword = new WeaponSet { Name="Plasma Pistol & Chainsword", MainWeapon = eq.PlasmaPistol, SecondaryWeapon = eq.Chainsword };
            Shotgun = new WeaponSet { Name="Shotgun", MainWeapon = eq.Shotgun, SecondaryWeapon = null };
            SniperRifle = new WeaponSet { Name="Sniper Rifle", MainWeapon = eq.SniperRifle, SecondaryWeapon = null };
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

    
    public sealed class TempTyranidUnitTempates
    {
        private TempTyranidUnitTempates _instance;
        public TempTyranidUnitTempates Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempTyranidUnitTempates();
                }
                return _instance;
            }
        }

        private TempTyranidUnitTempates() 
        { 
        }

        public UnitTemplate HormagauntSquadTemplate { get; private set; }
        public UnitTemplate TermagauntSquadTemplate { get; private set; }
        public UnitTemplate TyranidWarriorTemplate { get; private set; }
    }
}
