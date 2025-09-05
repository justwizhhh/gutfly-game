using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformMoveState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is moving left and right, on the ground, in "platforming mode"
    //
    // ----------------------

    public override void StartState()
    {
        player.isInCoyoteTime = false;

        // Reset the player's stomp ability once they reach the ground
        if (player.stateMachine.PreviousState.GetType() == typeof(PlayerPlatformMidairMoveState))
        {
            player.canStomp = false;
            player.stompAttempts = 0;
        }
    }

    public override void UpdateState()
    {
        if (player.moveInput.x != 0)
        {
            if (player.isGrounded)
            {
                if (player.actionInput1Start)
                {
                    stateMachine.ChangeState(typeof(PlayerPlatformJumpState));
                }
            }
            else
            {
                stateMachine.ChangeState(typeof(PlayerPlatformMidairMoveState));
            }
        }
        else
        {
            stateMachine.ChangeState(typeof(PlayerPlatformIdleState));
        }
    }

    public override void FixedUpdateState()
    {
        if (player.rb.velocity.sqrMagnitude < player.Platform_MaxMoveSpeed)
        {
            player.rb.AddForce(
                (level.RightAxis * player.moveInput.x)
                    .normalized * player.Float_MoveForce * Time.fixedDeltaTime);
        }

        if (player.currentSlopeDirection != Vector2.zero)
        {
            player.rb.AddForce(player.currentSlopeDirection * -player.moveInput.x * player.Platform_GravityScale * Time.fixedDeltaTime);
        }
    }

    public override void EndState()
    {

    }
}
