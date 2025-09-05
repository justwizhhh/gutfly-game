using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformIdleState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is standing still in "platforming mode"
    //
    // ----------------------

    public override void StartState()
    {
        player.isInCoyoteTime = false;
        
        // Reset the player's stomp ability once they reach the ground
        if (player.stateMachine.PreviousState.GetType() == typeof(PlayerPlatformJumpState))
        {
            player.canStomp = false;
            player.stompAttempts = 0;
        }
    }

    public override void UpdateState()
    {
        if (player.moveInput.x == 0)
        {
            if (player.isGrounded && player.actionInput1Start)
            {
                stateMachine.ChangeState(typeof(PlayerPlatformJumpState));
            }
        }
        else
        {
            stateMachine.ChangeState(typeof(PlayerPlatformMoveState));
        }

        // If stationary in mid-air, let the player stomp here aswell
        if (!player.isGrounded)
        {
            // Only let the player stomp once in the air
            if (player.actionInput2Start && player.stompAttempts < 1)
            {
                player.rb.AddForce(
                    level.Gravity.normalized * player.Platform_StompForce,
                    ForceMode2D.Impulse);
                player.stompAttempts++;
                player.canStomp = false;

                player.anim.SetTrigger("Platform_StartStomp");
            }
        }
    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {

    }
}
