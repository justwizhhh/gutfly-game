using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButton : MonoBehaviour, IMenuButton
{
    // ----------------------
    //
    // This object is used to select an individual level in the level select menu
    //
    // ----------------------

    public bool IsActive;
    public LevelData LevelData;

    [Space(10)]
    public Color DeactivatedColor;

    // Component/object references
    private SpriteRenderer sr;
    private LevelSelectMenu levelSelect;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        levelSelect = FindFirstObjectByType<LevelSelectMenu>();
    }

    private void OnEnable()
    {
        // Only make the level accessible if it has been tracked by the player's progress (inside of levelProgress.json)
        LevelProgressData levelProgress = PlayerProgressTracker.LoadProgress(LevelData.LevelSceneID);

        if (levelProgress != null)
        {
            ActivateButton();

            // Unlock all other levels that have been listed inside of LevelInfo
            if (levelProgress.IsCompleted && LevelData.NextLevel != null)
            {
                LevelProgressData unlockedLevelProgress = PlayerProgressTracker.LoadProgress(LevelData.NextLevel.LevelSceneID);
                if (unlockedLevelProgress == null)
                {
                    PlayerProgressTracker.AddProgress(LevelData.NextLevel.LevelSceneID);
                }
            }
        }
        else
        {
            if (LevelData.IsLevelDefaultUnlocked)
            {
                PlayerProgressTracker.AddProgress(LevelData.LevelSceneID);
                ActivateButton();
            }
            else
            {
                DeactivateButton();
            }
        }
    }

    public void ActivateButton()
    {
        IsActive = true;
        sr.color = Color.white;
    }

    public void DeactivateButton()
    {
        IsActive = false;
        sr.color = DeactivatedColor;
    }

    public void OnHovered()
    {
        if (LevelData != null)
        {
            if (IsActive)
            {
                levelSelect.SelectLevel(LevelData);
            }
        }
    }

    public void OnLeave()
    {
        if (LevelData != null)
        {
            if (levelSelect.CurrentLevelData == LevelData)
            {
                levelSelect.SelectLevel(null);
            }
        }
    }

    public void OnSelected()
    {
        levelSelect.LoadLevel();
    }
}
