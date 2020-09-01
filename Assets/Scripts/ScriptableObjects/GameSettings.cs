using UnityEngine;
using System;
using Iam.Scripts.Models.Units;

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
    public UnitTemplate ChapterTemplate = TempUnitTemplates.Instance.ChapterTemplate;
    public Unit Chapter;
    public int ChapterPlanetId;

    [Header("Date")]
    public int Year;
    public int Week;
}