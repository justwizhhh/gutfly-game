using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStartGate : MonoBehaviour
{
    // ----------------------
    //
    // This object is used at the beginning of the level to start its "rotation" and timer
    //
    // ----------------------

    private bool hasBeenTriggered;

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasBeenTriggered)
            {
                LevelController.Instance.StartLevel();
                hasBeenTriggered = true;

                GetComponent<Animation>().Play();
            }
        }
    }
}
