using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    // ----------------------
    //
    // Class for updating the UI in the level select, including displaying info about the current-selected level
    //
    // ----------------------

    [Header("UI Object References")]
    public TMP_Text LevelNameText;
    public TMP_Text LevelDescriptionText;
    public TMP_Text LevelRecordTimeText;
    public TMP_Text LevelRecordScoreText;

    private void Start()
    {
        DisableLevelDisplay();
    }

    public void EnableLevelDisplay(LevelData level)
    {
        if (LevelNameText != null)
        {
            LevelNameText.transform.parent.gameObject.SetActive(true);

            LevelNameText.text = level.LevelName;
            LevelDescriptionText.text = level.LevelDescription;
            LevelRecordTimeText.text =
                PlayerProgressTracker.GenerateReadableRecordTime(
                    PlayerProgressTracker.LoadProgress(level.LevelSceneID).RecordCompletionTimes[0]);
            LevelRecordScoreText.text = 
                PlayerProgressTracker.GenerateReadableRecordScore(
                    PlayerProgressTracker.LoadProgress(level.LevelSceneID).RecordCompletionScores[0]);
        }
    }

    public void DisableLevelDisplay()
    {
        if (LevelNameText != null)
        {
            LevelNameText.transform.parent.gameObject.SetActive(false);
        }
    }
}
