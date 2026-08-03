using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float fallMultiplier = 2.2f;
    [SerializeField] private float lowJumpMultiplier = 7f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float moveLockDuration = 0.3f;

    [Header("References")]
    [SerializeField] private GroundChecker groundChecker;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerAnimator playerAnimator;

    private Rigidbody2D rb;
    private PlayerControls controls;
    private IGroundChecker groundCheck;

    private float moveInput;
    private bool jumpHeld;
    private bool jumpRequested;

    private bool isDashing;
    private float dashTimer;
    private float dashCooldownTimer;
    private float dashDirection;

    private bool moveInputLocked;
    private float moveLockTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();
        groundCheck = groundChecker;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJumpPressed;
        controls.Player.Jump.canceled += OnJumpReleased;
        controls.Player.Dash.performed += OnDashPressed;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Jump.performed -= OnJumpPressed;
        controls.Player.Jump.canceled -= OnJumpReleased;
        controls.Player.Dash.performed -= OnDashPressed;
        controls.Player.Disable();
    }

    // ---------- INPUT CALLBACKS ----------

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();

        if (playerAnimator != null)
            playerAnimator.SetMoveInput(moveInput);
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        jumpHeld = true;

        if (groundCheck.IsGrounded)
        {
            jumpRequested = true;
            Debug.Log("[Jump] PRESSED while grounded — jump requested");
        }
        else
        {
            Debug.Log("[Jump] PRESSED while NOT grounded — ignored");
        }
    }

    private void OnJumpReleased(InputAction.CallbackContext context)
    {
        jumpHeld = false;
    }

    private void OnDashPressed(InputAction.CallbackContext context)
    {
        float direction = context.ReadValue<float>();

        if (isDashing)
        {
            Debug.Log("[Dash] Ignored — already dashing");
            return;
        }

        if (dashCooldownTimer > 0f)
        {
            Debug.Log($"[Dash] Ignored — on cooldown ({dashCooldownTimer:F2}s left)");
            return;
        }

        dashDirection = direction; // -1 = left (Q), +1 = right (E)
        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;

        // Check if currently held movement input opposes the dash direction
        float currentMoveInput = controls.Player.Move.ReadValue<float>();
        bool isOpposingDirection = currentMoveInput != 0f && Mathf.Sign(currentMoveInput) != Mathf.Sign(dashDirection);

        if (isOpposingDirection)
        {
            moveInputLocked = true;
            moveLockTimer = moveLockDuration;
            moveInput = 0f;
            Debug.Log($"[Dash] STARTED — direction: {dashDirection} | opposing input detected ({currentMoveInput}) — movement locked for {moveLockDuration}s");
        }
        else
        {
            // Same direction or no input held — let movement continue uninterrupted
            moveInput = currentMoveInput;
            Debug.Log($"[Dash] STARTED — direction: {dashDirection} | input matches or idle ({currentMoveInput}) — no lock");
        }
    }

    // ---------- UPDATE LOOP ----------

    private void Update()
    {
        if (moveInputLocked)
        {
            moveLockTimer -= Time.deltaTime;

            if (moveLockTimer <= 0f)
            {
                moveInputLocked = false;
                moveInput = controls.Player.Move.ReadValue<float>();
                Debug.Log($"[Move] Movement input unlocked — resumed value: {moveInput}");
            }
        }
    }

    // ---------- PHYSICS LOOP ----------
    
    private void FixedUpdate()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.fixedDeltaTime;

        // Don't override velocity while being knocked back
        if (playerHealth != null && playerHealth.IsKnockedBack) return;

        if (isDashing)
        {
            HandleDash();
        }
        else
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            if (jumpRequested)
            {
                Debug.Log("[HandleJump] Jumping!");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpRequested = false;
            }

            HandleFallGravity();
        }
    }

    private void HandleDash()
    {
        dashTimer -= Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        if (dashTimer <= 0f)
        {
            isDashing = false;
            Debug.Log("[Dash] ENDED");
        }
    }

    private void HandleFallGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }
}