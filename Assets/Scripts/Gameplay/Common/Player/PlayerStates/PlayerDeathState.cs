using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player gets hurt and dies
    //
    // ----------------------

    public override void StartState()
    {
        player.rb.velocity = Vector2.zero;
        player.rb.simulated = false;

        player.input.enabled = false;
        player.actionInput1Start = false;

        player.anim.SetTrigger("Death");

        level.PlayerDead();
    }

    public override void UpdateState()
    {
        
    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {

    }
}
