using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Titlescreen : MonoBehaviour
{
    // ----------------------
    //
    // This menu allows the player to go from the titlescreen to the level select in the game's demo
    //
    // ----------------------

    private GlobalUIController ui;

    private void Awake()
    {
        ui = FindFirstObjectByType<GlobalUIController>();
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            StartCoroutine(ui.PlayFadeOut(true, LoadLevelSelect));
        }
    }

    private void LoadLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
