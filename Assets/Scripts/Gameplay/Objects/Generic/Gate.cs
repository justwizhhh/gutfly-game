using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // ----------------------
    //
    // A generic gate object that can open and close
    //
    // ----------------------

    private bool isOpened;
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void Open()
    {
        if (!isOpened)
        {
            anim.clip = anim.GetClip("GateOpen");
            anim.Play();
            isOpened = true;
        }
    }

    public void Close()
    {
        if (isOpened)
        {
            anim.clip = anim.GetClip("GateClose");
            anim.Play();
            isOpened = false;
        }
    }
}
