using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class Collectible : BaseLevelObject
{
    // ----------------------
    //
    // This is a generic collectible that the player gets score from inside of levels
    //
    // ----------------------

    [Space(10)]
    public int Score;

    [Space(10)]
    public float MinMoveSpeed;
    public float MaxMoveSpeed;
    public float MoveAccel;
    public float MinPlayerRadius;

    [Space(10)]
    public Sprite[] RandomTextures;

    private bool isMovingTowardsPlayer;
    private float currentMoveSpeed;
    private float currentMoveAccel;

    // Object/component references
    [HideInInspector] public Rigidbody2D public_rb;
    [HideInInspector] public Collider2D public_col;
    private GameObject particles;
    private Rigidbody2D playerRB;


    public override void Awake()
    {
        base.Awake();

        public_rb = GetComponent<Rigidbody2D>();
        public_col = GetComponent<Collider2D>();
        particles = transform.GetChild(0).gameObject;
        particles.SetActive(false);

        playerRB = FindFirstObjectByType<PlayerController>().rb;
    }

    public override void Start()
    {
        base.Start();
        if (RandomTextures.Length != 0)
        {
            sr.sprite = RandomTextures[Random.Range(0, RandomTextures.Length - 1)];
        }
    }

    public override void Update()
    {
        base.Update();
        
        // Start moving towards the player if they are within a close-enough radius
        if (!isMovingTowardsPlayer)
        {
            if (Vector2.Distance(playerRB.position, rb.position) < MinPlayerRadius)
            {
                isMovingTowardsPlayer = true;
            }
        }
    }

    public override void FixedUpdate()
    {
        if (isMovingTowardsPlayer)
        {
            currentMoveAccel = Mathf.Clamp(currentMoveAccel + MoveAccel * Time.fixedDeltaTime, 0, 1);
            currentMoveSpeed = Mathf.Lerp(MinMoveSpeed * Time.fixedDeltaTime, MaxMoveSpeed * Time.fixedDeltaTime, currentMoveAccel);

            // Hover towards the player with increasing speed
            rb.MovePosition(Vector2.MoveTowards(rb.position, playerRB.position, currentMoveSpeed));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // For when the collectible physically touches the player and gets collected
        if (collision.CompareTag("Player"))
        {
            level.UpdateScore(Score);
            particles.SetActive(true);
            transform.DetachChildren();
            gameObject.SetActive(false);
        }
    }

    public void ForceFollowPlayer()
    {
        isMovingTowardsPlayer = true;
        MinPlayerRadius = Mathf.Infinity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, MinPlayerRadius);
    }
}
