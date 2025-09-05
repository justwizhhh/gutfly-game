using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    // ----------------------
    //
    // This object switchesthe player's view to a different camera object
    //
    // ----------------------

    public CinemachineVirtualCamera NewCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (NewCamera != null)
            {
                NewCamera.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (NewCamera != null)
            {
                NewCamera.gameObject.SetActive(false);
            }
        } 
    }
}
