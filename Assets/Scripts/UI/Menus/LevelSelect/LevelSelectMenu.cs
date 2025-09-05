using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{
    // ----------------------
    //
    // Class for keeping track of, and loading, the currently-hovered-over level in the level select menu
    //
    // ----------------------

    public LevelSelectButton[] LevelButtons;

    public LevelData CurrentLevelData;

    private bool isLevelSelected;
    private int newLevelSceneID;

    // Object references
    private GlobalUIController ui;
    private LevelSelectUI levelSelectUi;

    private void Awake()
    {
        ui = FindFirstObjectByType<GlobalUIController>();
        levelSelectUi = FindFirstObjectByType<LevelSelectUI>();
    }

    private void Start()
    {
        foreach (var level in LevelButtons)
        {
            level.enabled = true;
        }
    }

    private void LoadNewLevelScene()
    {
        SceneManager.LoadScene(newLevelSceneID);
    }

    public void SelectLevel(LevelData newLevelData)
    {
        if (newLevelData != null)
        {
            isLevelSelected = true;
            CurrentLevelData = newLevelData;

            levelSelectUi.EnableLevelDisplay(newLevelData);
        }
        else
        {
            isLevelSelected = false;
            CurrentLevelData = null;

            levelSelectUi.DisableLevelDisplay();
        }
    }

    public void LoadLevel()
    {
        if (isLevelSelected && CurrentLevelData != null)
        {
            newLevelSceneID = CurrentLevelData.LevelSceneID;
            StartCoroutine(ui.PlayFadeOut(true, LoadNewLevelScene));
        }
    }
}
