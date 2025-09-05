using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloatMoveState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is moving in all directions, in "floating mode"
    //
    // ----------------------

    public override void StartState()
    {
        player.anim.SetBool("Float_IsMoving", true);
    }

    public override void UpdateState()
    {
        if (player.moveInput != Vector2.zero)
        {
            if (player.actionInput1Start && !player.isDashOnCooldown)
            {
                if (!player.isInSpeedBooster)
                {
                    stateMachine.ChangeState(typeof(PlayerFloatDashState));
                }
            }

            // Pause and come to a stop if the second action button is held
            if (player.actionInput2Start)
            {
                player.rb.drag = player.Float_PauseDrag;
            }
            if (player.actionInput2End)
            {
                player.rb.drag = player.rbDragCache;
            }
        }
        else
        {
            stateMachine.ChangeState(typeof(PlayerFloatIdleState));
        }
    }

    public override void FixedUpdateState()
    {
        if (player.moveInput != Vector2.zero)
        {
            if (player.rb.velocity.sqrMagnitude < player.Float_MaxMoveSpeed)
            {
                player.rb.AddForce((transform.right * player.moveInput.x
                + transform.up * player.moveInput.y)
                     * player.Float_MoveForce * Time.fixedDeltaTime);
            }
        }
    }
}
