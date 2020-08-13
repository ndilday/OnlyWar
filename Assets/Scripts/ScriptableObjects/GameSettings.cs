using UnityEngine;
using System;
using Iam.Scripts.Models;

[Serializable]
[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    [Header("Debug")]
    public bool debugMode;

    [Header("Map Scale")]
    public Vector2 MapScale;

    [Header("Chapter Definitions")]
    public UnitTemplate ChapterTemplate = TempChapterOrganization.Instance.Chapter;
}