using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformJumpState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is jumping up in "platforming mode"
    //
    // ----------------------

    public override void StartState()
    {
        if (stateMachine.PreviousState.GetType() != typeof(PlayerPlatformMidairMoveState) 
            || (player.isInCoyoteTime && player.actionInput1Start))
        {
            player.isGrounded = false;
            player.noLongerJumping = false;
            player.isInCoyoteTime = false;
            player.currentJumpTime = 0;
            player.canStomp = false;
            player.stompAttempts = 0;

            // Offset the player's gravity before jumping (does this work consistently? idk)
            float gravVelocity = Vector2.Dot(player.rb.velocity, level.Gravity.normalized);
            if (gravVelocity > 0f)
                player.rb.velocity -= level.Gravity.normalized * gravVelocity;

            player.rb.AddForce(
                    level.Gravity.normalized * -player.Platform_MaxJumpStartForce,
                    ForceMode2D.Impulse);

            player.anim.SetTrigger("Platform_Jump");
        }
    }

    public override void UpdateState()
    {
        // Only continue moving upwards while the player is holding the jump button. Otherwise, force them back down
        if (!player.actionInput1End && player.currentJumpTime <= player.Platform_MaxJumpTime)
        {
            if (!player.noLongerJumping)
            {
                player.currentJumpTime += Time.deltaTime;
            }
        }
        else
        {
            player.noLongerJumping = true;
        }

        // Stomping while mid-air
        if (player.actionInput1End && !player.isInCoyoteTime && !player.isGrounded)
        {
            player.canStomp = true;
        }

        if (player.noLongerJumping)
        {
            // Only let the player stomp once in the air
            if (player.actionInput2Start && player.canStomp && player.stompAttempts < 1)
            {
                player.rb.AddForce(
                    level.Gravity.normalized * player.Platform_StompForce,
                    ForceMode2D.Impulse);
                player.stompAttempts++;
                player.canStomp = false;

                player.anim.SetTrigger("Platform_StartStomp");
            }
        }

        if (player.moveInput.x != 0)
        {
            stateMachine.ChangeState(typeof(PlayerPlatformMidairMoveState));
        }

        if (player.isGrounded && Mathf.Round(Vector3.Dot(player.rb.velocity.normalized, level.UpAxis)) <= 0)
        {
            player.actionInput1Start = false;
            player.noLongerJumping = true;
            stateMachine.ChangeState(typeof(PlayerPlatformIdleState));
        }
    }

    public override void FixedUpdateState()
    {
        if (!player.noLongerJumping)
        {
            player.rb.AddForce(level.Gravity.normalized * -player.Platform_MaxJumpForce * Time.fixedDeltaTime);
        }
    }

    public override void EndState()
    {

    }
}
