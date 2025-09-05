using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "HalfConshuss/Level/Level Data")]
public class LevelData : ScriptableObject
{
    public int LevelSceneID;

    [Space(10)]
    [Header("Base Level Information")]
    public string LevelID;
    public string LevelName;
    public string LevelDescription;
    public bool IsLevelDefaultUnlocked;
    public LevelData NextLevel;

    [Space(10)]
    [Header("Score Bonus Times (in seconds)")]
    public int MinCompletionTime;
    public int MaxCompletionTime;
}
