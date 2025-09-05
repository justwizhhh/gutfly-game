using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : BaseMenu
{
    // ----------------------
    //
    // This menu allows the player to temporarily pause, restart the level, or quit the game
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

            // Restart
            case 1:
                StartCoroutine(ui.PlayFadeOut(false, RestartLevel));
                break;

            // Back to Level Select
            case 2:
                LoadLevelSelect();
                break;
                
            // Quit
            case 3:
                Application.Quit();
                break;
        }
    }

    private void RestartLevel()
    {
        pauseController.UnpauseGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadLevelSelect()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }
}
