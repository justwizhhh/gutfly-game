using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class PlayerProgressTracker
{
    private static int LevelRecordTimeLimit = 3;

    private static string SavePath => Application.persistentDataPath + "/levelProgress.json";

    //  ---------------------------------------------
    //
    //  Level info processing functions
    //
    //  ---------------------------------------------

    // Generate a human-readable time record value, for level progress saving
    public static string GenerateReadableRecordTime(float levelTime)
    {
        float minutes = Mathf.FloorToInt(levelTime / 60f);
        float seconds = Mathf.FloorToInt(levelTime % 60f);
        float milliseconds = Mathf.FloorToInt((levelTime * 100f) % 100f);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    public static string GenerateReadableRecordScore(int levelScore)
    {
        return String.Format("{0:0000000}", levelScore);
    }

    // Generate a list of time records, sorted by lowest time, and restricted to a set list size
    private static List<float> GenerateRecordTimeList(List<float> currentTimeList)
    {
        // Remove temporary '0' time value
        int zeroIndex = currentTimeList.FindIndex(x => x == 0);
        if (zeroIndex != -1)
        {
            currentTimeList.RemoveAt(zeroIndex);
        }

        currentTimeList.Sort();
        if (currentTimeList.Count >= LevelRecordTimeLimit)
        {
            currentTimeList.RemoveAt(currentTimeList.Count - 1);
        }

        return currentTimeList;
    }

    // Generate a list of scores in order, like the function above
    private static List<int> GenerateRecordScoreList(List<int> currentScoreList)
    {
        // Remove temporary '0' time value
        int zeroIndex = currentScoreList.FindIndex(x => x == 0);
        if (zeroIndex != -1)
        {
            currentScoreList.RemoveAt(zeroIndex);
        }

        currentScoreList.Sort();
        if (currentScoreList.Count >= LevelRecordTimeLimit)
        {
            currentScoreList.RemoveAt(currentScoreList.Count - 1);
        }

        return currentScoreList;
    }

    //  ---------------------------------------------
    //
    //  Data saving logic
    //
    //  ---------------------------------------------

    // Access information inside of PlayerProgress
    public static PlayerProgress LoadPlayer()
    {
        if (File.Exists(SavePath))
        {
            string playerJson = File.ReadAllText(SavePath);
            return JsonConvert.DeserializeObject<PlayerProgress>(playerJson);
        }
        else
        {
            return new PlayerProgress();
        }
    }

    // Access the player's level progress info, inside of PlayerProgress
    public static LevelProgressData LoadProgress(int levelSceneID)
    {
        if (!File.Exists(SavePath))
        {
            return null;
        }

        PlayerProgress player = LoadPlayer();
        if (player == null || player.levelProgressStates == null)
        {
            return null;
        }

        return player.levelProgressStates.Find(x => x.LevelSceneID == levelSceneID);
    }

    // Add a new level, in its incomplete state, to PlayerProgress
    public static void AddProgress(int levelSceneID)
    {
        PlayerProgress player = LoadPlayer();

        LevelProgressData newLevelProgress = LoadProgress(levelSceneID);
        LevelProgressData progressData = new LevelProgressData
        {
            LevelSceneID = levelSceneID,
            IsCompleted = false,
            RecordCompletionTimes = new List<float> { 0f },
            RecordCompletionScores = new List<int> { 0 }
        };

        player.levelProgressStates.Add(progressData);

        string json = JsonConvert.SerializeObject(player);
        File.WriteAllText(SavePath, json);
    }

    // Save and update contents of PlayerProgress
    public static void SaveProgress(int levelSceneID, bool isLevelCompleted, float newLevelTime, int newLevelScore)
    {
        PlayerProgress player = LoadPlayer();
        
        LevelProgressData savedLevelProgress = LoadProgress(levelSceneID);
        if (savedLevelProgress == null)
        {
            // If no progress data for this level exists yet, add it
            LevelProgressData progressData = new LevelProgressData
            {
                LevelSceneID = levelSceneID,
                IsCompleted = isLevelCompleted,
                RecordCompletionTimes = new List<float> { newLevelTime },
                RecordCompletionScores = new List<int> { newLevelScore }
            };

            player.levelProgressStates.Add(progressData);
        }
        else
        {
            // If there is already progress data saved here, update it
            int index = player.levelProgressStates.FindIndex(x => x.LevelSceneID == savedLevelProgress.LevelSceneID);
            if (isLevelCompleted)
            {
                player.levelProgressStates[index].IsCompleted = true;
            }

            List<float> times = player.levelProgressStates[index].RecordCompletionTimes;
            times.Add(newLevelTime);
            player.levelProgressStates[index].RecordCompletionTimes = GenerateRecordTimeList(times);

            List<int> scores = player.levelProgressStates[index].RecordCompletionScores;
            scores.Add(newLevelScore);
            player.levelProgressStates[index].RecordCompletionScores = GenerateRecordScoreList(scores);
        }

        string json = JsonConvert.SerializeObject(player);
        File.WriteAllText(SavePath, json);
    }

    [ContextMenu("Clear All Level Progress")]
    public static void ClearAllProgress()
    {
        PlayerProgress player = LoadPlayer();
        player.levelProgressStates.Clear();

        string json = JsonConvert.SerializeObject(player);
        File.WriteAllText(SavePath, json);
    }
}
