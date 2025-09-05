using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    // ----------------------
    //
    // Class for controlling the game's current "pause state", as well as triggering specific UI objects to show when paused
    //
    // ----------------------

    public bool IsGamePaused;

    private PlayerInput playerInput;
    private GlobalUIController ui;

    private void Awake()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        ui = FindFirstObjectByType<GlobalUIController>();
    }

    public void PauseGame()
    {
        IsGamePaused = true;

        playerInput.DeactivateInput();
        Time.timeScale = 0;
        ui.ShowPauseMenu();
    }

    public void UnpauseGame()
    {
        IsGamePaused = false;

        playerInput.ActivateInput();
        Time.timeScale = 1;
        ui.HidePauseMenu();
    }
}
