using System;

using UnityEngine;

using OnlyWar.Models;
using OnlyWar.Helpers;

[Serializable]
[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Debug")]
    public bool debugMode;

    [Header("Galaxy Map")]
    public int SectorSize;
    public Vector2 MapScale;

    [Header("Battle Map")]
    public Vector2 BattleMapScale;

    [Header("Chapter Definitions")]
    [HideInInspector]
    public Sector Sector;
    [HideInInspector]
    public Force Chapter;

    [Header("Date")]
    public Date Date;

    //[Header("SharedData")]
    //public bool IsDialogShowing;
}