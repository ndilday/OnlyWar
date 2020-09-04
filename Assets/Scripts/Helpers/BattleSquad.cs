using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Helpers
{
    public class BattleSquad
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Soldier[] Squad { get; private set; }
        public float CoverModifier { get; private set; }
        public bool IsPlayerSquad { get; private set; }

        public BattleSquad(int id, string name, bool isPlayerSquad, Squad squad)
        {
            Id = id;
            Name = name;
            Squad = squad.GetAllMembers();
            IsPlayerSquad = isPlayerSquad;
            // order weapon sets by strength of primary weapon
            AllocateEquipment(squad);
        }

        public Tuple<int, int> GetSquadBoxSize()
        {
            int numberOfRows = 1;
            if (Squad.Length >= 30)
            {
                numberOfRows = 3;
            }
            else if (Squad.Length > 7)
            {
                numberOfRows = 2;
            }
            // membersPerRow is how many soldiers are in each row (back row may be smaller)
            int membersPerRow = Mathf.CeilToInt((float)(Squad.Length) / (float)(numberOfRows));
            return new Tuple<int, int>(membersPerRow, numberOfRows);
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

        private void AllocateEquipment(Squad squad)
        {
            var tempSquad = Squad.ToList();
            var wsList = squad.Loadout.OrderByDescending(l => l.MainWeapon.ArmorPiercing).ThenBy(l => l.MainWeapon.PenetrationMultiplier).ThenBy(l => l.MainWeapon.Accuracy).ToList();
            // need to allocate weapons from squad weapon sets
            if (Squad[0] == squad.SquadLeader)
            {
                // for now, sgt always gets default weapons
                Squad[0].Weapons = squad.SquadTemplate.DefaultWeapons.GetWeapons();
                // TODO: personalize armor and weapons
                Squad[0].Armor = new Armor(squad.SquadTemplate.Armor);
                tempSquad.RemoveAt(0);
            }
            foreach (WeaponSet ws in wsList)
            {
                // TODO: we'll want to stop assuming Dex as the base stat at some point
                var bestShooter = tempSquad.OrderByDescending(s => s.Dexterity + s.Skills[ws.MainWeapon.RelatedSkill.Id].SkillBonus).First();
                bestShooter.Weapons = ws.GetWeapons();
                bestShooter.Armor = new Armor(squad.SquadTemplate.Armor);
                tempSquad.Remove(bestShooter);
            }
            if(tempSquad.Count() > 0)
            {
                Debug.Log("BattleSquad.AllocateEquipment: how did we get here?");
                foreach(Soldier soldier in tempSquad)
                {
                    soldier.Weapons = squad.SquadTemplate.DefaultWeapons.GetWeapons();
                    // TODO: personalize armor and weapons
                    soldier.Armor = new Armor(squad.SquadTemplate.Armor);
                }
            }
        }
    }
}
