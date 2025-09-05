using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFloatIdleState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player is hovering in place, while in "floating mode"
    //
    // ----------------------

    public override void StartState()
    {
        player.anim.SetBool("Float_IsMoving", false);
    }

    public override void UpdateState()
    {
        if (player.moveInput != Vector2.zero)
        {
            if (player.actionInput1Start)
            {
                stateMachine.ChangeState(typeof(PlayerFloatDashState));
            }
            else
            {
                stateMachine.ChangeState(typeof(PlayerFloatMoveState));
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
