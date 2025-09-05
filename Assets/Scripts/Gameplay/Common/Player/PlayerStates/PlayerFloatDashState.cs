using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloatDashState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is dashing towards a set direction, in "floating mode"
    // TO-DO: make turning feel a bit more smooth somehowm maybe?
    //
    // ----------------------

    private bool isDashing;
    private bool canDashStop;

    private Vector2 startDirection;
    private Vector2 dashDirection;
    
    public override void StartState()
    {
        if (player.isDashOnCooldown)
        {
            player.actionInput1Start = false;
            stateMachine.ChangeState(typeof(PlayerFloatMoveState));
        }
        else
        {
            player.col.radius = player.Float_DashColSize;
            StartCoroutine(DashTimer());

            player.rb.velocity = 
                (transform.right * player.moveInput.x
                + transform.up * player.moveInput.y).normalized 
                    * player.Float_DashMoveVelocity * Time.fixedDeltaTime;
            startDirection = player.rb.velocity.normalized;

            player.anim.SetTrigger("Float_StartDash");
        }
    }

    public override void UpdateState()
    {
        // Pause and come to a stop if the second action button is held
        if (player.actionInput2Start)
        {
            player.rb.drag = player.Float_PauseDrag;
        }
        if (player.actionInput2End)
        {
            player.rb.drag = player.rbDragCache;
        }

        if (player.actionInput1End && canDashStop)
        {
            EndDash(true);
        }

        if (player.isInSpeedBooster)
        {
            EndDash(false);
        }

        if (player.moveInput != Vector2.zero)
        {
            Vector2 inputDir = (transform.right * player.moveInput.x
                + transform.up * player.moveInput.y).normalized;
            dashDirection = Vector2.Lerp(startDirection, inputDir, player.Float_DashTurnTime);
        }
    }

    public override void FixedUpdateState()
    {
        if (isDashing)
        {
            if (player.rb.velocity.sqrMagnitude < player.Float_DashMoveVelocity)
            {
                player.rb.velocity = dashDirection * (player.Float_DashMoveVelocity * Time.fixedDeltaTime);
            }
        }
    }

    public override void EndState()
    {
        isDashing = false;
        canDashStop = false;
        StopAllCoroutines();
    }

    private void EndDash(bool resetVelocity)
    {
        isDashing = false;
        canDashStop = false;
        player.actionInput1Start = false;

        if (resetVelocity)
        {
            Vector2 newDir =
                (transform.right * player.moveInput.x
                + transform.up * player.moveInput.y).normalized;
            if (newDir != Vector2.zero)
            {
                player.rb.velocity = newDir * Mathf.Sqrt(player.Float_MaxMoveSpeed);
            }
            else
            {
                player.rb.velocity = player.rb.velocity.normalized * Mathf.Sqrt(player.Float_MaxMoveSpeed);
            }
        }
        
        player.col.radius = player.colSizeCache;
        player.anim.SetTrigger("Float_EndDash");

        if (player.currentPlayerMode == PlayerController.PlayerModes.Float)
        {
            stateMachine.ChangeState(typeof(PlayerFloatMoveState));
        }

        StopAllCoroutines();
        StartCoroutine(DashDelayTimer());
    }

    private IEnumerator DashTimer()
    {
        isDashing = true;

        yield return new WaitForSeconds(player.Float_DashMinTime);

        canDashStop = true;

        yield return new WaitForSeconds(player.Float_DashMaxTime);

        EndDash(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground"))
            {
                EndDash(true);
            }
        }
    }

    private IEnumerator DashDelayTimer()
    {
        player.isDashOnCooldown = true;

        yield return new WaitForSeconds(player.Float_DashDelayTime);

        player.isDashOnCooldown = false;
    }
}
