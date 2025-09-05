using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningWall : BaseLevelObject
{
    // ----------------------
    //
    // A simple spinning foreground object
    //
    // ----------------------

    [Space(10)]
    public float RotationSpeed;
    public bool IsRotationRelative;

    public override void Update()
    {
        
    }

    public override void FixedUpdate()
    {
        if (level.IsLevelActive)
        {
            if (!IsRotationRelative)
            {
                rb.rotation += RotationSpeed * Time.fixedDeltaTime;
            }
            else
            {
                rb.rotation += RotationSpeed * level.CurrentRotationSpeed * Time.fixedDeltaTime;
            }
        }
    }
}
