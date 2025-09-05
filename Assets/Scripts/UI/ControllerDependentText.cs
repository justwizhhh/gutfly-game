using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerDependentText : MonoBehaviour
{
    // ----------------------
    //
    // This class changes the contents of a TMP_Text object based on the type of controller being used
    //
    // ----------------------

    [TextArea]
    public string KeyboardText;
    [TextArea]
    public string GamepadText;
    
    private TMP_Text text;
    private PlayerInput playerInput;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        playerInput = FindFirstObjectByType<PlayerController>().GetComponent<PlayerInput>();
    }
    
    private void Update()
    {
        if (playerInput.enabled)
        {
            if (playerInput.currentControlScheme.Contains("Gamepad"))
            {
                text.text = GamepadText;
            }
            else if (playerInput.currentControlScheme.Contains("Keyboard"))
            {
                text.text = KeyboardText;
            }
        }
    }
}
