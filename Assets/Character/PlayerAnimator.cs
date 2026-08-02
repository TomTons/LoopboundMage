using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Animator parameter hashes (faster than passing strings every frame)
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private Rigidbody2D rb;
    private IGroundChecker groundChecker;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponent<IGroundChecker>();

        // Auto-grab if not assigned in Inspector
        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        HandleAnimations();
        HandleFlip();
    }

    private void HandleAnimations()
    {
        // Pass absolute horizontal speed to the Animator
        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat(SpeedHash, speed);

        // Pass grounded state
        if (groundChecker != null)
            animator.SetBool(IsGroundedHash, groundChecker.IsGrounded);
    }

    private void HandleFlip()
    {
        // Flip sprite based on horizontal velocity direction
        if (rb.linearVelocity.x > 0.01f)
            spriteRenderer.flipX = false; // facing right
        else if (rb.linearVelocity.x < -0.01f)
            spriteRenderer.flipX = true;  // facing left
    }
}
