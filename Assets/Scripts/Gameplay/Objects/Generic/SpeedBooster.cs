using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpeedBooster : MonoBehaviour
{
    // ----------------------
    //
    // This object pushes and boosts the player along a specified direction
    //
    // ----------------------

    [Space(10)]
    public float BoostForce;
    public float BoostMaxSpeed;
    public float BoostAngle;
    public bool IsBoostAngleRelative;

    // Private variables
    private float dirRadians;
    private float dirAngle;
    private Vector2 direction;

    // Component references
    private Material material;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        // Get the direction the speed booster is aiming at
        if (IsBoostAngleRelative)
        {
            // Boost the player in a direction relative to the level's current "up angle"
            dirAngle = (-BoostAngle + LevelController.Instance.UpAngle + 90);
        }
        else
        {
            // Boost the player in a direction relative to Vector2.up
            dirAngle = (-BoostAngle + 90);
        }

        dirRadians = dirAngle * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(dirRadians), Mathf.Sin(dirRadians));

        // Aim the booster's arrow graphic in its current direction
        material.SetFloat(Shader.PropertyToID("_ScrollAngle"), dirAngle - 90);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.GetComponent<PlayerController>().rb;

            if (playerRb.velocity.sqrMagnitude < BoostMaxSpeed)
            {
                playerRb.AddForce(direction * BoostForce * Time.fixedDeltaTime);
            }
        }
    }
}
