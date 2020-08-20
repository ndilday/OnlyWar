using Iam.Scripts.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Iam.Scripts.Helpers
{
    public class BattleSquad
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Soldier[] Squad { get; private set; }
        // for now, assume all members of Squad are same size. Won't work as well for Orks
        public float Height { get; private set; }
        public float CoverModifier { get; private set; }

        public BattleSquad(int id, string name, Soldier[] soldiers, float height)
        {
            Id = id;
            Name = name;
            Squad = soldiers;
            Height = height;
        }

        public Soldier GetRandomSquadMember()
        {
            return Squad[UnityEngine.Random.Range(0, Squad.Length)];
        }

        public List<ChosenWeapon> GetWeaponsForRange(float range)
        {
            List<ChosenWeapon> list = new List<ChosenWeapon>();
            foreach(Soldier soldier in Squad)
            {
                ChosenWeapon bestWeapon = null;
                foreach(Weapon weapon in soldier.Weapons)
                {
                    RangeBand band = weapon.Template.RangeBands.GetRangeForDistance(range);
                    if(band != null && (bestWeapon == null || bestWeapon.ActiveRangeBand.Strength < band.Strength 
                        || (bestWeapon.ActiveRangeBand.Strength == band.Strength && band.Accuracy > bestWeapon.ActiveRangeBand.Accuracy)))
                    {
                        bestWeapon = new ChosenWeapon(band, weapon, soldier);
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
    
        public void RemoveSoldier(Soldier soldier)
        {
            Squad = Squad.Except(Squad.Where(s => s.Id == soldier.Id)).ToArray();
        }
    }

    /*public class BattleForce
    {

    }*/
}
