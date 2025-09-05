using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelController : MonoBehaviour
{
    // ----------------------
    //
    // Class for controlling and keeping track of all global level-related variables, including its current rotation.
    //
    // ----------------------

    public static LevelController Instance;

    public enum RotationDirections
    {
        CounterClockwise,
        Clockwise
    }

    public LevelData LevelData;

    [Space(10)]
    [Header("Main Level Settings")]
    public bool IsLevelRotating;
    public float GravityScale;
    public float StartRotationSpeed;
    public RotationDirections StartRotationDirection;
    public float RotationAccel;
    public float MaxRotationSpeed;

    [Space(15)]
    [Header("Current Level Variables")]
    public bool IsLevelActive;
    public int CurrentScore;
    public float CurrentTime;
    [Space(10)]
    public float UpAngle;
    public Vector2 UpAxis;
    public Vector2 RightAxis;
    public Vector2 Gravity;
    public float CurrentRotationSpeed;
    public RotationDirections CurrentRotationDirection;

    [Space(15)]
    [Header("Score Bonus Settings")]
    public int MaxScoreBonus;

    public static UnityEvent EnableObjects = new UnityEvent();
    public static UnityEvent DisableObjects = new UnityEvent();

    // UI updating variables
    private GameplayUIController gameplayUi;
    public static UnityEvent<LevelData> ShowLevelIntro = new UnityEvent<LevelData>();
    public static UnityEvent ShowLevelOutro = new UnityEvent();

    public static UnityEvent<int> UpdateScoreUI = new UnityEvent<int>();
    public static UnityEvent<float> UpdateTimeUI = new UnityEvent<float>();
    public static UnityEvent<int> UpdateScoreBonusUI = new UnityEvent<int>();
    public static UnityEvent<int> UpdateFinalScoreUI = new UnityEvent<int>();

    public static UnityEvent ShowDeathScreen = new UnityEvent();

    // Object references
    private PlayerController player;
    private ParallaxBackground background;

    // Misc. variables
    public delegate void delegateEndMethod();

    //  ---------------------------------------------
    //
    //  Internal level logic
    //
    //  ---------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameplayUi = FindFirstObjectByType<GameplayUIController>();

        player = FindFirstObjectByType<PlayerController>();
        background = FindFirstObjectByType<ParallaxBackground>();
    }

    private void Start()
    {
        CurrentRotationSpeed = StartRotationSpeed;
        CurrentRotationDirection = StartRotationDirection;

        UpdateLevelRotation();
    }

    private void FixedUpdate()
    {
        if (IsLevelActive)
        {
            UpdateLevelRotation();
        }
    }

    private void Update()
    {
        if (IsLevelActive)
        {
            UpdateTimer();

            if (Input.GetKeyDown(KeyCode.H))
            {
                Debug.Log(PlayerProgressTracker.GenerateReadableRecordTime(CurrentTime));
            }
        }
    }

    // "Rotate" the in-game level, by updating the current "up direction" and object gravity every frame
    [ContextMenu("Update Level Rotation")]
    private void UpdateLevelRotation()
    {
        float upRadians = (UpAngle + 90) * Mathf.Deg2Rad;
        UpAxis = new Vector2(Mathf.Cos(upRadians), Mathf.Sin(upRadians));
        RightAxis = new Vector2(UpAxis.y, -UpAxis.x);
        Gravity = (UpAxis * -1) * GravityScale;

        // Gradually speed up the level's rotation until it reaches max speed
        if (IsLevelRotating)
        {
            UpAngle += Time.deltaTime * (CurrentRotationSpeed * ((int)CurrentRotationDirection * 2 - 1));
            CurrentRotationSpeed += RotationAccel;

            if (UpAngle > 360)
            {
                UpAngle -= UpAngle;
            }

            // Check if max spin speed has been reached, and initiate Game-Over if it has
            if (CurrentRotationSpeed >= MaxRotationSpeed)
            {
                Debug.Log("Game Over!!!");
            }
        }
    }

    private IEnumerator ManualLevelRotation(float newAngle, float rotSpeed, delegateEndMethod endMethod)
    {
        // Spin until the "level" is within the range of desired rotation
        IsLevelRotating = false;

        while (!Mathf.Approximately(Mathf.DeltaAngle(UpAngle, newAngle), 0))
        {
            UpAngle = Mathf.MoveTowardsAngle(UpAngle, newAngle, rotSpeed * Time.deltaTime);
            UpdateLevelRotation();

            yield return null;
        }

        UpAngle = newAngle;
        UpdateLevelRotation();

        IsLevelRotating = true;

        // Trigger delegate method if any was provided by another object
        if (endMethod != null)
        {
            endMethod.Invoke();
        }
    }

    // Depending on how quickly the player completes the level, they get bonus points added to their total score
    private int CalculateScoreBonus()
    {
        int score = Mathf.FloorToInt(
            Mathf.Lerp(
                MaxScoreBonus, 0, (CurrentTime - LevelData.MinCompletionTime) / LevelData.MaxCompletionTime) / 10) * 10;
        //Debug.Log(PlayerProgressTracker.GenerateReadableRecordScore(score));
        return score;
    }

    //  ---------------------------------------------
    //
    //  Publically-accessible level functions (for other objects to use)
    //
    //  ---------------------------------------------

    public void StartLevel(PlayerController.PlayerModes newPlayerMode = PlayerController.PlayerModes.Float)
    {
        IsLevelActive = true;
        EnableLevelRotation(true);

        player.currentPlayerMode = newPlayerMode;
        EnableObjects.Invoke();

        ShowLevelIntro.Invoke(LevelData);
    }

    public void ResetPlayer(Vector2 newPosition, PlayerController.PlayerModes newPlayerMode)
    {
        player.SetPosition(newPosition);
        background.transform.position = newPosition;
    }

    public void ResetLevel(float newAngle, float newTime, int newScore)
    {
        UpAngle = newAngle;
        UpdateLevelRotation();

        CurrentTime = newTime;
        CurrentScore = newScore;
    }

    public void EndLevel()
    {
        IsLevelActive = false;
        EnableLevelRotation(false);
        StopAllCoroutines();

        CalculateScoreBonus();

        DisableObjects.Invoke();

        // Update the UI screen at the end of the level
        ShowLevelOutro.Invoke();

        UpdateScoreUI.Invoke(CurrentScore);
        int bonus = CalculateScoreBonus();
        UpdateScoreBonusUI.Invoke(bonus);
        CurrentScore += bonus;
        UpdateFinalScoreUI.Invoke(CurrentScore);
    }

    public void UpdateScore(int addedScore)
    {
        CurrentScore += addedScore;
        UpdateScoreUI.Invoke(CurrentScore);
    }

    public void UpdateTimer()
    {
        CurrentTime += Time.deltaTime;
        UpdateTimeUI.Invoke(CurrentTime);
    }

    public void PlayerDead()
    {
        IsLevelActive = false;
        EnableLevelRotation(false);
        StopAllCoroutines();

        ShowDeathScreen.Invoke();
    }

    // Toggle the speed at which the level is spinning
    public void SetRotationSpeed(float newSpeed)
    {
        CurrentRotationSpeed = newSpeed;
    }

    // Toggle whether the level is spinning clockwise or anti-clockwise
    public void SetRotationDirection(RotationDirections newDirection)
    {
        CurrentRotationDirection = newDirection;
    }

    // Turn the level's "on" and "off"
    public void EnableLevelRotation(bool enabled)
    {
        IsLevelRotating = enabled;
        UpdateLevelRotation();
    }

    // Manually spin the level around until it reaches the specified angle
    public void SetLevelToAngle(float newAngle, float rotSpeed, delegateEndMethod endMethod)
    {
        IsLevelRotating = false;
        UpdateLevelRotation();

        StopAllCoroutines();
        StartCoroutine(ManualLevelRotation(newAngle, rotSpeed, endMethod));
    }
}
