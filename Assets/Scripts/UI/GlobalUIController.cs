using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GlobalUIController : MonoBehaviour
{
    // ----------------------
    //
    // Class for triggering basic UI functions, like fading animations
    //
    // ----------------------

    public RawImage GameDisplay;

    [Space(10)]
    public bool IsFading;
    public float FadeSwirlMax;
    public float FadeSwirlSpeed;

    [Space(10)]
    public GameObject PauseMenu;

    private int fadeDir;

    private Animator anim;
    public delegate void fadingEndDelegate();

    private void Awake()
    {
        Animator[] animators = GetComponentsInChildren<Animator>();
        foreach (var animator in animators)
        {
            if (animator.transform == transform)
            {
                continue;
            }
            else
            {
                anim = animator;
                break;
            }
        }
    }

    private void Start()
    {
        SetSwirlMaterial(FadeSwirlMax);
        StartCoroutine(PlayFadeIn(true, null));
    }

    //  ---------------------------------------------
    //
    //  Publically-accessible functions
    //
    //  ---------------------------------------------

    public IEnumerator PlayFadeIn(bool timedUpdate, fadingEndDelegate endMethod)
    {
        if (timedUpdate)
        {
            anim.updateMode = AnimatorUpdateMode.Normal;
        }
        else
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        IsFading = true;
        fadeDir = -1;
        anim.SetTrigger("FadeIn");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        IsFading = false;
        SetSwirlMaterial(0);

        // Trigger other object behaviour in the scene, if a delegate method has been defined
        if (endMethod != null)
        {
            endMethod.Invoke();
        }
    }

    public IEnumerator PlayFadeOut(bool timedUpdate, fadingEndDelegate endMethod)
    {
        if (timedUpdate)
        {
            anim.updateMode = AnimatorUpdateMode.Normal;
        }
        else
        {
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        IsFading = true;
        SetSwirlMaterial(0);
        fadeDir = 1;
        anim.SetTrigger("FadeOut");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        IsFading = false;

        // Trigger other object behaviour in the scene, if a delegate method has been defined
        if (endMethod != null)
        {
            endMethod.Invoke();
        }
    }

    private void SetSwirlMaterial(float newBlend)
    {
        GameDisplay.material.SetFloat("_SwirlBlend",
            Mathf.Clamp(newBlend, 0, FadeSwirlMax));
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            SetSwirlMaterial(0);
        }

        if (IsFading)
        {
            float blend = GameDisplay.material.GetFloat("_SwirlBlend");
            SetSwirlMaterial(blend + (fadeDir * FadeSwirlSpeed));
        }
    }

    public void ShowPauseMenu()
    {
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(true);
        }
    }

    public void HidePauseMenu()
    {
        
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }
    }
}
