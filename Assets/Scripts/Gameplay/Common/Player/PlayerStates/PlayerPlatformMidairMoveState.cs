using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformMidairMoveState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is moving left and right, in the air, in "platforming mode"
    //
    // ----------------------

    public override void StartState()
    {
        // If the player isn't currently jumping (and they are currently falling), allow coyote time to put them back into the jump state
        if (player.stateMachine.PreviousState.GetType() != typeof(PlayerPlatformJumpState)
            && Vector2.Dot(player.rb.velocity.normalized, -level.UpAxis) > 0
            && player.currentPlayerMode == PlayerController.PlayerModes.Platform)
        {
            StartCoroutine(CoyoteTimeTimer());
        }
        else
        {
            StopCoroutine(CoyoteTimeTimer());
            player.isInCoyoteTime = false;
        }

        player.isGrounded = false;
        player.noLongerJumping = false;
    }

    public override void UpdateState()
    {
        if (player.moveInput.x != 0)
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
                // Jumping again during coyote time
                if (player.actionInput1Start && player.isInCoyoteTime)
                {
                    stateMachine.ChangeState(typeof(PlayerPlatformJumpState));
                }

                // Only let the player stomp once in the air
                if (player.actionInput2Start && player.canStomp && player.stompAttempts < 1)
                {
                    player.rb.AddForce(
                        LevelController.Instance.Gravity.normalized * player.Platform_StompForce,
                        ForceMode2D.Impulse);
                    player.stompAttempts++;
                    player.canStomp = false;

                    player.anim.SetTrigger("Platform_StartStomp");
                }
            }

            if (player.isGrounded)
            {
                player.actionInput1Start = false;
                player.noLongerJumping = true;
                stateMachine.ChangeState(typeof(PlayerPlatformMoveState));
            }
        }
        else
        {
            stateMachine.ChangeState(typeof(PlayerPlatformJumpState));
        }
    }

    public override void FixedUpdateState()
    {
        Vector2 horMovement = (player.rb.velocity * (player.moveInput.x * level.RightAxis));
        if (horMovement.sqrMagnitude < player.Platform_MaxMoveSpeed)
        {
            player.rb.AddForce(
                (level.RightAxis * player.moveInput.x)
                    .normalized * player.Float_MoveForce * Time.fixedDeltaTime);
        }

        if (!player.noLongerJumping)
        {
            player.rb.AddForce(LevelController.Instance.Gravity.normalized * -player.Platform_MaxJumpForce * Time.fixedDeltaTime);
        }
    }

    private IEnumerator CoyoteTimeTimer()
    {
        player.isInCoyoteTime = true;
        yield return new WaitForSeconds(player.Platform_CoyoteTime);
        player.isInCoyoteTime = false;
    }

    public override void EndState()
    {

    }
}
