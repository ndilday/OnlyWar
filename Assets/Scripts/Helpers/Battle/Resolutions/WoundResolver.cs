using System.Collections.Concurrent;

using UnityEngine.Events;

using Iam.Scripts.Models.Soldiers;

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
            Wounds wound;
            // check location for natural armor
            totalDamage -= location.Template.NaturalArmor;
            // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
            // multiply damage by location modifier
            totalDamage *= location.Template.DamageMultiplier;
            // compare total damage to soldier Constitution
            float ratio = totalDamage / hitSoldier.Soldier.Constitution;
            if (ratio >= 2.0f)
            {
                wound = Wounds.Unsurvivable;
            }
            else if (ratio >= 1.0f)
            {
                wound = Wounds.Critical;
            }
            else if (ratio >= 0.67f)
            {
                wound = Wounds.Severe;
            }
            else if (ratio >= 0.5f)
            {
                wound = Wounds.Serious;
            }
            else if (ratio >= 0.33f)
            {
                wound = Wounds.Major;
            }
            else if (ratio >= 0.2f)
            {
                wound = Wounds.Moderate;
            }
            else if (ratio >= 0.1f)
            {
                wound = Wounds.Minor;
            }
            else
            {
                wound = Wounds.Negligible;
            }
            //Log(true, wound.ToFriendlyString() + " wound");
            location.Wounds = (byte)location.Wounds + wound;
            if ((short)location.Wounds >= (short)location.Template.WoundLimit * 2)
            {
                //Log(false, "<b>" + _ + " " + location.Template.Name + " is blown off</b>");
                location.Wounds = (Wounds)((short)location.Template.WoundLimit * 2);
                if ((short)wound > (short)location.Template.WoundLimit * 2)
                {
                    wound = (Wounds)((short)location.Template.WoundLimit * 2);
                }
            }
            else if ((short)location.Wounds >= (short)location.Template.WoundLimit)
            {
                // TODO: if arm or hand, handle unequipping a weapon
                //Log(false, "<b>" + location.Template.Name + " is crippled</b>");
            }

            if (location.Template.Name == "Left Foot" || location.Template.Name == "Right Foot"
                || location.Template.Name == "Left Leg" || location.Template.Name == "Right Leg")
            {
                if (location.Wounds >= location.Template.WoundLimit)
                {
                    Log(false, "<b>" + hitSoldier.Soldier.ToString() + " has fallen and can't get up</b>");
                    OnSoldierFall.Invoke(hitSoldier);
                }
            }
            if (location.Wounds >= Wounds.Critical)
            {
                // TODO: need to start making consciousness checks
            }
            if (location.Wounds >= Wounds.Unsurvivable)
            {
                // make additional death check
                OnSoldierDeath.Invoke(hitSoldier);
                Log(false, "<b>" + hitSoldier.Soldier.ToString() + " died</b>");
            }
            else if (wound >= Wounds.Critical)
            {
                // make death check
                CheckForDeath(hitSoldier);
            }

        }

        private void CheckForDeath(BattleSoldier soldier)
        {
            float roll = 10.5f + (3.0f * (float)Random.NextGaussianDouble());
            if (roll > soldier.Soldier.Constitution)
            {
                Log(false, "<b>" + soldier.Soldier.ToString() + " died</b>");
                OnSoldierDeath.Invoke(soldier);
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
