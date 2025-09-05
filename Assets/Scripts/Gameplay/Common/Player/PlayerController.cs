using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerController : MonoBehaviour
{
    // ----------------------
    //
    // Class for controlling the player and switching in-between their different modes
    //
    // ----------------------

    // Toggleables
    [Header("General Settings")]
    public int Health;
    public float Float_HurtForce;
    public float Platform_HurtForce;
    public float HurtInvincibilityTime;
    public float HurtRegenTime;
    public Color HurtColor;
    public float SpriteRotationSpeed;

    [Space(10)]
    [Header("Floating Physics")]
    public PhysicsMaterial2D Float_PhysicsMaterial;
    public float Float_MoveForce;
    public float Float_MaxMoveSpeed;
    [Space(10)]
    public float Float_DashMoveForce;
    public float Float_DashMoveVelocity;
    public float Float_DashTurnTime;
    public float Float_DashMinTime;
    public float Float_DashMaxTime;
    public float Float_DashDelayTime;
    public float Float_DashColSize;
    [Space(10)]
    public float Float_PauseDrag;

    [Space(15)]
    [Header("Platformer Physics")]
    public PhysicsMaterial2D Platform_PhysicsMaterial;
    public Transform Platform_GroundCheck;
    public float Platform_MoveForce;
    public float Platform_MaxMoveSpeed;
    public float Platform_MidairMoveForce;
    public float Platform_SlopeMoveForce;
    public float Platform_MaxSlopeAngle;
    [Space(10)]
    public float Platform_GravityScale;
    public float Platform_MaxJumpStartForce;
    public float Platform_MaxJumpForce;
    public float Platform_MaxJumpTime;
    public float Platform_GroundMargin;
    public float Platform_CoyoteTime;
    public float Platform_StompForce;

    [Space(10)]
    [Header("Particles")]
    public ParticleSystem GravityModeParticles;

    // Core physics variables
    // Common
    [HideInInspector] public int maxHealth;
    [HideInInspector] public bool isInvincible;
    [HideInInspector] public bool isInSpeedBooster;
    // Float
    [HideInInspector] public bool isDashOnCooldown;
    [HideInInspector] public float colSizeCache;
    [HideInInspector] public float rbDragCache;
    // Platform
    [HideInInspector] public RigidbodyConstraints2D rbConstraintsCache;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isOnSlope;
    [HideInInspector] public Vector2 currentSlopeDirection;
    [HideInInspector] public float prevSlopeAngle;
    [HideInInspector] public float currentSlopeAngle;
    [HideInInspector] public bool noLongerJumping;
    [HideInInspector] public bool isInCoyoteTime;
    [HideInInspector] public bool canStomp;
    [HideInInspector] public int stompAttempts;
    [HideInInspector] public float currentJumpTime;

    // Input variables
    [HideInInspector] public bool isInputEnabled = true;

    [HideInInspector] public Vector2 prevMoveInput = Vector2.zero;
    [HideInInspector] public Vector2 moveInput = Vector2.zero;
    [HideInInspector] public bool actionInput1Start = false;
    [HideInInspector] public bool actionInput1End = false;
    [HideInInspector] public bool actionInput2Start = false;
    [HideInInspector] public bool actionInput2End = false;
    [HideInInspector] public bool pauseInput = false;

    // State machine variables
    public enum PlayerModes
    {
        Float,
        Platform
    }
    [HideInInspector] public PlayerModes currentPlayerMode;

    // Event variables
    public static UnityEvent<int> UpdateHealthUI = new UnityEvent<int>();

    // Component/object references
    [HideInInspector] public PlayerInput input;
    [HideInInspector] public ObjectStateMachine stateMachine;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public PlayerTail tail;
    [HideInInspector] public Animator anim;

    private PauseController pause;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        stateMachine = GetComponent<ObjectStateMachine>();
        rb = GetComponent<Rigidbody2D>();
        col = rb.GetComponent<CircleCollider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        tail = GetComponentInChildren<PlayerTail>();
        anim = sr.GetComponent<Animator>();

        pause = FindFirstObjectByType<PauseController>();
    }

    private void Start()
    {
        maxHealth = Health;
        UpdateHealthUI.Invoke(Health);

        isInputEnabled = true;

        rbDragCache = rb.drag;
        colSizeCache = col.radius;
        rbConstraintsCache = rb.constraints;

        GravityModeParticles.gameObject.SetActive(false);
    }

    //  ---------------------------------------------
    //
    //  Input logic
    //
    //  ---------------------------------------------

    public void OnMovement(InputAction.CallbackContext value)
    {
        // Regular movement for all player states
        Vector2 originalInput = value.ReadValue<Vector2>();
        Vector2 truncatedInput = new Vector2(
            Mathf.Round(originalInput.x * 10) / 10, 
            Mathf.Round(originalInput.y * 10) / 10);

        moveInput = isInputEnabled ? truncatedInput : Vector2.zero;
    }

    public void OnPrimaryAction(InputAction.CallbackContext value)
    {
        switch (currentPlayerMode)
        {
            // Dashing, while in "floating mode"
            case PlayerModes.Float:
                actionInput1Start = isInputEnabled ? value.performed : false;
                actionInput1End = isInputEnabled ? value.ReadValue<float>() <= 0f : false;
                break;

            // Jumping, while in "platforming mode"
            case PlayerModes.Platform:
                if (isGrounded || canStomp || isInCoyoteTime)
                {
                    actionInput1Start = isInputEnabled ? value.ReadValue<float>() > 0f : false;
                }
                else
                {
                    actionInput1Start = false;
                }
                actionInput1End = isInputEnabled ? value.ReadValue<float>() <= 0f : false;

                break;
        }
    }

    public void OnSecondaryAction(InputAction.CallbackContext value)
    {
        actionInput2Start = isInputEnabled ? value.performed : false;
        actionInput2End = isInputEnabled ? value.ReadValue<float>() <= 0f : false;
    }

    public void OnPause(InputAction.CallbackContext value)
    {
        pauseInput = value.performed;
    }

    //  ---------------------------------------------
    //
    //  Internal player logic
    //
    //  ---------------------------------------------

    private void Update()
    {
        if (pauseInput)
        {
            if (stateMachine.CurrentState.GetType() != typeof(PlayerDeathState))
            {
                pause.PauseGame();
                pauseInput = false;
            }
        }

        UpdateSpriteRotation();
        UpdateAnimVariables();

        // TO-DO: move particle visual handling to a separate script later
        GravityModeParticles.transform.eulerAngles = new Vector3(0, 0, LevelController.Instance.UpAngle);
    }

    private void FixedUpdate()
    {
        switch (currentPlayerMode)
        {
            default:
            case PlayerModes.Float:
                if (moveInput != Vector2.zero)
                {
                    prevMoveInput = moveInput;
                }
                break;

            case PlayerModes.Platform:
                break;
        }

        UpdateRotation();
        UpdatePhysics();
        CrushCheck();
    }

    // Snap the player's rotation to align with that of the level
    private void UpdateRotation()
    {
        transform.eulerAngles = new Vector3(0, 0, LevelController.Instance.UpAngle);
    }

    // Update the player's general physics between different modes
    private void UpdatePhysics()
    {
        switch (currentPlayerMode)
        {
            default:
            case PlayerModes.Float:
                break;

            case PlayerModes.Platform:
                rb.AddForce((LevelController.Instance.Gravity * Platform_GravityScale));

                CheckForGround();
                // Keep the player smoothly walking along the ground if they are on the slope
                if (moveInput.x != 0 && isOnSlope)
                {
                    //Debug.DrawLine(rb.position, rb.position + slopeForceDir);
                    rb.AddForce(currentSlopeDirection * (Platform_SlopeMoveForce * Time.fixedDeltaTime));
                }
                break;
        }
    }

    private void HurtPlayer(Vector2 hurtSource)
    {
        if (!isInvincible)
        {
            if (Health > 1)
            {
                rb.velocity = Vector2.zero;
                switch (currentPlayerMode)
                {
                    case PlayerModes.Float:
                        rb.AddForce(
                            (rb.position - hurtSource).normalized * Float_HurtForce,
                            ForceMode2D.Impulse);
                        stateMachine.ChangeState(typeof(PlayerFloatMoveState));
                        break;

                    case PlayerModes.Platform:
                        rb.AddForce(
                            (rb.position - hurtSource).normalized * Platform_HurtForce,
                            ForceMode2D.Impulse);
                        stateMachine.ChangeState(typeof(PlayerPlatformMidairMoveState));
                        break;
                }
                
                Health--;
                anim.SetTrigger("HitDamage");
                StartCoroutine(InvincibilityTimer());
            }
            else
            {
                KillPlayer();
            }

            UpdateHealthUI.Invoke(Health);
        }
    }

    private void KillPlayer()
    {
        Health = 0;
        stateMachine.ChangeState(typeof(PlayerDeathState));
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        sr.color = HurtColor;
        tail.lr.startColor = HurtColor;
        tail.lr.endColor = HurtColor;

        yield return new WaitForSeconds(HurtInvincibilityTime);

        isInvincible = false;
        sr.color = Color.white;
        tail.lr.startColor = Color.white;
        tail.lr.endColor = Color.white;
        StopCoroutine(InvincibilityTimer());
        StartCoroutine(HealthRegenTimer());
    }

    private IEnumerator HealthRegenTimer()
    {
        yield return new WaitForSeconds(HurtRegenTime);

        if (Health < maxHealth)
        {
            Health++;
            UpdateHealthUI.Invoke(Health);
        }
        else
        {
            StopCoroutine(HealthRegenTimer());
        }
    }

    // If another object's collision overlaps with the centre point of the player, crush and kill them
    private void CrushCheck()
    {
        if (rb.simulated && col.enabled)
        {
            Collider2D[] crushCheck = Physics2D.OverlapCircleAll(rb.position, col.radius);
            foreach (Collider2D hit in crushCheck)
            {
                if (!hit.isTrigger && !hit.usedByEffector)
                {
                    if (hit.gameObject.GetComponent<PlayerController>() != null
                    || hit.gameObject.GetComponent<Rigidbody2D>() == null)
                    {
                        continue;
                    }
                    else
                    {
                        if ((hit.ClosestPoint(rb.position) - rb.position).sqrMagnitude == 0)
                        {
                            stateMachine.ChangeState(typeof(PlayerDeathState));
                            break;
                        }
                    }
                }
            }
        }
    }

    // Generic collision handling for level objects
    private void OnCollisionEnter2D(Collision2D collision)
    {
        anim.SetTrigger("HitWall");
        
        if (collision.gameObject.CompareTag("Harmful"))
        {
            HurtPlayer(collision.GetContact(0).point);
        }

        if (collision.gameObject.CompareTag("Death"))
        {
            KillPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Harmful"))
        {
            HurtPlayer(collision.transform.position);
        }

        if (collision.gameObject.CompareTag("Death"))
        {
            KillPlayer();
        }

        if (collision.gameObject.CompareTag("SpeedBooster"))
        {
            isInSpeedBooster = true;
        }
    }

    // Foreground collision checking
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Harmful"))
        {
            HurtPlayer(collision.transform.position);
        }

        if (collision.gameObject.CompareTag("Death"))
        {
            KillPlayer();
        }
    }

    // Foreground collision disabling, upon leaving the ground
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            isGrounded = false;
            currentSlopeAngle = 0;
            currentSlopeDirection = Vector2.zero;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SpeedBooster"))
        {
            isInSpeedBooster = false;
        }
    }

    private void CheckForGround()
    {
        RaycastHit2D[] groundCheck = Physics2D.CircleCastAll(rb.position, col.radius, LevelController.Instance.Gravity.normalized);
        isGrounded = false;

        foreach (RaycastHit2D hit in groundCheck)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
            {
                if (hit.distance <= col.radius + Platform_GroundMargin)
                {
                    float angle = Vector2.Angle(hit.normal, LevelController.Instance.UpAxis);

                    if (angle <= Platform_MaxSlopeAngle)
                    {
                        isGrounded = true;
                        CheckForSlope(hit);

                        break;
                    }
                }
            }
            else
            {
                continue;
            }
        }

        if (!isGrounded)
        {
            currentSlopeAngle = 0;
            currentSlopeDirection = Vector2.zero;
        }
    }

    // Check if the player is moving along a slope, and adjust their target velocity accordingly
    private void CheckForSlope(RaycastHit2D groundHit)
    {
        Vector2 slopeTangent = new Vector2(groundHit.normal.y, -groundHit.normal.x).normalized;

        if (moveInput.x < 0)
        {
            currentSlopeDirection = -slopeTangent;
        }
        else if (moveInput.x > 0)
        {
            currentSlopeDirection = slopeTangent;
        }
        else
        {
            currentSlopeDirection = Vector2.zero;
        }
        currentSlopeAngle = Vector2.Angle(groundHit.normal, LevelController.Instance.UpAxis);

        if (Mathf.Abs(currentSlopeAngle - prevSlopeAngle) > 0.01f)
        {
            isOnSlope = true;
        }

        //Debug.DrawLine(rb.position, rb.position + currentSlopeDirection);
        prevSlopeAngle = currentSlopeAngle;
    }

    // Update the rotation of the player's sprite for animations
    private void UpdateSpriteRotation()
    {
        if (rb.velocity != Vector2.zero)
        {
            Quaternion targetRotation;

            if (currentPlayerMode == PlayerModes.Float || (currentPlayerMode == PlayerModes.Platform && !isGrounded))
            {
                Vector3 direction = rb.velocity.normalized * -1;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            else
            {
                targetRotation = Quaternion.AngleAxis(LevelController.Instance.UpAngle - 90, Vector3.forward);
            }

            sr.transform.rotation = Quaternion.Slerp(sr.transform.rotation, targetRotation, SpriteRotationSpeed * Time.deltaTime);
        }
    }

    // Update the parameter variables inside the player's Animator
    private void UpdateAnimVariables()
    {
        switch (currentPlayerMode)
        {
            case PlayerModes.Float:
                break;

            case PlayerModes.Platform:
                anim.SetInteger("Platform_MoveDir", Mathf.RoundToInt(moveInput.x));
                anim.SetBool("Platform_IsGrounded", isGrounded);
                break;
        }
    }

    //  ---------------------------------------------
    //
    //  Publically-accessible player functions (for other objects to use)
    //
    //  ---------------------------------------------

    public void SetPosition(Vector2 pos)
    {
        transform.position = pos;
        tail.ResetTail();
    }

    public void SwitchModes(PlayerModes newMode)
    {
        switch (newMode)
        {
            // Switch to floating mode
            case PlayerModes.Float:
                rb.sharedMaterial = Float_PhysicsMaterial;
                GravityModeParticles.gameObject.SetActive(false);
                anim.SetInteger("PlayerState", 0);
                stateMachine.ChangeState(typeof(PlayerFloatIdleState));
                break;
            
            // Switch to platforming mode
            case PlayerModes.Platform:
                rb.sharedMaterial = Platform_PhysicsMaterial;
                GravityModeParticles.gameObject.SetActive(true);
                anim.SetInteger("PlayerState", 1);
                stateMachine.ChangeState(typeof(PlayerPlatformMidairMoveState));

                stompAttempts = 0;
                canStomp = true;

                break;
        }

        actionInput1Start = false;
        rb.drag = rbDragCache;

        currentPlayerMode = newMode;
    }
}
