using System.Collections.Concurrent;

using UnityEngine.Events;

using OnlyWar.Models.Soldiers;
using OnlyWar.Models.Equippables;

namespace OnlyWar.Helpers.Battles.Resolutions
{
    public class WoundResolver : IResolver
    {
        public UnityEvent<BattleSoldier, BattleSoldier, WeaponTemplate> OnSoldierDeath;
        public UnityEvent<BattleSoldier, BattleSoldier, WeaponTemplate> OnSoldierFall;

        private readonly bool _allowVerbose;
        public string ResolutionLog { get; private set; }
        public ConcurrentBag<WoundResolution> WoundQueue { get; private set; }

        public WoundResolver(bool allowVerbose)
        {
            WoundQueue = new ConcurrentBag<WoundResolution>();
            _allowVerbose = allowVerbose;
            ResolutionLog = "";
            OnSoldierDeath = new UnityEvent<BattleSoldier, BattleSoldier, WeaponTemplate>();
            OnSoldierFall = new UnityEvent<BattleSoldier, BattleSoldier, WeaponTemplate>();
        }

        public void Resolve()
        {
            ResolutionLog = "";
            while(!WoundQueue.IsEmpty)
            {
                WoundQueue.TryTake(out WoundResolution wound);
                HandleWound(wound);
            }
        }

        private void HandleWound(WoundResolution wound)
        {
            if (wound.HitLocation.IsSevered)
            {
                // this shouldn't happen, as we check elsewhere
                Log(true, wound.HitLocation.Template.Name + " was previously severed");
            }
            else
            {
                float totalDamage = wound.Damage;
                WoundLevel woundLevel;
                // check wound.HitLocation for natural armor
                totalDamage -= wound.HitLocation.Template.NaturalArmor;
                // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
                // multiply damage by wound.HitLocation modifier
                totalDamage *= wound.HitLocation.Template.WoundMultiplier;
                // compare total damage to soldier Constitution
                float ratio = totalDamage / wound.Suffererer.Soldier.Constitution;
                if (ratio >= 8.0f)
                {
                    woundLevel = WoundLevel.Unsurvivable;
                }
                else if (ratio >= 4.0f)
                {
                    woundLevel = WoundLevel.Mortal;
                }
                else if (ratio >= 2f)
                {
                    woundLevel = WoundLevel.Massive;
                }
                else if (ratio >= 1f)
                {
                    woundLevel = WoundLevel.Critical;
                }
                else if (ratio >= 0.5f)
                {
                    woundLevel = WoundLevel.Major;
                }
                else if (ratio >= 0.25f)
                {
                    woundLevel = WoundLevel.Moderate;
                }
                else if (ratio >= 0.125f)
                {
                    woundLevel = WoundLevel.Minor;
                }
                else
                {
                    woundLevel = WoundLevel.Negligible;
                }
                wound.HitLocation.Wounds.AddWound(woundLevel);

                // see if wound.HitLocation is now severed
                if (wound.HitLocation.IsSevered || wound.HitLocation.IsCrippled)
                {
                    // if severed, see if it's an arm or leg
                    if (wound.HitLocation.Template.IsMotive)
                    {
                        Log(false, "<b>" + wound.Suffererer.Soldier.Name + " can no longer walk</b>");
                        OnSoldierFall.Invoke(wound.Suffererer, wound.Inflicter, wound.Weapon);
                    }
                    else if(wound.HitLocation.Template.IsRangedWeaponHolder)
                    {
                        if(wound.Suffererer.EquippedRangedWeapons.Count > 0 && wound.Suffererer.EquippedRangedWeapons[0].Template.Location == EquipLocation.OneHand)
                        {
                            wound.Suffererer.EquippedRangedWeapons.RemoveAt(0);
                        }
                    }
                    else if(wound.HitLocation.Template.IsMeleeWeaponHolder)
                    {
                        if (wound.Suffererer.EquippedMeleeWeapons.Count > 0 && wound.Suffererer.EquippedMeleeWeapons[0].Template.Location == EquipLocation.OneHand)
                        {
                            wound.Suffererer.EquippedMeleeWeapons.RemoveAt(0);
                        }
                    }
                    if(wound.HitLocation.Template.IsVital && wound.HitLocation.IsCrippled)
                    {
                        Log(false, "<b>" + wound.Suffererer.Soldier.Name + " has succumbed to their wounds</b>");
                        OnSoldierFall.Invoke(wound.Suffererer, wound.Inflicter, wound.Weapon);
                    }
                }
            }
        }

        private void Log(bool isVerboseMessage, string text)
        {
            if (!isVerboseMessage || _allowVerbose)
            {
                ResolutionLog += text + "\n";
            }
        }
    }
}
