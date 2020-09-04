using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Iam.Scripts.Helpers;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;

namespace Iam.Scripts.Controllers
{
    public class BattleController : MonoBehaviour
    {
        public UnityEvent OnBattleComplete;
        public BattleView BattleView;
        
        private readonly Dictionary<int, BattleSquad> _playerSquads;
        private readonly Dictionary<int, BattleSquad> _opposingSquads;
        private readonly Dictionary<int, BattleSquad> _playerSoldierSquadMap;
        private readonly Dictionary<int, BattleSquad> _opposingSoldierSquadMap;
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
            _opposingSquads = new Dictionary<int, BattleSquad>();
            _playerSoldierSquadMap = new Dictionary<int, BattleSquad>();
            _opposingSoldierSquadMap = new Dictionary<int, BattleSquad>();
        }

        public void GalaxyController_OnBattleStarted(Planet planet)
        {
            BattleView.gameObject.SetActive(true);
            BattleView.Clear();
            BattleView.SetMapSize(new Vector2(MAP_WIDTH, MAP_HEIGHT));
            _turnNumber = 0;
            _grid = new BattleGrid(MAP_WIDTH, MAP_HEIGHT);
            // assume, for now, that space marines will be one of the two factions
            int currentBottom = 0;
            int currentLeft = MAP_WIDTH / 2;
            int currentTop = 0;
            int currentRight = MAP_WIDTH / 2;
            foreach (Unit unit in planet.FactionGroundUnitListMap[TempFactions.Instance.SpaceMarines.Id])
            {
                // the four coordinates represent the smallest possible box that contains only the current row of units
                PlacePlayerUnitSquads(unit, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }
            currentBottom = MAP_HEIGHT - 5;
            currentTop = MAP_HEIGHT - 5;
            currentLeft = MAP_WIDTH / 2;
            currentRight = MAP_WIDTH / 2;
            var oppUnitList = planet.FactionGroundUnitListMap.First(kvp => kvp.Key != TempFactions.Instance.SpaceMarines.Id).Value;
            foreach (Unit unit in oppUnitList)
            {
                // the four coordinates represent the smallest possible box that contains only the current row of units
                PlaceOpponentUnitSquads(unit, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }
            BattleView.NextStepButton.SetActive(true);
            BattleView.NextStepButton.GetComponentInChildren<Text>().text = "Next Turn";
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
                foreach (BattleSquad squad in _playerSquads.Values)
                {
                    if (_opposingSquads.Count() > 0)
                    {
                        TakeAction(squad);
                    }
                }
                foreach(BattleSquad squad in _opposingSquads.Values)
                {
                    if (_playerSquads.Count() > 0)
                    {
                        TakeAction(squad);
                    }
                }
                BattleView.OverwritePlayerWoundTrack(_selectedBattleSquad == null ? "" : GetSquadSummary(_selectedBattleSquad));
                if (_playerSquads.Count() == 0 && _opposingSquads.Count() == 0)
                {
                    Log(false, "One side destroyed, battle over");
                    BattleView.NextStepButton.GetComponentInChildren<Text>().text = "End Battle";
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

        private void PlacePlayerUnitSquads(Unit unit, ref int currentLeft, ref int currentBottom, ref int currentRight, ref int currentTop)
        {
            // start in bottom left 
            if (!unit.HQSquad.IsInReserve)
            {
                PlacePlayerSquad(unit.HQSquad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }
            foreach (Squad squad in unit.Squads)
            {
                if (!squad.IsInReserve)
                {
                    PlacePlayerSquad(squad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
                }
            }
            foreach(Unit childUnit in unit.ChildUnits)
            {
                PlacePlayerUnitSquads(childUnit, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }
        }

        private void PlacePlayerSquad(Squad squad, ref int left, ref int bottom, ref int right, ref int top)
        {
            BattleSquad bs = new BattleSquad(squad.Id, squad.Name, true, squad);
            _playerSquads[squad.Id] = bs;
            foreach(Soldier soldier in bs.Squad)
            {
                _playerSoldierSquadMap[soldier.Id] = bs;
            }
            Tuple<int, int> squadSize = bs.GetSquadBoxSize();

            // determine if there's more space to the left or right of the current limits
            int spaceRight = MAP_WIDTH - right;
            int placeLeft, placeBottom;
            if(squadSize.Item1 > spaceRight && squadSize.Item1 > left)
            {
                // there's not enough room; move "up"
                bottom = top + 2;
                top += squadSize.Item2 + 4;
                left = (MAP_WIDTH / 2) - 2;
                right = left + squadSize.Item1 + 2;

                placeLeft = left;
                placeBottom = bottom;
            }
            else if(spaceRight > left)
            {
                // place to the right of the current box
                placeLeft = right + 2;
                placeBottom = bottom + 2;
                right += squadSize.Item1 + 5;
                if (top < bottom + squadSize.Item2 + 2) top = bottom + squadSize.Item2 + 2;
            }
            else
            {
                // place to the left of the current box
                left -= squadSize.Item1 + 2;
                placeLeft = left;
                placeBottom = bottom + 2;
                if (top < bottom + squadSize.Item2 + 2) top = bottom + squadSize.Item2 + 2;
            }


            _grid.PlaceSquad(bs, placeLeft, placeBottom);
            BattleView.AddSquad(squad.Id, squad.Name, new Vector2(placeLeft, placeBottom), new Vector2(squadSize.Item1, squadSize.Item2), Color.blue);
        }

        private void PlaceOpponentUnitSquads(Unit unit, ref int currentLeft, ref int currentBottom, ref int currentRight, ref int currentTop)
        {
            // start in bottom left 
            if (unit.HQSquad != null && !unit.HQSquad.IsInReserve)
            {
                PlaceOpponentSquad(unit.HQSquad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }
            foreach (Squad squad in unit.Squads)
            {
                if (!squad.IsInReserve)
                {
                    PlaceOpponentSquad(squad, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
                }
            }
            foreach (Unit childUnit in unit.ChildUnits)
            {
                PlaceOpponentUnitSquads(childUnit, ref currentLeft, ref currentBottom, ref currentRight, ref currentTop);
            }
        }

        private void PlaceOpponentSquad(Squad squad, ref int left, ref int bottom, ref int right, ref int top)
        {
            BattleSquad bs = new BattleSquad(squad.Id, squad.Name, false, squad);
            _opposingSquads[squad.Id] = bs;
            foreach (Soldier soldier in bs.Squad)
            {
                _opposingSoldierSquadMap[soldier.Id] = bs;
            }
            Tuple<int, int> squadSize = bs.GetSquadBoxSize();

            // determine if there's more space to the left or right of the current limits
            int spaceRight = MAP_WIDTH - right;
            int placeLeft, placeBottom;
            if (squadSize.Item1 > spaceRight && squadSize.Item1 > left)
            {
                // there's not enough room; move "up"
                top = bottom;
                bottom += squadSize.Item2;
                left = MAP_WIDTH / 2;
                right = left + squadSize.Item1;

                placeLeft = left;
                placeBottom = bottom;
            }
            else if (spaceRight > left)
            {
                // place to the right of the current box
                placeLeft = right;
                placeBottom = top - squadSize.Item2;
                right += squadSize.Item1;
                if (bottom > top - squadSize.Item2) bottom = top - squadSize.Item2;
            }
            else
            {
                // place to the left of the current box
                left -= squadSize.Item1;
                placeLeft = left;
                placeBottom = top - squadSize.Item2;
                if (top < bottom + squadSize.Item2) bottom = top - squadSize.Item2;
            }

            _grid.PlaceSquad(bs, placeLeft, placeBottom);
            BattleView.AddSquad(squad.Id, squad.Name, new Vector2(placeLeft, placeBottom), new Vector2(squadSize.Item1, squadSize.Item2), Color.black);
        }

        private string GetSquadDetails(BattleSquad squad)
        {
            string report = "\n" + squad.Name + "\n" + squad.Squad.Length.ToString() + " soldiers standing\n\n";
            foreach(Soldier soldier in squad.Squad)
            {
                report += soldier.ToString() + "\n";
                foreach (Weapon weapon in soldier.Weapons)
                {
                    report += weapon.Template.Name + "\n";
                }
                report += soldier.Armor.Template.Name + "\n";
                foreach(HitLocation hl in soldier.Body.HitLocations)
                {
                    report += hl.ToString() + "\n";
                }
                report += "\n";
            }
            return report;
        }

        private string GetSquadSummary(BattleSquad squad)
        {
            return "\n" + squad.Name + "\n" + squad.Squad.Length.ToString() + " soldiers standing\n\n";
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
            float range = _grid.GetNearestEnemy(squad.Squad[0], squad.IsPlayerSquad, out int enemyId) * GRID_SCALE;
            List<ChosenWeapon> bestWeapons = squad.GetWeaponsForRange(range);
            BattleSquad enemySquad = squad.IsPlayerSquad ? _opposingSoldierSquadMap[enemyId] : _playerSoldierSquadMap[enemyId];
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
            int moveAmount = squad.GetSquadMove() * 3 / GRID_SCALE;
            Tuple<int, int> newPosition = _grid.MoveSquad(squad, 0, squad.IsPlayerSquad ? moveAmount : -moveAmount);
            BattleView.MoveSquad(squad.Id, new Vector2(newPosition.Item1, newPosition.Item2));
        }

        private bool ShouldFire(List<ChosenWeapon> weapons, BattleSquad enemy, float range)
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

        private bool IsWorthShooting(List<ChosenWeapon> bestWeapons, BattleSquad enemy, float range)
        {
            foreach(ChosenWeapon weapon in bestWeapons)
            {
                int effectiveArmor = enemy.GetAverageArmor() - weapon.ActiveWeapon.Template.ArmorPiercing;
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
    
        private void Shoot(List<ChosenWeapon> weapons, BattleSquad target, float range, float coverModifier)
        {
            // figure out to-hit modifiers
            // distance + speed
            float rangeModifier = CalculateRangeModifier(range); // add target speed to 
            // size modifier is built into the target squad
            // weapon accuracy
            // cover
            foreach(ChosenWeapon weapon in weapons)
            {
                // a previous shot may have finished off this squad; if so, other shots from this squad are wasted
                if (target.Squad.Length == 0) break;

                Soldier hitSoldier = target.GetRandomSquadMember();
                float totalModifier = weapon.ActiveWeapon.Template.Accuracy + rangeModifier + CalculateSizeModifier(hitSoldier.Size) + coverModifier;
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

        private void ResolveHit(ChosenWeapon weapon, Soldier hitSoldier, BattleSquad target, float range)
        {
            // TODO: handle hit location
            HitLocation location = DetermineHitLocation(hitSoldier);
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
                float effectiveArmor = hitSoldier.Armor.Template.ArmorProvided - weapon.ActiveWeapon.Template.ArmorPiercing;
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

        private void HandleWound(float totalDamage, Soldier hitSoldier, HitLocation location, BattleSquad soldierSquad)
        {
            Wounds wound;
            // check location for natural armor
            totalDamage -= location.Template.NaturalArmor;
            // for now, natural armor reducing the damange below 0 will still cause a Negligible injury
            // multiply damage by location modifier
            totalDamage *= location.Template.DamageMultiplier;
            // compare total damage to soldier Constitution
            float ratio = totalDamage / hitSoldier.Constitution;
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

        private void CheckForDeath(Soldier soldier, BattleSquad squad)
        {
            float roll = 10.5f + (3.0f * (float)Gaussian.NextGaussianDouble());
            if (roll > soldier.Constitution)
            {
                Log(false, soldier.ToString() + " died");
                RemoveSoldier(soldier, squad);
            }
        }

        private void RemoveSoldier(Soldier soldier, BattleSquad squad)
        {
            squad.RemoveSoldier(soldier);
            _grid.RemoveSoldier(soldier.Id, squad.IsPlayerSquad);
            if(squad.Squad.Length == 0)
            {
                Log(false, "<b>" + squad.Name + " wiped out</b>");
                RemoveSquad(squad);
            }
        }

        private void RemoveSquad(BattleSquad squad)
        {
            BattleView.RemoveSquad(squad.Id);
            if (squad.IsPlayerSquad)
            {
                _playerSoldierSquadMap.Remove(squad.Id);
                _playerSquads.Remove(squad.Id);
            }
            else
            {
                _opposingSquads.Remove(squad.Id);
                _opposingSoldierSquadMap.Remove(squad.Id);
            }
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
