using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndGate : MonoBehaviour
{
    // ----------------------
    //
    // This object is used at the end of the level to stop its "rotation" and to stop keeping track of time/score
    //
    // ----------------------

    private bool hasBeenTriggered;

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasBeenTriggered)
            {
                LevelController.Instance.EndLevel();
                hasBeenTriggered = true;

                GetComponent<Animation>().Play();
            }
        }
    }
}
