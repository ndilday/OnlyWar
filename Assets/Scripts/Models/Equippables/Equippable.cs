using OnlyWar.Models.Soldiers;
using System; 

namespace OnlyWar.Models.Equippables
{
    public enum EquipLocation
    {
        Body = 0,
        OneHand = 1,
        TwoHand = 2
    }

    public class EquippableTemplate
    {
        public int Id { get; }
        public string Name { get; }
        public EquipLocation Location { get; }
        public EquippableTemplate(int id, string name, EquipLocation location)
        {
            Id = id;
            Name = name;
            Location = location;
        }
    }

    public class ArmorTemplate : EquippableTemplate
    {
        public byte ArmorProvided { get; }
        public short StealthModifier { get; }
        public ArmorTemplate(int id, string name, byte armorProvided, short stealthModifier) : base(id, name, EquipLocation.Body)
        {
            ArmorProvided = armorProvided;
            StealthModifier = stealthModifier;
        }
    }

    public class WeaponTemplate : EquippableTemplate
    {
        public BaseSkill RelatedSkill { get; }
        public float Accuracy { get; }
        public float ArmorMultiplier { get; }
        public float WoundMultiplier { get; }
        public float RequiredStrength { get; }
        public WeaponTemplate(int id, string name, EquipLocation location,
                              BaseSkill skill, float accuracy, 
                              float armorMultiplier, float penetrationMultiplier,
                              float requiredStrength) : base(id, name, location)
        {
            RelatedSkill = skill;
            Accuracy = accuracy;
            ArmorMultiplier = armorMultiplier;
            WoundMultiplier = penetrationMultiplier;
            RequiredStrength = requiredStrength;
        }
    }

    public class RangedWeaponTemplate: WeaponTemplate
    {
        public float DamageMultiplier { get; }
        public float MaximumRange { get; }
        public byte RateOfFire { get; }
        public ushort AmmoCapacity { get; }
        public ushort Recoil { get; }
        public ushort Bulk { get; }
        public bool DoesDamageDegradeWithRange { get; }

        public ushort ReloadTime { get; }
        public RangedWeaponTemplate(int id, string name, EquipLocation location,
                              BaseSkill skill, float accuracy,
                              float armorMultiplier, float penetrationMultiplier,
                              float requiredStrength, float baseDamage,
                              float maxDistance, byte rof, ushort ammo,
                              ushort recoil, ushort bulk, bool doesDamageDegradeWithRange, ushort reloadTime)
                              : base(id, name, location, skill, accuracy, armorMultiplier, 
                                     penetrationMultiplier, requiredStrength)
        {
            DamageMultiplier = baseDamage;
            MaximumRange = maxDistance;
            RateOfFire = rof;
            AmmoCapacity = ammo;
            Recoil = recoil;
            Bulk = bulk;
            DoesDamageDegradeWithRange = doesDamageDegradeWithRange;
            ReloadTime = reloadTime;
        }
    }

    public class MeleeWeaponTemplate: WeaponTemplate
    {
        public float StrengthMultiplier { get; }
        public float ExtraDamage { get; }
        public float ParryModifier { get; }
        //public float Reach;
        public float ExtraAttacks { get; }
        public MeleeWeaponTemplate(int id, string name, EquipLocation location,
                              BaseSkill skill, float accuracy,
                              float armorMultiplier, float penetrationMultiplier,
                              float requiredStrength, float strengthMultiplier,
                              float extraDamage, float parryMod, float extraAttacks)
                              : base(id, name, location, skill, accuracy, armorMultiplier, 
                                     penetrationMultiplier, requiredStrength)
        {
            StrengthMultiplier = strengthMultiplier;
            ExtraDamage = extraDamage;
            ParryModifier = parryMod;
            ExtraAttacks = extraAttacks;
        }
    }

    public class Equippable
    {
        public EquippableTemplate Template { get; private set; }
        public Equippable(EquippableTemplate template) { Template = template; }
    }

    public class Armor
    {
        public ArmorTemplate Template { get; private set; }
        public Armor(ArmorTemplate template) { Template = template; }
    }

    public class RangedWeapon
    {
        public RangedWeaponTemplate Template { get; private set; }
        public ushort LoadedAmmo { get; set; }
        public RangedWeapon(RangedWeaponTemplate template) 
        { 
            Template = template;
            LoadedAmmo = template.AmmoCapacity;
        }

        public override string ToString()
        {
            return Template.Name;
        }
    }

    public class MeleeWeapon
    {
        public MeleeWeaponTemplate Template { get; private set; }
        public MeleeWeapon(MeleeWeaponTemplate template) { Template = template; }
        public override string ToString()
        {
            return Template.Name;
        }
    }
}