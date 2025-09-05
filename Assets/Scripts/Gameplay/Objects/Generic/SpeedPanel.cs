using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPanel : BaseLevelObject
{
    // ----------------------
    //
    // This object is one of many "speed panels" that need to be activated under a certain amount of time to open a gate
    //
    // ----------------------

    [Space(10)]
    public bool IsActivated;
    public float ActivationTime;
    public Color ActivationColor;
    public Color CompletedColor;

    // Private variables
    private Color colorCache;

    public override void Start()
    {
        base.Start();
        colorCache = sr.color;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (IsActivated)
            {
                StopCoroutine(ActivationTimer());
            }

            StartCoroutine(ActivationTimer());
        }
    }

    private IEnumerator ActivationTimer()
    {
        IsActivated = true;
        sr.color = ActivationColor;

        yield return new WaitForSeconds(ActivationTime);

        IsActivated = false;
        sr.color = colorCache;
        StopCoroutine(ActivationTimer());
    }

    public void DisablePanel()
    {
        enabled = false;
        col.enabled = false;
        sr.color = CompletedColor;
        StopAllCoroutines();
    }
}
