using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

using Iam.Scripts.Helpers.Battle;
using Iam.Scripts.Helpers.Battle.Actions;
using Iam.Scripts.Models;
using Iam.Scripts.Models.Equippables;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Units;
using Iam.Scripts.Views;
using Iam.Scripts.Helpers.Battle.Resolutions;

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
        private readonly Dictionary<int, BattleSoldier> _casualtyMap;

        private BattleSquad _selectedBattleSquad;
        private BattleGrid _grid;
        private int _turnNumber;
        private readonly MoveResolver _moveResolver;
        private readonly WoundResolver _woundResolver;

        private const int MAP_WIDTH = 100;
        private const int MAP_HEIGHT = 450;
        private const bool VERBOSE = false;

        public BattleController()
        {
            _playerSquads = new Dictionary<int, BattleSquad>();
            _opposingSquads = new Dictionary<int, BattleSquad>();;
            _soldierSquadMap = new Dictionary<int, BattleSquad>();
            _moveResolver = new MoveResolver();
            _moveResolver.OnRetreat.AddListener(MoveResolver_OnRetreat);
            _woundResolver = new WoundResolver(VERBOSE);
            _woundResolver.OnSoldierDeath.AddListener(WoundResolver_OnSoldierDeath);
            _woundResolver.OnSoldierFall.AddListener(WoundResolver_OnSoldierFall);
            _casualtyMap = new Dictionary<int, BattleSoldier>();
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

        public void NextStepButton_OnClick()
        {
            if (_playerSquads.Count() > 0 && _opposingSquads.Count() > 0)
            {
                _turnNumber++;
                BattleView.UpdateNextStepButton("Next Turn", false);
                BattleView.ClearBattleLog();
                _casualtyMap.Clear();
                Log(false, "Turn " + _turnNumber.ToString());
                // this is a three step process: plan, execute, and apply

                ConcurrentBag<IAction> actionBag = new ConcurrentBag<IAction>();
                ConcurrentQueue<string> log = new ConcurrentQueue<string>();
                Plan(actionBag, log);
                while(!log.IsEmpty)
                {
                    log.TryDequeue(out string line);
                    Log(false, line);
                }
                Execute(actionBag);

                while (!log.IsEmpty)
                {
                    log.TryDequeue(out string line);
                    Log(false, line);
                }

                Apply();

                RedrawSquadPositions();

                if (_playerSquads.Count() == 0 && _opposingSquads.Count() == 0)
                {
                    Log(false, "One side destroyed, battle over");
                    BattleView.UpdateNextStepButton("End Battle", true);
                }
                else
                {
                    BattleView.UpdateNextStepButton("Next Turn", true);
                }
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

        private void Plan(ConcurrentBag<IAction> actionBag, ConcurrentQueue<string> log)
        {
            // PLAN
            // use the thread pool to handle the BattleSquadPlanner classes;
            // these look at the current game state to figure out the actions each soldier should take
            // the planners populate the actionBag with what they want to do
            //Parallel.ForEach(_playerSquads.Values, (squad) =>
            foreach(BattleSquad squad in _playerSquads.Values)
            {
                BattleSquadPlanner planner = new BattleSquadPlanner(_grid, _soldierSquadMap, actionBag, _woundResolver.WoundQueue, _moveResolver.MoveQueue, log);
                planner.PrepareActions(squad);
            };
            //Parallel.ForEach(_opposingSquads.Values, (squad) =>
            foreach(BattleSquad squad in _opposingSquads.Values)
            {
                BattleSquadPlanner planner = new BattleSquadPlanner(_grid, _soldierSquadMap, actionBag, _woundResolver.WoundQueue, _moveResolver.MoveQueue, log);
                planner.PrepareActions(squad);
            };
        }

        private static void Execute(ConcurrentBag<IAction> actionBag)
        {
            // EXECUTE
            // once the squads have all finished planning actions, we use the thread pool to process the execution logic. 
            // These use the command pattern to allow the controller to execute each without having any knowledge of what the internal implementation is
            // this also allows us to separate the concerns of the planner and the executor
            // we take the results/side effects of each execution that impact the outside world and put those results into queues
            // (movement and wounding are the only things that fit this category, today, but there will be others in the future)
            //Parallel.ForEach(actionBag, (action) => action.Execute());
            foreach(IAction action in actionBag)
            {
                action.Execute();
            }
        }

        private void Apply()
        {
            // APPLY
            // the move resolver and wound resolver should now be populated
            // because movement and wounding may have race conditions, resolution has to be handled serially
            _moveResolver.Resolve();
            _woundResolver.Resolve();
            Log(false, _woundResolver.ResolutionLog);
            foreach (BattleSoldier soldier in _casualtyMap.Values)
            {
                RemoveSoldier(soldier, _soldierSquadMap[soldier.Soldier.Id]);
            }
        }

        private void RedrawSquadPositions()
        {
            foreach(BattleSquad squad in _playerSquads.Values)
            {
                var corners = _grid.GetSoldierBottomLeftAndSize(squad.Soldiers);
                BattleView.MoveSquad(squad.Id, new Vector2(corners.Item1.Item1, corners.Item1.Item2), new Vector2(corners.Item2.Item1, corners.Item2.Item2));
            }
            foreach(BattleSquad squad in _opposingSquads.Values)
            {
                var corners = _grid.GetSoldierBottomLeftAndSize(squad.Soldiers);
                BattleView.MoveSquad(squad.Id, new Vector2(corners.Item1.Item1, corners.Item1.Item2), new Vector2(corners.Item2.Item1, corners.Item2.Item2));
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
                report += soldier.Soldier.ToString() + "\n";
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

        private void WoundResolver_OnSoldierDeath(BattleSoldier soldier)
        {
            _casualtyMap[soldier.Soldier.Id] = soldier;
        }

        private void WoundResolver_OnSoldierFall(BattleSoldier soldier)
        {
            _casualtyMap[soldier.Soldier.Id] = soldier;
        }

        private void MoveResolver_OnRetreat(BattleSoldier soldier)
        {
            Log(false, "<b>" + soldier.Soldier.ToString() + " has retreated from the battlefield</b>");
            _casualtyMap[soldier.Soldier.Id] = soldier;
        }

        private void RemoveSoldier(BattleSoldier soldier, BattleSquad squad)
        {
            squad.RemoveSoldier(soldier);
            _grid.RemoveSoldier(soldier.Soldier.Id);
            _soldierSquadMap.Remove(soldier.Soldier.Id);
            if(squad.Soldiers.Count == 0)
            {
                RemoveSquad(squad);
            }
        }

        private void RemoveSquad(BattleSquad squad)
        {
            Log(false, "<b>" + squad.Name + " wiped out</b>");
            BattleView.RemoveSquad(squad.Id);
            
            if(squad.IsPlayerSquad)
            {
                _playerSquads.Remove(squad.Id);
            }
            else
            {
                _opposingSquads.Remove(squad.Id);
            }

            if(_selectedBattleSquad == squad)
            {
                _selectedBattleSquad = null;
            }
        }
    }
}
