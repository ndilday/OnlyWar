﻿using System;
using System.Collections.Generic;

using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;

namespace Iam.Scripts.Helpers.Battle
{
    public class BattleSoldier
    {
        private ushort _enemiesTakenOut;
        public Soldier Soldier { get; private set; }

        public Tuple<int, int> Location { get; set; }
        public BattleSquad BattleSquad { get; private set; }

        public List<RangedWeapon> EquippedRangedWeapons { get; private set; }

        public List<MeleeWeapon> EquippedMeleeWeapons { get; private set; }

        public List<MeleeWeapon> MeleeWeapons { get; private set; }
        public List<RangedWeapon> RangedWeapons { get; private set; }
        public Armor Armor { get; set; }
        public bool IsInMelee { get; set; }
        public Stance Stance { get; set; }
        public float CurrentSpeed { get; set; }

        public float TurnsRunning { get; set; }
        public ushort TurnsShooting { get; set; }
        public ushort TurnsSwinging { get; set; } 
        public ushort TurnsAiming { get; set; }
        public uint WoundsTaken { get; set; }

        public ushort EnemiesTakenDown 
        { 
            get
            {
                return _enemiesTakenOut;
            }
        }
        public int HandsFree
        {
            get
            {
                int handCount = Soldier.FunctioningHands;
                foreach(RangedWeapon weapon in EquippedRangedWeapons)
                {
                    handCount -= GetHandsForWeapon(weapon.Template);
                }
                foreach(MeleeWeapon weapon in EquippedMeleeWeapons)
                {
                    handCount -= GetHandsForWeapon(weapon.Template);
                }
                return handCount;
            }
        }
        public Tuple<BattleSoldier, RangedWeapon, int> Aim { get; set; }

        public BattleSoldier(Soldier soldier, BattleSquad squad)
        {
            Soldier = soldier;
            BattleSquad = squad;
            MeleeWeapons = new List<MeleeWeapon>();
            RangedWeapons = new List<RangedWeapon>();
            EquippedMeleeWeapons = new List<MeleeWeapon>();
            EquippedRangedWeapons = new List<RangedWeapon>();
            Location = null;
            Aim = null;
            IsInMelee = false;
            Stance = Stance.Standing;
            CurrentSpeed = 0;
            _enemiesTakenOut = 0;
        }
        
        public void AddWeapons(List<RangedWeapon> rangedWeapons, List<MeleeWeapon> meleeWeapons)
        {
            if (rangedWeapons?.Count > 0)
            {
                RangedWeapons.AddRange(rangedWeapons);
            }
            if (meleeWeapons?.Count > 0)
            {
                MeleeWeapons.AddRange(meleeWeapons);
            }

            if (RangedWeapons.Count > 0)
            {
                if (RangedWeapons.Count == 1 )
                {
                    EquippedRangedWeapons.AddRange(RangedWeapons);
                }
                else if (RangedWeapons[0].Template.Location == EquipLocation.OneHand && RangedWeapons[1].Template.Location == EquipLocation.OneHand)
                {
                    EquippedRangedWeapons.Add(RangedWeapons[0]);
                    EquippedRangedWeapons.Add(RangedWeapons[1]);
                }
                else
                {
                    EquippedRangedWeapons.Add(RangedWeapons[0]);
                }
            }
            if (MeleeWeapons.Count > 0)
            {
                if (EquippedRangedWeapons.Count == 0)
                {
                    // we have two hands free for close combat weapons
                    if (MeleeWeapons.Count == 1)
                    {
                        EquippedMeleeWeapons.AddRange(MeleeWeapons);
                    }
                    else if (MeleeWeapons[0].Template.Location == EquipLocation.OneHand && MeleeWeapons[1].Template.Location == EquipLocation.OneHand)
                    {
                        EquippedMeleeWeapons.Add(MeleeWeapons[0]);
                        EquippedMeleeWeapons.Add(MeleeWeapons[1]);

                    }
                    else
                    {
                        EquippedMeleeWeapons.Add(MeleeWeapons[0]);
                    }
                }
                else if (EquippedRangedWeapons.Count == 1 && EquippedRangedWeapons[0].Template.Location == EquipLocation.OneHand)
                {
                    if(MeleeWeapons[0].Template.Location == EquipLocation.OneHand)
                    {
                        EquippedMeleeWeapons.Add(MeleeWeapons[0]);
                    }
                    else if(MeleeWeapons.Count > 1 && MeleeWeapons[1].Template.Location == EquipLocation.OneHand)
                    {
                        EquippedMeleeWeapons.Add(MeleeWeapons[1]);
                    }
                }
            }
        }

        public float GetMoveSpeed()
        {
            // TODO: if leg injuries, slow soldier down
            float baseMoveSpeed = Soldier.MoveSpeed;
            //soldier.Body.HitLocations.Where(hl => hl)
            return baseMoveSpeed;
        }

        public void AddKill(Faction faction, WeaponTemplate weapon)
        {
            _enemiesTakenOut++;
            Soldier.AddKill(faction, weapon);
        }

        public override string ToString()
        {
            return Soldier.ToString();
        }

        private int GetHandsForWeapon(WeaponTemplate template)
        {
            switch (template.Location)
            {
                case EquipLocation.OneHand:
                    return 1;
                case EquipLocation.TwoHand:
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
