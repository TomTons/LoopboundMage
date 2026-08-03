using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");

    private Rigidbody2D rb;
    private IGroundChecker groundChecker;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GetComponent<IGroundChecker>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetMoveInput(float input)
    {
        moveInput = input;
    }

    private void Update()
    {
        HandleAnimations();
        HandleFlip();
    }

    private void HandleAnimations()
    {
        bool isGrounded = groundChecker != null && groundChecker.IsGrounded;
        bool isMoving = Mathf.Abs(moveInput) > 0.01f;

        // Normalize vertical velocity to -1/1 range for blend tree
        float verticalVelocity = Mathf.Clamp(rb.linearVelocity.y, -1f, 1f);
        float speed = Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat(SpeedHash, speed);
        animator.SetBool(IsGroundedHash, isGrounded);
        animator.SetBool(IsMovingHash, isMoving);
        animator.SetFloat(VerticalVelocityHash, verticalVelocity);

        Debug.Log($"[Anim] grounded: {isGrounded} | verticalVel: {verticalVelocity:F2} | moving: {isMoving}");
    }

    private void HandleFlip()
    {
        if (rb.linearVelocity.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (rb.linearVelocity.x < -0.01f)
            spriteRenderer.flipX = true;
    }
}