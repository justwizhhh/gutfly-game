using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : BaseLevelObject
{
    // ----------------------
    //
    // This object is what's shot out of cannon block objects inside of a level
    //
    // ----------------------

    [HideInInspector] public Collider2D CannonCol;

    public override void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == CannonCol)
        {
            Physics2D.IgnoreCollision(collision, CannonCol);
        }
        else if (collision.CompareTag("Player") || collision.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            gameObject.SetActive(false);
        } 
    }
}
