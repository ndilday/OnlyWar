using System.Collections.Concurrent;

using UnityEngine.Events;

using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Equippables;

namespace Iam.Scripts.Helpers.Battle.Resolutions
{
    public class WoundResolver : IResolver
    {
        public UnityEvent<BattleSoldier> OnSoldierDeath;
        public UnityEvent<BattleSoldier> OnSoldierFall;

        private readonly bool _allowVerbose;
        public string ResolutionLog { get; private set; }
        public ConcurrentBag<WoundResolution> WoundQueue { get; private set; }

        public WoundResolver(bool allowVerbose)
        {
            WoundQueue = new ConcurrentBag<WoundResolution>();
            _allowVerbose = allowVerbose;
            ResolutionLog = "";
            OnSoldierDeath = new UnityEvent<BattleSoldier>();
            OnSoldierFall = new UnityEvent<BattleSoldier>();
        }

        public void Resolve()
        {
            ResolutionLog = "";
            while(!WoundQueue.IsEmpty)
            {
                WoundQueue.TryTake(out WoundResolution wound);
                HandleWound(wound.Damage, wound.Soldier, wound.HitLocation);
            }
        }

        private void HandleWound(float totalDamage, BattleSoldier hitSoldier, HitLocation location)
        {
            if (location.IsSevered)
            {
                // this shouldn't happen, as we check elsewhere
                Log(true, location.Template.Name + " was previously severed");
            }
            else
            {
                WoundLevel wound;
                // check location for natural armor
                totalDamage -= location.Template.NaturalArmor;
                // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
                // multiply damage by location modifier
                totalDamage *= location.Template.DamageMultiplier;
                // compare total damage to soldier Constitution
                float ratio = totalDamage / hitSoldier.Soldier.Constitution;
                if (ratio >= 8.0f)
                {
                    wound = WoundLevel.Unsurvivable;
                }
                else if (ratio >= 4.0f)
                {
                    wound = WoundLevel.Mortal;
                }
                else if (ratio >= 2f)
                {
                    wound = WoundLevel.Massive;
                }
                else if (ratio >= 1f)
                {
                    wound = WoundLevel.Critical;
                }
                else if (ratio >= 0.5f)
                {
                    wound = WoundLevel.Major;
                }
                else if (ratio >= 0.25f)
                {
                    wound = WoundLevel.Moderate;
                }
                else if (ratio >= 0.125f)
                {
                    wound = WoundLevel.Minor;
                }
                else
                {
                    wound = WoundLevel.Negligible;
                }
                location.Wounds.AddWound(wound);

                // see if location is now severed
                if (location.IsSevered || location.IsCrippled)
                {
                    // if severed, see if it's an arm or leg
                    if (location.Template.IsMotive)
                    {
                        Log(false, "<b>" + hitSoldier.Soldier.ToString() + " can no longer walk</b>");
                        OnSoldierFall.Invoke(hitSoldier);
                    }
                    else if(location.Template.IsRangedWeaponHolder)
                    {
                        if(hitSoldier.EquippedRangedWeapons.Count > 0 && hitSoldier.EquippedRangedWeapons[0].Template.Location == EquipLocation.OneHand)
                        {
                            hitSoldier.EquippedRangedWeapons.RemoveAt(0);
                        }
                    }
                    else if(location.Template.IsMeleeWeaponHolder)
                    {
                        if (hitSoldier.EquippedMeleeWeapons.Count > 0 && hitSoldier.EquippedMeleeWeapons[0].Template.Location == EquipLocation.OneHand)
                        {
                            hitSoldier.EquippedMeleeWeapons.RemoveAt(0);
                        }
                    }
                    if(location.Template.IsVital && location.IsSevered)
                    {
                        Log(false, "<b>" + hitSoldier.Soldier.ToString() + " has succumbed to their wounds</b>");
                        OnSoldierFall.Invoke(hitSoldier);
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
