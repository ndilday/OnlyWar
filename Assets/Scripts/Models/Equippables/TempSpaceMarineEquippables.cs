using Iam.Scripts.Models.Soldiers;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Equippables
{
    class TempSpaceMarineEquippables
    {
        private static TempSpaceMarineEquippables _instance;
        public static TempSpaceMarineEquippables Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TempSpaceMarineEquippables();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; }
        public IReadOnlyDictionary<int, ArmorTemplate> ArmorTemplates { get; }

        private TempSpaceMarineEquippables()
        {
            RangedWeaponTemplates = CreateRangedWeaponTemplates().ToDictionary(rwt => rwt.Id);
            MeleeWeaponTemplates = CreateMeleeWeaponTemplates().ToDictionary(mwt => mwt.Id);
            ArmorTemplates = new List<ArmorTemplate>
            {
                new ArmorTemplate(200, "Astartes Power Armor Mk VII", 20),
                new ArmorTemplate(201, "Carapace Armor", 15)
            }.ToDictionary(template => template.Id);
        }

        private List<RangedWeaponTemplate> CreateRangedWeaponTemplates()
        {
            return new List<RangedWeaponTemplate>
            {
                new RangedWeaponTemplate(0, "Boltgun", EquipLocation.TwoHand, TempBaseSkillList.Instance.Bolter,
                                       3, 1, 2.0f, 10.0f, 6.0f, 1000.0f, 9, 30, 2, 4, false),
                new RangedWeaponTemplate(1, "Bolt Pistol", EquipLocation.OneHand, TempBaseSkillList.Instance.Bolter,
                                       1, 1, 2.0f, 10.0f, 6.0f, 500.0f, 3, 10, 3, 2, false),
                new RangedWeaponTemplate(2, "Flamer", EquipLocation.TwoHand, TempBaseSkillList.Instance.Flamer,
                                       6, 1, 1.0f, 5.0f, 6.0f, 30.0f, 1, 50, 1, 3, false),
                new RangedWeaponTemplate(3, "Plasma Gun", EquipLocation.TwoHand, TempBaseSkillList.Instance.Plasma,
                                       2, 0.25f, 1.0f, 9.0f, 10.5f, 1000.0f, 3, 30, 2, 3, true),
                new RangedWeaponTemplate(4, "Meltagun", EquipLocation.TwoHand, TempBaseSkillList.Instance.Plasma,
                                       2, 0.2f, 4.0f, 10.0f, 12.0f, 200.0f, 3, 20, 2, 4, true),
                new RangedWeaponTemplate(5, "Heavy Bolter", EquipLocation.TwoHand, TempBaseSkillList.Instance.Bolter,
                                       0, 0.75f, 2.0f, 16.0f, 7.5f, 1600.0f, 15, 150, 2, 8, false),
                new RangedWeaponTemplate(6, "Lascannon", EquipLocation.TwoHand, TempBaseSkillList.Instance.Lascannon,
                                       10, 0.25f, 1.0f, 15.0f, 13.5f, 2000.0f, 1, 10, 1, 8, true),
                new RangedWeaponTemplate(7, "MissileLauncher", EquipLocation.TwoHand, TempBaseSkillList.Instance.MissileLauncher,
                                       3, 0.5f, 3.0f, 12.0f, 12.0f, 2000.0f, 1, 10, 1, 8, false),
                // TODO: handle weird damage falloff of multi-melta (works out to wound x of 4.467 on average); for now, just using 4
                new RangedWeaponTemplate(8, "Multi-melta", EquipLocation.TwoHand, TempBaseSkillList.Instance.Plasma,
                                       3, 0.2f, 4.0f, 10.0f, 12.0f, 1000.0f, 1, 10, 1, 6, true),
                new RangedWeaponTemplate(9, "Plasma Cannon", EquipLocation.TwoHand, TempBaseSkillList.Instance.Plasma,
                                       6, 0.25f, 1.0f, 15.0f, 10.5f, 1500.0f, 10, 30, 3, 6, true),
                new RangedWeaponTemplate(10, "Plasma Pistol", EquipLocation.OneHand, TempBaseSkillList.Instance.Plasma,
                                       2, 0.25f, 1.0f, 6.0f, 10.5f, 500.0f, 3, 40, 2, 2, true),
                new RangedWeaponTemplate(11, "Sniper Rifle", EquipLocation.TwoHand, TempBaseSkillList.Instance.Sniper,
                                       9, 0.5f, 1.0f, 10.0f, 6.0f, 1600.0f, 1, 20, 1, 8, true),
                new RangedWeaponTemplate(12, "Astartes Shotgun", EquipLocation.TwoHand, TempBaseSkillList.Instance.Shotgun,
                                       3, 1.0f, 2.0f, 11.0f, 6.0f, 100.0f, 2, 8, 3, 4, true)
            };
        }
    
        private List<MeleeWeaponTemplate> CreateMeleeWeaponTemplates()
        {
            return new List<MeleeWeaponTemplate>
            {
                new MeleeWeaponTemplate(101, "Chainsword", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Sword,
                                        0, 1.0f, 1.5f, 3.0f, 0.25f, 0, 0, 1),
                new MeleeWeaponTemplate(102, "Chainfist", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Fist,
                                        -1, 0.75f, 3f, 3.0f, 0.5f, 0, -1, 0),
                new MeleeWeaponTemplate(103, "Eviscerator", EquipLocation.TwoHand,
                                        TempBaseSkillList.Instance.Sword,
                                        -1, 0.2f, 3f, 12f, 0.5f, 0, -1, 0),
                new MeleeWeaponTemplate(104, "Crozius Arcanum", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Axe,
                                        0, 0.75f, 3f, 12, 0.25f, 1, 0, 0),
                new MeleeWeaponTemplate(105, "Force Axe", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Axe,
                                        0, 0.5f, 3f, 11, 0.25f, 1, -1, 0),
                new MeleeWeaponTemplate(106, "Force Sword", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Sword,
                                        0, 0.25f, 3f, 11, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(107, "Power Axe", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Axe,
                                        0, 0.5f, 1.5f, 11, 0.25f, 1, -1, 0),
                new MeleeWeaponTemplate(108, "Power Sword", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Sword,
                                        0, 0.25f, 1.5f, 10, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(109, "Power Fist", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Fist,
                                        -1, 0.25f, 3f, 3, 0.5f, 0, -1, 0),
                new MeleeWeaponTemplate(110, "Servo-arm", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.ServoArm,
                                        -1, 0.5f, 4.5f, 5, 0.5f, 0, -1, 0),
                new MeleeWeaponTemplate(111, "Master-crafted Power Sword", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Sword,
                                        0, 0.25f, 3f, 10, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(112, "Thunder Hammer", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Axe,
                                        -1, 0.25f, 4.5f, 12, 0.5f, 0, -1, 0),
                new MeleeWeaponTemplate(113, "Fist", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.Fist,
                                        0, 1f, 1f, 3, 0.25f, 0, -1, 0)

            };
        }
    }
}
