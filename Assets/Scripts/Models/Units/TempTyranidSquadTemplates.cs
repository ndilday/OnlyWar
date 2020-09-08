using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Models.Units
{
    public sealed class TempTyranidWeaponSets
    {
        private static TempTyranidWeaponSets _instance;
        public static TempTyranidWeaponSets Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempTyranidWeaponSets();
                }
                return _instance;
            }
        }
        private TempTyranidWeaponSets()
        {
            Deathspitter = new WeaponSet { Name = "Deathspitter", PrimaryRangedWeapon = TempEquipment.Instance.Deathspitter };
            Devourer = new WeaponSet { Name = "Devourer", PrimaryRangedWeapon = TempEquipment.Instance.Devourer };
            ScythingTalons = new WeaponSet { Name = "Scything Talons", PrimaryMeleeWeapon = TempEquipment.Instance.ScythingTalons, SecondaryMeleeWeapon = TempEquipment.Instance.ScythingTalons };
            RendingClaws = new WeaponSet { Name = "Rending Claws", PrimaryMeleeWeapon = TempEquipment.Instance.RendingClaws, SecondaryMeleeWeapon = TempEquipment.Instance.RendingClaws };
        }

        public WeaponSet ScythingTalons { get; private set; }
        public WeaponSet Deathspitter { get; private set; }
        public WeaponSet Devourer { get; private set; }
        public WeaponSet RendingClaws { get; private set; }
    }

    public sealed class TempTyranidSquadTemplates
    {
        private static TempTyranidSquadTemplates _instance;
        public static TempTyranidSquadTemplates Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempTyranidSquadTemplates();
                }
                return _instance;
            }
        }

        private TempTyranidSquadTemplates()
        {
            CreateHormagauntSquad();
            CreateTermagauntSquad();
            CreateTyranidWarriorSquad();
        }

        public SquadTemplate HormagauntSquadTemplate { get; private set; }
        public SquadTemplate TermagauntSquadTemplate { get; private set; }
        public SquadTemplate TyranidWarriorSquadTemplate { get; private set; }


        private void CreateTyranidWarriorSquad()
        {
            TyranidWarriorSquadTemplate = new SquadTemplate(1012, "Tyranid Warrior Squad", TempTyranidWeaponSets.Instance.Deathspitter, null, TempEquipment.Instance.HeavyChitin);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
            TyranidWarriorSquadTemplate.Members.Add(TempTyranidWarriorTemplate.Instance);
        }
        private void CreateTermagauntSquad()
        {
            TermagauntSquadTemplate = new SquadTemplate(1011, "Termagaunt Squad", TempTyranidWeaponSets.Instance.Devourer, null, TempEquipment.Instance.LightChitin);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
            TermagauntSquadTemplate.Members.Add(TempTermagauntTemplate.Instance);
        }

        private void CreateHormagauntSquad()
        {
            HormagauntSquadTemplate = new SquadTemplate(1010, "Hormagaunt Squad", TempTyranidWeaponSets.Instance.ScythingTalons, null, TempEquipment.Instance.LightChitin);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
            HormagauntSquadTemplate.Members.Add(TempHormagauntTemplate.Instance);
        }
    }
}
