using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{
    // ----------------------
    //
    // Class for updating the game's UI while inside of a level, for variables such as time and score
    //
    // ----------------------

    [Header("Level Start UI")]
    public TMP_Text LevelIDText;
    public TMP_Text LevelNameText;

    [Space(10)]
    [Header("Level End UI")]
    public TMP_Text EndScoreText;
    public TMP_Text BonusScoreText;
    public TMP_Text FinalScoreText;
    public TMP_Text FinalTimeText;

    [Space(10)]
    [Header("Gameplay UI")]
    public TMP_Text ScoreText;
    public RawImage HealthDisplay;
    public Texture2D[] HealthDisplayTextures;
    public TMP_Text TimeText;
    public TMP_Text ClockTimerText;

    [Space(10)]
    public GameObject DeathMenu;
    public float DeathMenuAppearTime;

    // Component references
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        LevelController.ShowLevelIntro.AddListener(PlayIntro);
        LevelController.ShowLevelOutro.AddListener(PlayOutro);

        LevelController.UpdateScoreUI.AddListener(UpdateScoreDisplay);
        PlayerController.UpdateHealthUI.AddListener(UpdateHealthDisplay);
        LevelController.UpdateTimeUI.AddListener(UpdateTimeDisplay);
        LevelController.UpdateScoreBonusUI.AddListener(UpdateScoreBonusDisplay);
        LevelController.UpdateFinalScoreUI.AddListener(UpdateFinalScoreDisplay);

        RotateToggleBlock.ShowTimerUI.AddListener(ShowClockTimer);
        RotateToggleBlock.UpdateTimerUI.AddListener(UpdateClockTimer);
        RotateToggleBlock.HideTimerUI.AddListener(HideClockTimer);

        LevelController.ShowDeathScreen.AddListener(ShowDeathMenu);
    }

    public void OnDisable()
    {
        LevelController.ShowLevelIntro.RemoveListener(PlayIntro);
        LevelController.ShowLevelOutro.RemoveListener(PlayOutro);

        LevelController.UpdateScoreUI.RemoveListener(UpdateScoreDisplay);
        PlayerController.UpdateHealthUI.RemoveListener(UpdateHealthDisplay);
        LevelController.UpdateTimeUI.RemoveListener(UpdateTimeDisplay);
        LevelController.UpdateScoreBonusUI.RemoveListener(UpdateScoreBonusDisplay);
        LevelController.UpdateFinalScoreUI.RemoveListener(UpdateFinalScoreDisplay);

        RotateToggleBlock.ShowTimerUI.RemoveListener(ShowClockTimer);
        RotateToggleBlock.UpdateTimerUI.RemoveListener(UpdateClockTimer);
        RotateToggleBlock.HideTimerUI.RemoveListener(HideClockTimer);

        LevelController.ShowDeathScreen.RemoveListener(ShowDeathMenu);
    }

    //  ---------------------------------------------
    //
    //  Internal UI-updating logic
    //
    //  ---------------------------------------------

    private void UpdateScoreDisplay(int newScore)
    {
        if (ScoreText != null && ScoreText.IsActive())
        {
            ScoreText.text = PlayerProgressTracker.GenerateReadableRecordScore(newScore);
        }
    }

    private void UpdateHealthDisplay(int newHealth)
    {
        if (HealthDisplay != null && HealthDisplay.IsActive())
        {
            if (newHealth <= HealthDisplayTextures.Length - 1)
            {
                HealthDisplay.texture = HealthDisplayTextures[newHealth];
            }
        }
    }

    private void UpdateTimeDisplay(float newTime)
    {
        if (TimeText != null && TimeText.IsActive())
        {
            TimeText.text = PlayerProgressTracker.GenerateReadableRecordTime(newTime);
        }
    }

    private void UpdateScoreBonusDisplay(int newScoreBonus)
    {
        if (BonusScoreText != null)
        {
            BonusScoreText.text = PlayerProgressTracker.GenerateReadableRecordScore(newScoreBonus);
        }
    }

    private void UpdateFinalScoreDisplay(int newFinalScore)
    {
        if (FinalScoreText != null)
        {
            FinalScoreText.text = PlayerProgressTracker.GenerateReadableRecordScore(newFinalScore);
        }
    }

    private void ShowClockTimer()
    {
        anim.SetTrigger("ShowClockTimer");
    }

    private void UpdateClockTimer(float newTime)
    {
        ClockTimerText.text = (Mathf.Round(newTime * 10) / 10).ToString() + "s";
    }

    private void HideClockTimer()
    {
        ClockTimerText.text = "0.0s";
        anim.SetTrigger("HideClockTimer");
    }

    private IEnumerator DeathMenuTimer()
    {
        yield return new WaitForSeconds(DeathMenuAppearTime);
        DeathMenu.SetActive(true);
    }

    //  ---------------------------------------------
    //
    //  Publically-accessible functions
    //
    //  ---------------------------------------------

    public void ShowUI()
    {
        anim.SetTrigger("Show");
    }

    public void HideUI()
    {
        anim.SetTrigger("Hide");
    }

    public void PlayIntro(LevelData level)
    {
        LevelNameText.text = level.LevelName;
        LevelIDText.text = "Level: " + level.LevelID;
        
        anim.SetTrigger("PlayIntro");
    }

    public void PlayOutro()
    {
        EndScoreText.text = ScoreText.text;
        FinalTimeText.text = TimeText.text;

        anim.SetTrigger("PlayOutro");
    }

    public void ShowDeathMenu()
    {
        HideUI();
        StartCoroutine(DeathMenuTimer());
    }
}
