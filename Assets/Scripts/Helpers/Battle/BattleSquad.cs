using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;

namespace Iam.Scripts.Helpers.Battle
{
    public class BattleSquad
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<BattleSoldier> Soldiers { get; private set; }
        public float CoverModifier { get; private set; }
        public bool IsPlayerSquad { get; private set; }
        public bool IsInMelee { get; set; }

        private readonly Squad _squad;

        public BattleSquad(bool isPlayerSquad, Squad squad)
        {
            Id = squad.Id;
            Name = squad.Name;
            _squad = squad;
            Soldiers = squad.GetAllMembers().Select(s => new BattleSoldier(s, this)).ToList();
            IsPlayerSquad = isPlayerSquad;
            IsInMelee = false;
            // order weapon sets by strength of primary weapon
            AllocateEquipment();
        }

        public Tuple<int, int> GetSquadBoxSize()
        {
            int numberOfRows = 1;
            if (Soldiers.Count >= 30)
            {
                numberOfRows = 3;
            }
            else if (Soldiers.Count > 7)
            {
                numberOfRows = 2;
            }
            // membersPerRow is how many soldiers are in each row (back row may be smaller)
            int membersPerRow = Mathf.CeilToInt((float)(Soldiers.Count) / (float)(numberOfRows));
            return new Tuple<int, int>(membersPerRow, numberOfRows);
        }

        public BattleSoldier GetRandomSquadMember()
        {
            return Soldiers[RNG.GetIntBelowMax(0, Soldiers.Count)];
        }

        public List<ChosenRangedWeapon> GetWeaponsForRange(float range)
        {
            List<ChosenRangedWeapon> list = new List<ChosenRangedWeapon>();
            foreach(BattleSoldier soldier in Soldiers)
            {
                ChosenRangedWeapon bestWeapon = null;
                float bestStrength = 0;
                foreach(RangedWeapon weapon in soldier.RangedWeapons)
                {
                    if (soldier.Soldier.FunctioningHands == 0) continue;
                    ChosenRangedWeapon newWeapon = new ChosenRangedWeapon(weapon, soldier.Soldier);
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
    
        public float GetAverageArmor()
        {
            int runningTotal = 0;
            int squadSize = 0;
            foreach(BattleSoldier soldier in Soldiers)
            {
                if(soldier.Armor != null)
                {
                    runningTotal += soldier.Armor.Template.ArmorProvided;
                    squadSize++;
                }
            }
            if (squadSize == 0) return 0;
            return (float)runningTotal / (float)squadSize;
        }
    
        public float GetAverageSize()
        {
            float squadSize = 0;
            float runningTotal = 0;
            foreach(BattleSoldier soldier in Soldiers)
            {
                runningTotal += soldier.Soldier.Size;
                squadSize += 1.0f;
            }
            return runningTotal / squadSize;
        }

        public float GetAverageConstitution()
        {
            float squadSize = 0;
            float runningTotal = 0;
            foreach (BattleSoldier soldier in Soldiers)
            {
                runningTotal += soldier.Soldier.Constitution;
                squadSize += 1.0f;
            }
            return runningTotal / squadSize;
        }

        public float GetSquadMove()
        {
            float runningTotal = float.MaxValue;
            foreach (BattleSoldier soldier in Soldiers)
            {
                // TODO: take leg wounds into account
                if (soldier.GetMoveSpeed() < runningTotal)
                {
                    runningTotal = soldier.Soldier.MoveSpeed;
                }
            }
            return runningTotal;
        }

        public void RemoveSoldier(BattleSoldier soldier)
        {
            Soldiers.Remove(soldier);
        }

        public override string ToString()
        {
            return _squad.Name;
        }

        private void AllocateEquipment()
        {
            List<BattleSoldier> tempSquad = new List<BattleSoldier>(Soldiers);
            var wsList = _squad.Loadout.ToList();
            // need to allocate weapons from squad weapon sets
            if (Soldiers[0].Soldier == _squad.SquadLeader)
            {
                // for now, sgt always gets default weapons
                Soldiers[0].AddWeapons(_squad.SquadTemplate.DefaultWeapons.GetRangedWeapons(), _squad.SquadTemplate.DefaultWeapons.GetMeleeWeapons());
                // TODO: personalize armor and weapons
                Soldiers[0].Armor = new Armor(_squad.SquadTemplate.Armor);
                tempSquad.RemoveAt(0);
            }
            foreach (WeaponSet ws in wsList)
            {
                // TODO: we'll want to stop assuming Dex as the base stat at some point
                if (ws.PrimaryRangedWeapon != null)
                {
                    var bestShooter = tempSquad.OrderByDescending(s => s.Soldier.GetTotalSkillValue(ws.PrimaryRangedWeapon.RelatedSkill)).First();
                    bestShooter.AddWeapons(ws.GetRangedWeapons(), ws.GetMeleeWeapons());
                    bestShooter.Armor = new Armor(_squad.SquadTemplate.Armor);
                    tempSquad.Remove(bestShooter);
                }
                else
                {
                    var bestHitter = tempSquad.OrderByDescending(s => s.Soldier.GetTotalSkillValue(ws.PrimaryMeleeWeapon.RelatedSkill)).First();
                    bestHitter.AddWeapons(ws.GetRangedWeapons(), ws.GetMeleeWeapons());
                    bestHitter.Armor = new Armor(_squad.SquadTemplate.Armor);
                    tempSquad.Remove(bestHitter);
                }
            }
            if(tempSquad.Count() > 0)
            {
                foreach(BattleSoldier soldier in tempSquad)
                {
                    soldier.AddWeapons(_squad.SquadTemplate.DefaultWeapons.GetRangedWeapons(), _squad.SquadTemplate.DefaultWeapons.GetMeleeWeapons());
                    // TODO: personalize armor and weapons
                    soldier.Armor = new Armor(_squad.SquadTemplate.Armor);
                }
            }
        }
    }
}
