using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class DeathMenu : BaseMenu
{
    // ----------------------
    //
    // This menu allows the player to restart or quit the level after they have died
    //
    // ----------------------

    // Activate the currently-selected button
    public override void ActivateButton()
    {
        switch (currentSelection)
        {
            // Restart
            default:
            case 0:
                StartCoroutine(ui.PlayFadeOut(false, RestartLevel));
                break;

            // Back to Level Select
            case 1:
                StartCoroutine(ui.PlayFadeOut(false, LoadLevelSelect));
                break;

            // Quit
            case 2:
                Application.Quit();
                break;
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadLevelSelect()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }
}
