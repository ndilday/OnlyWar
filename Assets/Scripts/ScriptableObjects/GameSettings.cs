using System;
using System.Collections.Generic;

using UnityEngine;

using Iam.Scripts.Models;
using Iam.Scripts.Models.Soldiers;
using Iam.Scripts.Models.Squads;
using Iam.Scripts.Models.Factions;

[Serializable]
[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Debug")]
    public bool debugMode;

    [Header("Galaxy Map")]
    public int GalaxySize;
    public Vector2 MapScale;

    [Header("Battle Map")]
    public Vector2 BattleMapScale;

    [Header("Chapter Definitions")]
    public Faction PlayerFaction;
    public IReadOnlyList<Faction> OpposingFactions;
    public Chapter Chapter;
    [HideInInspector]
    public Dictionary<int, PlayerSoldier> PlayerSoldierMap;
    [HideInInspector]
    public Dictionary<int, Squad> SquadMap;
    public int ChapterPlanetId;

    [Header("Date")]
    public Date Date;

    //[Header("SharedData")]
    //public bool IsDialogShowing;
}