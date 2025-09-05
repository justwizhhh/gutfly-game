using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseMenu : MonoBehaviour
{
    // ----------------------
    //
    // Base class for simple menu interfaces in the game
    //
    // ----------------------

    public List<GameObject> MenuButtons = new List<GameObject>();
    public Color ButtonUnselectedColor;
    public Color ButtonSelectedColor;
    public float ButtonSelectDelay;

    protected List<TMP_Text> MenuButtonTexts = new List<TMP_Text>();
    protected int currentSelection;
    protected bool isSelectionDelayed;

    // Object references
    protected GlobalUIController ui;
    protected PlayerInput input;
    protected PlayerInput playerInput;

    public virtual void Awake()
    {
        foreach (GameObject button in MenuButtons)
        {
            MenuButtonTexts.Add(button.GetComponent<TMP_Text>());
        }

        ui = FindFirstObjectByType<GlobalUIController>();
        input = GetComponent<PlayerInput>();
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        input.ActivateInput();

        if (input.user != null && input.user.valid)
        {
            switch (playerInput.currentControlScheme)
            {
                default:
                case "Keyboard":
                    input.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
                    break;

                case "Gamepad":
                    input.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
                    break;
            }
        }

        currentSelection = 0;
        SelectButtons();
    }

    private void OnDisable()
    {
        input.DeactivateInput();
    }

    private IEnumerator InputDelay()
    {
        isSelectionDelayed = true;
        yield return new WaitForSecondsRealtime(ButtonSelectDelay);
        isSelectionDelayed = false;
    }

    // Select a button on the menu
    public virtual void OnMovement(InputValue value)
    {
        if (!isSelectionDelayed)
        {
            int movement = Mathf.RoundToInt(value.Get<Vector2>().y);
            if (movement != 0)
            {
                currentSelection = Mathf.Clamp(currentSelection - movement, 0, MenuButtons.Count - 1);

                SelectButtons();
                StartCoroutine(InputDelay());
            }
        }
    }

    public virtual void OnPrimaryAction(InputValue value)
    {
        if (value.isPressed)
        {
            ActivateButton();
        }
    }

    public virtual void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            ActivateButton();
        }
    }

    // Back out of the pause menu, and back into the game
    public virtual void OnSecondaryAction(InputValue input)
    {
        
    }

    // Highlight the selected button a different colour from the rest
    // Temporary, will make pause menu prettier later hopefully
    private void SelectButtons()
    {
        for (int i = 0; i < MenuButtons.Count; i++)
        {
            if (i != currentSelection)
            {
                MenuButtonTexts[i].color = ButtonUnselectedColor;
            }
            else
            {
                MenuButtonTexts[i].color = ButtonSelectedColor;
            }
        }
    }

    public virtual void ActivateButton()
    {

    }
}
