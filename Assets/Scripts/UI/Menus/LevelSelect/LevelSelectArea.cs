using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectArea : MonoBehaviour
{
    // ----------------------
    //
    // This object turns on the camera for a select section of the level select area
    //
    // ----------------------

    public CinemachineVirtualCamera Camera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Camera != null)
            {
                Camera.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Camera != null)
            {
                Camera.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size + GetComponent<BoxCollider2D>().offset);
    }
}
