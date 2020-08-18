using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iam.Scripts.Helpers;
using Iam.Scripts.Models;

namespace Iam.Scripts.Controllers
{
    public class BattleController: MonoBehaviour
    {
        public void Test()
        {
            // we're going to start by assuming grid spaces are 10yd x 10yd and turns are 2 seconds. 
            // A carefully moving unencumbered Marine can cover 10yrds in 2 seconds, or 20 yards if running/charging
            BattleGrid grid = new BattleGrid(1, 80);
            // generate a Space Marine armed with a Boltgun in Power Armor on each side.
            BattleSquad forceBlue = GenerateSingleMarineSquad();
            BattleSquad forceRed = GenerateSingleMarineSquad();
            // Start them 800 yards apart
            grid.PlacePlayerSquad(forceBlue, 0, 0);
            grid.PlaceOpposingSquad(forceRed, 0, 79);
            // don't worry about unit AI yet... just have them each move one and fire
            TakeAction(forceBlue, true, grid);
            TakeAction(forceRed, false, grid);
        }

        public BattleSquad GenerateSingleMarineSquad()
        {
            SoldierFactory factory = new SoldierFactory();
            Soldier[] soldiers = new Soldier[1];
            soldiers[0] = factory.GenerateNewSoldier(1, "666 Test");
            soldiers[0].Equipment.Add(TempEquipment.Instance.Boltgun.GenerateInstance());
            soldiers[0].Equipment.Add(TempEquipment.Instance.PowerArmor.GenerateInstance());
            return new BattleSquad(soldiers);
        }

        private void TakeAction(BattleSquad squad, bool isPlayerSquad, BattleGrid grid)
        {
            KeyValuePair<Tuple<int, int>, BattleSquad> enemy;
            float range = grid.GetNearestEnemy(squad, true, out enemy);
            if (ShouldFire(squad, enemy.Value, range))
            {
                Shoot(squad, enemy.Value, range);
            }
            else
            {
                grid.MoveSquad(squad, true, 0, isPlayerSquad ? 1 : -1);
            }
        }

        private bool ShouldFire(BattleSquad squad, BattleSquad enemy, float range)
        {
            List<Weapon> bestWeapons = squad.GetWeaponsForRange(range);
            if(bestWeapons.Count == 0)
            {
                return false;
            }
            else
            {
                int enemyArmor = enemy.GetAverageArmor();
                return IsWorthShooting(bestWeapons, enemyArmor, range);
            }
        }

        private bool IsWorthShooting(List<Weapon> bestWeapons, int enemyArmor, float range)
        {
            foreach(Weapon weapon in bestWeapons)
            {
                int effectiveArmor = enemyArmor - weapon.Template.ArmorPiercing;
                RangeBand band = weapon.Template.RangeBands.GetRangeForDistance(range);
                // TODO: come up with a better prediction equation
                if (band.Strength * 6 > effectiveArmor) return true;
            }
            return false;
        }
    
        private void Shoot(BattleSquad squad, BattleSquad target, float range)
        {

        }
    }
}
