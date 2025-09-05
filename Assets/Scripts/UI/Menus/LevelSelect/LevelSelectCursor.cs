using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LevelSelectCursor : MonoBehaviour
{
    // ----------------------
    //
    // Class for controlling the player's cursor on the level select screen
    //
    // ----------------------

    [Header("Physics Settings")]
    public float MaxMoveSpeed;
    public float MoveAccel;

    // Input variables
    private bool isInputEnabled = true;

    private Vector2 moveInput = Vector2.zero;
    private bool selectInput = false;
    private bool backInput = false;
    private bool pauseInput = false;

    // Component/object references
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private LevelSelectMenu levelSelect;
    private PauseController pause;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = rb.GetComponent<CircleCollider2D>();
        levelSelect = FindFirstObjectByType<LevelSelectMenu>();
        pause = FindFirstObjectByType<PauseController>();
    }

    //  ---------------------------------------------
    //
    //  Input logic
    //
    //  ---------------------------------------------

    private void OnMovement(InputValue value)
    {
        Vector2 originalInput = value.Get<Vector2>();
        Vector2 truncatedInput = new Vector2(
            Mathf.Round(originalInput.x * 10) / 10,
            Mathf.Round(originalInput.y * 10) / 10);

        moveInput = truncatedInput;
    }

    private void OnPrimaryAction(InputValue value)
    {
        selectInput = value.isPressed;
    }

    private void OnSecondaryAction(InputValue value)
    {
        backInput = value.isPressed;
    }

    private void OnPause(InputValue value)
    {
        pauseInput = value.isPressed;
    }

    //  ---------------------------------------------
    //
    //  Internal logic
    //
    //  ---------------------------------------------

    private void Update()
    {
        if (pauseInput)
        {
            pause.PauseGame();
            pauseInput = false;
        }
    }

    private void FixedUpdate()
    {
        if (moveInput != Vector2.zero)
        {
            rb.velocity += moveInput * MoveAccel * Time.fixedDeltaTime;
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, MaxMoveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IMenuButton button = null;
        if (collision.gameObject.TryGetComponent<IMenuButton>(out button))
        {
            button.OnHovered();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isInputEnabled)
        {
            if (selectInput)
            {
                IMenuButton button = null;
                if (collision.gameObject.TryGetComponent<IMenuButton>(out button))
                {
                    button.OnSelected();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IMenuButton button = null;
        if (collision.gameObject.TryGetComponent<IMenuButton>(out button))
        {
            button.OnLeave();
        }
    }
}
