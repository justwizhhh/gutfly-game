using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerZiplineState : BasePlayerState
{
    // ----------------------
    //
    // State for when the player gets transported through a zipline
    //
    // ----------------------

    public override void StartState()
    {
        player.rb.velocity = Vector2.zero;
        player.col.enabled = false;

        player.actionInput1Start = false;
    }

    public override void UpdateState()
    {

    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {
        player.col.enabled = true;
    }
}
