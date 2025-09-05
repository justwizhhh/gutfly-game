using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBlock : BaseLevelObject
{
    // ----------------------
    //
    // This obstacle object can only be destroyed when the player dashes through it
    //
    // ----------------------

    [Space(10)]
    public bool IsPlayerDashing;
    public bool IsPlayerStunned;
    public float PlayerStunTime;

    [Space(10)]
    public Collectible InsideCollectible;
    public float InsideCollectibleReleaseForce;
    public float InsideCollectibleReleaseTime;

    [Space(10)]
    public Sprite[] RockTextures;

    private Collectible insideCollectibleInstance;
    private GameObject particles;
    private int foregroundLayer;

    // Object/component references
    private PlayerController player;

    public override void Awake()
    {
        base.Awake();
        player = FindFirstObjectByType<PlayerController>();
        particles = transform.GetChild(0).gameObject;
        particles.SetActive(false);
    }

    public override void Start()
    {
        base.Start();
        if (InsideCollectible != null)
        {
            Collectible newCollectible = Instantiate(InsideCollectible);

            newCollectible.gameObject.SetActive(false);
            newCollectible.transform.position = transform.position;
            newCollectible.MinPlayerRadius = 0;

            insideCollectibleInstance = newCollectible;
        }

        foregroundLayer = LayerMask.NameToLayer("Foreground");

        sr.sprite = RockTextures[Random.Range(0, RockTextures.Length - 1)];
        //transform.eulerAngles = new Vector3(0, 0, 90 * Random.Range(0, 3));
    }

    public override void Update()
    {
        bool canDestroy;
        
        if (IsPlayerDashing)
        {
            canDestroy = (player.stateMachine.CurrentState.GetType() == typeof(PlayerFloatDashState)
                || player.stompAttempts > 0);
        }
        else
        {
            canDestroy = true;
        }

        col.isTrigger = canDestroy;
        gameObject.layer = canDestroy ? 0 : foregroundLayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollide(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnCollide(collision);
    }

    private void OnCollide(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            // If the rock is storing an object, push it out with a force upon release
            if (insideCollectibleInstance != null)
            {
                insideCollectibleInstance.public_col.enabled = false;
                insideCollectibleInstance.gameObject.SetActive(true);

                insideCollectibleInstance.public_rb.AddForce(
                    ((Vector2)transform.position - player.rb.position).normalized * InsideCollectibleReleaseForce,
                    ForceMode2D.Impulse);

                StartCoroutine(ActivateInsideCollectible());
            }

            // Temporarily pause the player's movement, for an added impact effect upon collision
            if (IsPlayerStunned)
            {
                StartCoroutine(StunTimer());
            }

            // If the player is in Platform mode, allow them to keep moving downwards
            if (player.currentPlayerMode == PlayerController.PlayerModes.Platform)
            {
                player.stompAttempts++;
            }

            particles.SetActive(true);
            transform.DetachChildren();
            particles.transform.localScale = Vector3.one;

            rb.simulated = false;
            col.enabled = false;
            sr.enabled = false;
        }
    }

    private IEnumerator StunTimer()
    {
        player.rb.constraints = RigidbodyConstraints2D.FreezePosition;

        Vector2 velocityCache = player.rb.velocity;
        //player.rb.position -= velocityCache * Time.deltaTime;

        yield return new WaitForSeconds(PlayerStunTime);

        player.rb.constraints = player.rbConstraintsCache;
        player.rb.velocity = velocityCache;

        if (player.stompAttempts > 0)
        {
            // Re-apply stomp force, if the player was previously stomping
            player.rb.AddForce(
                        LevelController.Instance.Gravity.normalized * player.Platform_StompForce,
                        ForceMode2D.Impulse);
        }
        else
        {
            // Otherwise, reapply dash force
            player.rb.velocity = -velocityCache.normalized * player.Float_DashMoveVelocity * Time.fixedDeltaTime;
        }

        StopCoroutine(StunTimer());
    }

    private IEnumerator ActivateInsideCollectible()
    {
        yield return new WaitForSeconds(InsideCollectibleReleaseTime);
        
        insideCollectibleInstance.ForceFollowPlayer();
        insideCollectibleInstance.public_col.enabled = true;
    }
}
