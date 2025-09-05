using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenuGate : BaseLevelObject
{
    // ----------------------
    //
    // This object is used to save the player's progress and boot them back to the game's level select menu
    //
    // ----------------------

    public float TransitionDelay;
    public Color TransitionColor;

    // Private variables
    private bool isPlayerNearby;
    private float currentDelayTime;
    private Color colorCache;

    // Component/object references
    private PlayerController player;
    private GlobalUIController ui;

    public override void Awake()
    {
        base.Awake();
        player = FindFirstObjectByType<PlayerController>();
        ui = FindFirstObjectByType<GlobalUIController>();
    }

    public override void Start()
    {
        base.Start();
        colorCache = sr.color;
    }

    public override void Update()
    {
        base.Update();
        if (isPlayerNearby)
        {
            currentDelayTime += Time.deltaTime;
            sr.color = Color.Lerp(colorCache, TransitionColor, currentDelayTime / TransitionDelay);

            if (currentDelayTime >= TransitionDelay)
            {
                player.GetComponent<Rigidbody2D>().simulated = false;
                SaveLevelProgress();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            currentDelayTime = 0;
            sr.color = colorCache;
            isPlayerNearby = false;
        }
    }

    private void SaveLevelProgress()
    {
        PlayerProgressTracker.SaveProgress(
            SceneManager.GetActiveScene().buildIndex,
            true,
            level.CurrentTime,
            level.CurrentScore);

        StartCoroutine(ui.PlayFadeOut(true, LoadNewScene));
    }

    private void LoadNewScene()
    {
        // Go back to the level select menu
        SceneManager.LoadScene("LevelSelect");
    }
}
