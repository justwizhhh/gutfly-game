using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointGate : MonoBehaviour
{
    // ----------------------
    //
    // This object respawns the player at its current position upon reloading the scene
    //
    // ----------------------

    public bool isCheckpointActive;
    public bool isResettingPlayer;

    private CheckpointManager manager;

    private void Start()
    {
        manager = CheckpointManager.active;

        if (manager.CurrentPosition == (Vector2)transform.position)
        {
            isCheckpointActive = true;
            isResettingPlayer = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCheckpointActive)
        {
            manager.SetNewCheckpoint(this, collision.GetComponent<PlayerController>());
            isCheckpointActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isResettingPlayer)
        {
            manager.RestartLevel();
            isResettingPlayer = false;
        }
    }
}
