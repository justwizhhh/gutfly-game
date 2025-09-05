using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelProgressData
{
    public int LevelSceneID;

    public bool IsCompleted;
    public List<float> RecordCompletionTimes = new List<float>();
    public List<int> RecordCompletionScores = new List<int>();
}
