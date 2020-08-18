using Iam.Scripts.Models;
using System.Collections.Generic;
using System.Linq;

namespace Iam.Scripts.Helpers
{
    public class BattleSquad
    {
        public Soldier[] Squad { get; private set; }

        public BattleSquad(Soldier[] soldiers)
        {
            Squad = soldiers;
        }

        public List<Weapon> GetWeaponsForRange(float range)
        {
            List<Weapon> list = new List<Weapon>();
            foreach(Soldier soldier in Squad)
            {
                Weapon bestWeapon = null;
                int bestStrength = 0;
                int bestAccuracy = int.MinValue;
                foreach(Weapon weapon in soldier.Weapons)
                {
                    RangeBand band = weapon.Template.RangeBands.GetRangeForDistance(range);
                    if(band != null && (band.Strength > bestStrength || band.Strength == bestStrength && band.Accuracy > bestAccuracy))
                    {
                        bestWeapon = weapon;
                        bestStrength = band.Strength;
                        bestAccuracy = band.Accuracy;
                    }
                }
                if(bestWeapon != null)
                {
                    list.Add(bestWeapon);
                }
            }
            return list;
        }
    
        public int GetAverageArmor()
        {
            int runningTotal = 0;
            foreach(Soldier soldier in Squad)
            {
                if(soldier.Armor != null)
                {
                    runningTotal += soldier.Armor.Template.ArmorProvided;
                }
            }
            return runningTotal / Squad.Count();
        }
    }

    /*public class BattleForce
    {

    }*/
}
