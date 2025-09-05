using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLevelObject : MonoBehaviour
{
    // ----------------------
    //
    // Base class for an object inside of a level.
    //
    // ----------------------

    [Header("Physics Settings")]
    public bool IsUsingGravity;

    protected LevelController level;
    protected Rigidbody2D rb;
    protected Collider2D col;
    protected SpriteRenderer sr;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Start()
    {
        level = LevelController.Instance;

        rb.gravityScale = 0;
    }

    public virtual void Update()
    {
        sr.gameObject.transform.eulerAngles = new Vector3(0, 0, level.UpAngle);
    }

    public virtual void FixedUpdate()
    {
        if (IsUsingGravity)
        {
            rb.AddForce(level.Gravity);
        }
    }
}
