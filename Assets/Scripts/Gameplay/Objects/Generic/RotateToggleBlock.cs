using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RotateToggleBlock : MonoBehaviour
{
    // ----------------------
    //
    // This object toggles and sets the level's current angle of rotation, as well as the direction it rotates in
    //
    // ----------------------

    [Space(10)]
    public float PlayerPushForce;

    [Space(10)]
    public bool IsRotationChanged;
    public float NewLevelAngle;
    public float RotationChangeSpeed;
    public float RotationChangePauseTime;
    public bool ShowPauseTimer;

    [Space(10)]
    public bool IsRotationSpeedChanged;
    public float NewRotationSpeed;

    [Space(10)]
    public bool IsDirectionChanged;

    private bool isRotPaused;
    private float currentPauseTime;

    public static UnityEvent ShowTimerUI = new UnityEvent();
    public static UnityEvent<float> UpdateTimerUI = new UnityEvent<float>();
    public static UnityEvent HideTimerUI = new UnityEvent();

    private Animation anim;
    private Transform childIcon;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        childIcon = transform.GetChild(0).GetChild(0);
    }

    private void ChangeRotDirection(LevelController level)
    {
        if (level.CurrentRotationDirection == LevelController.RotationDirections.Clockwise)
        {
            level.SetRotationDirection(LevelController.RotationDirections.CounterClockwise);
        }
        else
        {
            level.SetRotationDirection(LevelController.RotationDirections.Clockwise);
        }
    }

    private void StartRotTimer()
    {
        if (RotationChangePauseTime > 0)
        {
            LevelController.Instance.IsLevelRotating = false;
            currentPauseTime = RotationChangePauseTime;
            isRotPaused = true;

            if (ShowPauseTimer)
            {
                ShowTimerUI.Invoke();
            }
        }
    }

    private void Update()
    {
        if (isRotPaused)
        {
            currentPauseTime -= Time.deltaTime;
            UpdateTimerUI.Invoke(currentPauseTime);

            if (currentPauseTime <= 0)
            {
                LevelController.Instance.IsLevelRotating = true;
                isRotPaused = false;

                if (ShowPauseTimer && LevelController.Instance.IsLevelActive)
                {
                    HideTimerUI.Invoke();
                }
            }
        }

        childIcon.transform.eulerAngles = new Vector3(0, 0, LevelController.Instance.UpAngle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Apply level rotation settings
            if (IsDirectionChanged)
            {
                ChangeRotDirection(LevelController.Instance);
            }

            if (IsRotationSpeedChanged)
            {
                LevelController.Instance.SetRotationSpeed(NewRotationSpeed);
            }
            
            if (IsRotationChanged)
            {
                LevelController.Instance.SetLevelToAngle(NewLevelAngle, RotationChangeSpeed, StartRotTimer);
            }

            // Push the player away from the block
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.rb.AddForce(
                (player.transform.position - transform.position).normalized * PlayerPushForce, 
                ForceMode2D.Impulse);

            // Play hit animation if an animation has been assigned
            if (anim != null)
            {
                anim.Play();
            }
        }
    }
}
