using System;
using System.Collections.Generic;
using System.Linq;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers
{
    public class BattleSquad
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Soldier[] Squad { get; private set; }
        public float CoverModifier { get; private set; }

        public BattleSquad(int id, string name, Soldier[] soldiers)
        {
            Id = id;
            Name = name;
            Squad = soldiers;
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
                float bestStrength = 0;
                foreach(Weapon weapon in soldier.Weapons)
                {
                    if (!soldier.CanFireWeapon(weapon)) continue;
                    ChosenWeapon newWeapon = new ChosenWeapon(weapon, soldier);
                    float newStrength = newWeapon.GetStrengthAtRange(range);
                    if( bestWeapon == null || bestStrength < newStrength 
                        || (bestStrength == newStrength && newWeapon.ActiveWeapon.Template.Accuracy > bestWeapon.ActiveWeapon.Template.Accuracy))
                    {
                        bestWeapon = newWeapon;
                        bestStrength = newStrength;
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
            int squadSize = 0;
            foreach(Soldier soldier in Squad)
            {
                if(soldier != null && soldier.Armor != null)
                {
                    runningTotal += soldier.Armor.Template.ArmorProvided;
                    squadSize++;
                }
            }
            if (squadSize == 0) return 0;
            return runningTotal / squadSize;
        }
    
        public float GetAverageSize()
        {
            float squadSize = 0;
            float runningTotal = 0;
            foreach(Soldier soldier in Squad)
            {
                if(soldier != null)
                {
                    runningTotal += soldier.Size;
                    squadSize += 1.0f;
                }
            }
            return runningTotal / squadSize;
        }

        public float GetAverageConstitution()
        {
            float squadSize = 0;
            float runningTotal = 0;
            foreach (Soldier soldier in Squad)
            {
                if (soldier != null)
                {
                    runningTotal += soldier.Constitution;
                    squadSize += 1.0f;
                }
            }
            return runningTotal / squadSize;
        }

        public int GetSquadMove()
        {
            int runningTotal = int.MaxValue;
            foreach (Soldier soldier in Squad)
            {
                // TODO: take leg wounds into account
                if (soldier != null && soldier.MoveSpeed < runningTotal)
                {
                    runningTotal = (int)soldier.MoveSpeed;
                }
            }
            return runningTotal;
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
