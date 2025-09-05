using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformModeGate : MonoBehaviour
{
    // ----------------------
    //
    // This object is used to switch the player in and out of "platforming mode" inside a level
    //
    // ----------------------

    public enum GateModes
    {
        Enable,
        Disable,
        Both
    };
    public GateModes GateMode;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Vector2 playerMoveDir = collision.rigidbody.velocity.normalized;

        if (!collision.enabled && Vector2.Dot(playerMoveDir, transform.right) > 0)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();

                switch (GateMode)
                {
                    case GateModes.Enable:
                        player.SwitchModes(PlayerController.PlayerModes.Platform);
                        col.isTrigger = false;
                        break;

                    case GateModes.Disable:
                        player.SwitchModes(PlayerController.PlayerModes.Float);
                        col.isTrigger = false;
                        break;

                    case GateModes.Both:
                    default:
                        if (player.currentPlayerMode == PlayerController.PlayerModes.Float)
                        {
                            player.SwitchModes(PlayerController.PlayerModes.Platform);
                        }
                        else
                        {
                            player.SwitchModes(PlayerController.PlayerModes.Float);
                        }
                        break;
                }
            }
        }
    }
}
