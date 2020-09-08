using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

using Iam.Scripts.Helpers;
using Iam.Scripts.Helpers.Battle;
using Iam.Scripts.Helpers.Battle.Actions;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;
using Unity.Collections.LowLevel.Unsafe;

namespace Iam.Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        public UnityEvent OnBattleComplete;
        
        [SerializeField]
        private BattleView BattleView;
        
        private readonly Dictionary<int, BattleSquad> _playerSquads;
        private readonly Dictionary<int, BattleSquad> _opposingSquads;
        private readonly Dictionary<int, BattleSquad> _soldierSquadMap;

        private BattleSquad _selectedBattleSquad;
        private BattleGrid _grid;
        private int _turnNumber;

        private const int MAP_WIDTH = 100;
        private const int MAP_HEIGHT = 450;
        private const bool VERBOSE = false;
        private const int GRID_SCALE = 1;


        public BattleController()
        {
            _playerSquads = new Dictionary<int, BattleSquad>();
            _opposingSquads = new Dictionary<int, BattleSquad>();;
            _soldierSquadMap = new Dictionary<int, BattleSquad>();
        }

        public void GalaxyController_OnBattleStarted(Planet planet)
        {
            ResetValues();

            foreach(KeyValuePair<int, List<Unit>> kvp in planet.FactionGroundUnitListMap)
            {
                if(kvp.Key == TempFactions.Instance.SpaceMarines.Id)
                {
                    PopulateMapsFromUnitList(_playerSquads, kvp.Value, true);
                }
                else
                {
                    PopulateMapsFromUnitList(_opposingSquads, kvp.Value, false);
                }
            }

            BattleSquadPlacer placer = new BattleSquadPlacer(_grid);
            var playerPlacements = placer.PlaceSquads(_playerSquads.Values);
            PopulateBattleViewSquads(playerPlacements);
            var oppPlacements = placer.PlaceSquads(_opposingSquads.Values);
            PopulateBattleViewSquads(oppPlacements);
            BattleView.UpdateNextStepButton("Next Turn", true);
        }

        public void RunBattleTurn()
        {
            // don't worry about unit AI yet... just have them each move one and fire
            // TODO: handle initiative
            if (_playerSquads.Count() > 0 && _opposingSquads.Count() > 0)
            {
                _turnNumber++;
                BattleView.ClearBattleLog();
                Log(false, "Turn " + _turnNumber.ToString());
                ConcurrentBag<IAction> actionBag = new ConcurrentBag<IAction>();
                Parallel.ForEach(_playerSquads.Values, (squad) =>
                {
                    BattleSquadPlanner planner = new BattleSquadPlanner(_grid, _soldierSquadMap, actionBag);
                    planner.PrepareActions(squad);
                });
                Parallel.ForEach(_opposingSquads.Values, (squad) =>
                {
                    BattleSquadPlanner planner = new BattleSquadPlanner(_grid, _soldierSquadMap, actionBag);
                    planner.PrepareActions(squad);
                });

                // execute


                // apply
                
                if (_playerSquads.Count() == 0 && _opposingSquads.Count() == 0)
                {
                    Log(false, "One side destroyed, battle over");
                    BattleView.UpdateNextStepButton("End Battle", true);
                    // update View button
                }

                //UpdateInjuryTrackers();
            }
            else
            {
                Debug.Log("Battle completed");
                BattleView.gameObject.SetActive(false);
                OnBattleComplete.Invoke();
            }
        }

        public void BattleView_OnSquadSelected(int squadId)
        {
            if (_playerSquads.ContainsKey(squadId))
            {
                _selectedBattleSquad = _playerSquads[squadId];
                BattleView.OverwritePlayerWoundTrack(GetSquadDetails(_selectedBattleSquad));
            }
            else
            {
                _selectedBattleSquad = _opposingSquads[squadId];
                BattleView.OverwritePlayerWoundTrack(GetSquadSummary(_selectedBattleSquad));
            }
        }

        private void ResetValues()
        {
            BattleView.gameObject.SetActive(true);
            BattleView.Clear();
            BattleView.SetMapSize(new Vector2(MAP_WIDTH, MAP_HEIGHT));
            _turnNumber = 0;

            _playerSquads.Clear();
            _soldierSquadMap.Clear();
            _opposingSquads.Clear();

            _grid = new BattleGrid(MAP_WIDTH, MAP_HEIGHT);
        }

        private void PopulateMapsFromUnitList(Dictionary<int, BattleSquad> map, List<Unit> units, bool isPlayerSquad)
        {
            foreach (Unit unit in units)
            {
                var battleSquads = unit.GetAllSquads().Where(s => !s.IsInReserve).Select(s => new BattleSquad(isPlayerSquad, s));
                foreach (BattleSquad bs in battleSquads)
                {
                    map[bs.Id] = bs;
                    foreach(BattleSoldier soldier in bs.Soldiers)
                    {
                        _soldierSquadMap[soldier.Soldier.Id] = bs;
                    }
                }
            }
        }

        private void PopulateBattleViewSquads(Dictionary<BattleSquad, Vector2> squadLocationMap)
        {
            foreach(KeyValuePair<BattleSquad, Vector2> kvp in squadLocationMap)
            {
                Tuple<int, int> size = kvp.Key.GetSquadBoxSize();
                Color color = kvp.Key.IsPlayerSquad ? Color.blue : Color.black;
                BattleView.AddSquad(kvp.Key.Id, kvp.Key.Name, kvp.Value, new Vector2(size.Item1, size.Item2), color);
            }
        }

        private string GetSquadDetails(BattleSquad squad)
        {
            string report = "\n" + squad.Name + "\n" + squad.Soldiers.Count.ToString() + " soldiers standing\n\n";
            foreach(BattleSoldier soldier in squad.Soldiers)
            {
                report += soldier.ToString() + "\n";
                foreach (RangedWeapon weapon in soldier.RangedWeapons)
                {
                    report += weapon.Template.Name + "\n";
                }
                report += soldier.Armor.Template.Name + "\n";
                foreach(HitLocation hl in soldier.Soldier.Body.HitLocations)
                {
                    report += hl.ToString() + "\n";
                }
                report += "\n";
            }
            return report;
        }

        private string GetSquadSummary(BattleSquad squad)
        {
            return "\n" + squad.Name + "\n" + squad.Soldiers.Count.ToString() + " soldiers standing\n\n";
        }

        private void Log(bool isMessageVerbose, string text)
        {
            if (VERBOSE || !isMessageVerbose)
            {
                BattleView.LogToBattleLog(text);
            }
        }

        private void TakeAction(BattleSquad squad)
        {
            // for now, if anyone in the squad starts shooting, the squad shoots
            float range = _grid.GetNearestEnemy(squad.Soldiers[0].Soldier, out int enemyId) * GRID_SCALE;
            List<ChosenRangedWeapon> bestWeapons = squad.GetWeaponsForRange(range);
            BattleSquad enemySquad =  _soldierSquadMap[enemyId];
            if(ShouldFire(bestWeapons, enemySquad, range))
            {
                Log(false, squad.Name + "is shooting");
                Shoot(bestWeapons, enemySquad, range, 0f);
            }
            else
            {
                Log(false, squad.Name + " is advancing");
                MoveSquad(squad);
            }
        }

        private void MoveSquad(BattleSquad squad)
        {
            int moveAmount = squad.GetSquadMove() / GRID_SCALE;
            int yMove = squad.IsPlayerSquad ? moveAmount : -moveAmount;
            foreach(BattleSoldier soldier in squad.Soldiers)
            {
                _grid.MoveSoldier(soldier.Soldier.Id, 0, yMove);
            }
            var location = _grid.GetSoldierBoxCorners(squad.Soldiers);
            BattleView.MoveSquad(squad.Id, new Vector2(location.Item1.Item1, location.Item1.Item2));
        }

        private bool ShouldFire(List<ChosenRangedWeapon> weapons, BattleSquad enemy, float range)
        {
            if(weapons.Count < 1)
            {
                return false;
            }
            else
            {
                return IsWorthShooting(weapons, enemy, range);
            }
        }

        private bool IsWorthShooting(List<ChosenRangedWeapon> bestWeapons, BattleSquad enemy, float range)
        {
            foreach(ChosenRangedWeapon weapon in bestWeapons)
            {
                float effectiveArmor = enemy.GetAverageArmor() * weapon.ActiveWeapon.Template.ArmorMultiplier;
                float effectiveStrength = weapon.GetStrengthAtRange(range);
                float effectiveWound = enemy.GetAverageConstitution() / 10;
                //float shootingSkill = 
                // TODO: come up with a better prediction equation
                bool canWound = effectiveStrength * 5 > effectiveArmor + effectiveWound;
                if(canWound)
                {
                    if (weapon.GetAccuracyAtRange(range) > 6)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    
        private void Shoot(List<ChosenRangedWeapon> weapons, BattleSquad target, float range, float coverModifier)
        {
            // figure out to-hit modifiers
            // distance + speed
            float rangeModifier = CalculateRangeModifier(range); // add target speed to 
            // size modifier is built into the target squad
            // weapon accuracy
            // cover
            foreach(ChosenRangedWeapon weapon in weapons)
            {
                // a previous shot may have finished off this squad; if so, other shots from this squad are wasted
                if (target.Soldiers.Count == 0) break;

                BattleSoldier hitSoldier = target.GetRandomSquadMember();
                float totalModifier = weapon.ActiveWeapon.Template.Accuracy + rangeModifier + CalculateSizeModifier(hitSoldier.Soldier.Size) + coverModifier;
                Log(true, "Total modifier to shot is " + totalModifier.ToString("F0"));
                // bracing
                // figure out number of shots fired
                RangedWeaponTemplate template = (RangedWeaponTemplate)weapon.ActiveWeapon.Template;
                for(int i = 0; i < template.RateOfFire; i++)
                {
                    Skill soldierSkill = weapon.Soldier.Skills[weapon.ActiveWeapon.Template.RelatedSkill.Id];

                    float skillTotal = soldierSkill.SkillBonus + GetStatForSkill(weapon.Soldier, soldierSkill);
                    float roll = 10.5f + (3.0f * (float)Gaussian.NextGaussianDouble());
                    float marginOfSuccess = skillTotal + totalModifier - roll;
                    Log(true, "Modified roll total is " + marginOfSuccess.ToString("F0"));
                    if(marginOfSuccess > 0)
                    {
                        // a hit!
                        ResolveHit(weapon, hitSoldier, target, range);
                    }
                }
            }
        }

        private float GetStatForSkill(Soldier soldier, Skill skill)
        {
            switch(skill.BaseSkill.BaseAttribute)
            {
                case SkillAttribute.Dexterity:
                    return soldier.Dexterity;
                case SkillAttribute.Intelligence:
                    return soldier.Intelligence;
                case SkillAttribute.Ego:
                    return soldier.Ego;
                case SkillAttribute.Presence:
                    return soldier.Presence;
                default:
                    return soldier.Dexterity;
            }
        }

        private void ResolveHit(ChosenRangedWeapon weapon, BattleSoldier hitSoldier, BattleSquad target, float range)
        {
            // TODO: handle hit location
            HitLocation location = DetermineHitLocation(hitSoldier.Soldier);
            Log(false, "<b>" + hitSoldier.ToString() + " hit in " + location.Template.Name + "</b>");
            if ((short)location.Wounds >= (short)location.Template.WoundLimit * 2)
            {
                Log(true, location.Template.Name + " already shot off");
            }
            else
            {
                // TODO: should hit margin of success affect damage? If so, how much?
                float damage = weapon.GetStrengthAtRange(range) * (3.5f + ((float)Gaussian.NextGaussianDouble() * 1.75f));
                Log(true, damage.ToString("F0") + " damage rolled");
                float effectiveArmor = hitSoldier.Armor.Template.ArmorProvided * weapon.ActiveWeapon.Template.ArmorMultiplier;
                if (effectiveArmor < 0) effectiveArmor = 0;
                float penDamage = damage - effectiveArmor;
                if (penDamage > 0)
                {
                    float totalDamage = penDamage * weapon.ActiveWeapon.Template.PenetrationMultiplier;
                    HandleWound(totalDamage, hitSoldier, location, target);
                }
            }
        }

        private HitLocation DetermineHitLocation(Soldier soldier)
        {
            // we're using the "lottery ball" approach to randomness here, where each point of probability
            // for each available body party defines the size of the random linear distribution
            // TODO: factor in cover/body position
            // 
            int roll = UnityEngine.Random.Range(1, soldier.Body.TotalProbability);
            foreach(HitLocation location in soldier.Body.HitLocations)
            {
                if(roll < location.Template.HitProbability)
                {
                    return location;
                }
                else
                {
                    // this is basically an easy iterative way to figure out which body part on the "chart" the roll matches
                    roll -= location.Template.HitProbability;
                }
            }
            // this should never happen
            return null;
        }

        private void HandleWound(float totalDamage, BattleSoldier hitSoldier, HitLocation location, BattleSquad soldierSquad)
        {
            Wounds wound;
            // check location for natural armor
            totalDamage -= location.Template.NaturalArmor;
            // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
            // multiply damage by location modifier
            totalDamage *= location.Template.DamageMultiplier;
            // compare total damage to soldier Constitution
            float ratio = totalDamage / hitSoldier.Soldier.Constitution;
            if(ratio >= 2.0f)
            {
                wound = Wounds.Unsurvivable;
            }
            else if(ratio >= 1.0f)
            {
                wound = Wounds.Critical;
            }
            else if(ratio >= 0.67f)
            {
                wound = Wounds.Severe;
            }
            else if(ratio >= 0.5f)
            {
                wound = Wounds.Serious;
            }
            else if(ratio >= 0.33f)
            {
                wound = Wounds.Major;
            }
            else if(ratio >= 0.2f)
            {
                wound = Wounds.Moderate;
            }
            else if(ratio >= 0.1f)
            {
                wound = Wounds.Minor;
            }
            else
            {
                wound = Wounds.Negligible;
            }
            Log(true, "The hit causes a " + wound.ToFriendlyString() + " wound");
            location.Wounds = (byte)location.Wounds + wound;
            if((short)location.Wounds >= (short)location.Template.WoundLimit * 2)
            {
                Log(false, "<b>" + location.Template.Name + " is blown off</b>");
                location.Wounds = (Wounds)((short)location.Template.WoundLimit * 2);
                if((short)wound > (short)location.Template.WoundLimit * 2)
                {
                    wound = (Wounds)((short)location.Template.WoundLimit * 2);
                }
            }
            else if ((short)location.Wounds >= (short)location.Template.WoundLimit)
            {
                Log(false, "<b>" + location.Template.Name + " is crippled</b>");
            }

            if (location.Template.Name == "Left Foot" || location.Template.Name == "Right Foot" 
                || location.Template.Name == "Left Leg" || location.Template.Name == "Right Leg")
            {
                if(location.Wounds >= location.Template.WoundLimit)
                {
                    Log(false, "<b>" + hitSoldier.ToString() + " has fallen and can't get up</b>");
                    RemoveSoldier(hitSoldier, soldierSquad);
                }
            }
            if (location.Wounds >= Wounds.Critical)
            {
                // TODO: need to start making consciousness checks
            }
            if (location.Wounds >= Wounds.Unsurvivable)
            {
                // make additional death check
                Log(false, "<b>" + hitSoldier.ToString() + " died</b>");
                RemoveSoldier(hitSoldier, soldierSquad);
            }
            else if (wound >= Wounds.Critical)
            {
                // make death check
                CheckForDeath(hitSoldier, soldierSquad);
            }
            
        }

        private void CheckForDeath(BattleSoldier soldier, BattleSquad squad)
        {
            float roll = 10.5f + (3.0f * (float)Gaussian.NextGaussianDouble());
            if (roll > soldier.Soldier.Constitution)
            {
                Log(false, soldier.ToString() + " died");
                RemoveSoldier(soldier, squad);
            }
        }

        private void RemoveSoldier(BattleSoldier soldier, BattleSquad squad)
        {
            squad.RemoveSoldier(soldier);
            _grid.RemoveSoldier(soldier.Soldier.Id);
            if(squad.Soldiers.Count == 0)
            {
                Log(false, "<b>" + squad.Name + " wiped out</b>");
                RemoveSquad(squad);
            }
        }

        private void RemoveSquad(BattleSquad squad)
        {
            BattleView.RemoveSquad(squad.Id);
             _opposingSquads.Remove(squad.Id);
            _soldierSquadMap.Remove(squad.Id);
            if(_selectedBattleSquad == squad)
            {
                _selectedBattleSquad = null;
            }
        }

        private float CalculateRangeModifier(float range)
        {
            return 2.4663f * (float)Math.Log(2 / range);
        }

        private float CalculateSizeModifier(float size)
        {
            // this is just the opposite of the range modifier
            return -CalculateRangeModifier(size);
        }
    }
}
