using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelSelectPauseMenu : BaseMenu
{
    // ----------------------
    //
    // A simpler pause menu, found on in the main level select area
    //
    // ----------------------

    private PauseController pauseController;

    public override void Awake()
    {
        base.Awake();
        pauseController = FindFirstObjectByType<PauseController>();
    }

    // Back out of the pause menu, and back into the game
    public override void OnSecondaryAction(InputValue input)
    {
        if (input.isPressed)
        {
            pauseController.UnpauseGame();
        }
    }

    // Activate the currently-selected button
    public override void ActivateButton()
    {
        switch (currentSelection)
        {
            // Continue
            default:
            case 0:
                pauseController.UnpauseGame();
                break;

            // Back to Titlescreen
            case 1:
                LoadTitlescreen();
                break;

            // Quit
            case 2:
                Application.Quit();
                break;
        }
    }

    private void LoadTitlescreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Titlescreen");
    }
}
