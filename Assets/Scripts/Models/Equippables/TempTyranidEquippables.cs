using Iam.Scripts.Models.Soldiers;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Models.Equippables
{
    public class TempTyranidEquippables
    {
        private static TempTyranidEquippables _instance = null;
        public static TempTyranidEquippables Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TempTyranidEquippables();
                }
                return _instance;
            }
        }

        public IReadOnlyDictionary<int, RangedWeaponTemplate> RangedWeaponTemplates { get; }
        public IReadOnlyDictionary<int, MeleeWeaponTemplate> MeleeWeaponTemplates { get; }
        public IReadOnlyDictionary<int, ArmorTemplate> ArmorTemplates { get; }

        private TempTyranidEquippables()
        {
            RangedWeaponTemplates = new List<RangedWeaponTemplate>
            {
                new RangedWeaponTemplate(1, "Deathspitter", EquipLocation.TwoHand, TempBaseSkillList.Instance.OpponentRanged,
                                         0, 0.75f, 1.0f, 12.0f, 7.5f, 750.0f, 15, 100, 3, 3, true),
                new RangedWeaponTemplate(2, "Devourer", EquipLocation.TwoHand, TempBaseSkillList.Instance.OpponentRanged,
                                         0, 1f, 1.0f, 8.0f, 6f, 750.0f, 15, 100, 3, 2, true),
            }.ToDictionary(rwt => rwt.Id);

            MeleeWeaponTemplates = new List<MeleeWeaponTemplate>
            {
                new MeleeWeaponTemplate(101, "Scything Talons", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.OpponentMelee,
                                        1, 1.0f, 1.5f, 8f, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(102, "Rending Claws", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.OpponentMelee,
                                        0, 0.75f, 3f, 8f, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(103, "Fist", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.OpponentMelee,
                                        0, 1, 1, 3, 0.25f, 0, -1, 0),
                new MeleeWeaponTemplate(104, "Monsterous Rending Claws", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.OpponentMelee,
                                        0, 0.25f, 3f, 16f, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(105, "Monsterous Scything Talons", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.OpponentMelee,
                                        1, 0.25f, 3f, 16f, 0.25f, 0, 0, 0),
                new MeleeWeaponTemplate(107, "Prehensile Pincer Tail", EquipLocation.OneHand,
                                        TempBaseSkillList.Instance.OpponentMelee,
                                        0, 1f, 2f, 16f, 0.25f, 0, 0, 0)
            }.ToDictionary(mwt => mwt.Id);
            ArmorTemplates = new List<ArmorTemplate>
            {
                new ArmorTemplate(201, "Tyranid 5mm Chitin", 5),
                new ArmorTemplate(202, "Tyranid 10mm Chitin", 10),
                new ArmorTemplate(203, "Tyranid 15mm Chitin", 15),
                new ArmorTemplate(204, "Tyranid 20mm Chitin", 20)
            }.ToDictionary(at => at.Id);
        }
    }
}
