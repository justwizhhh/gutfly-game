using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialTextBox : MonoBehaviour
{
    // ----------------------
    //
    // This object shows a text-box in the game's tutorial section(s), explaining different controls and gameplay features
    //
    // ----------------------

    public Canvas TextboxCanvas;

    private TMP_Text[] textboxes;
    private Animation[] textboxAnimations;

    private void Awake()
    {
        if (TextboxCanvas != null)
        {
            textboxes = TextboxCanvas.GetComponentsInChildren<TMP_Text>();
            textboxAnimations = TextboxCanvas.GetComponentsInChildren<Animation>();

            foreach (TMP_Text textbox in textboxes)
            {
                Color colorCache = textbox.color;
                textbox.color = new Color(colorCache.r, colorCache.g, colorCache.b, 0);
            }
        }
    }

    private void Update()
    {
        LevelController level = LevelController.Instance;
        TextboxCanvas.transform.eulerAngles = new Vector3(0, 0, level.UpAngle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (Animation animation in textboxAnimations)
            {
                if (animation != null)
                {
                    animation.clip = animation.GetClip("ShowTextbox");
                    animation.Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (Animation animation in textboxAnimations)
            {
                if (animation != null)
                {
                    animation.clip = animation.GetClip("HideTextbox");
                    animation.Play();
                }
            }
        }
    }
}
