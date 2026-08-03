using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

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
        float speed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat(SpeedHash, speed);

        // IsMoving is true only when input is actively held
        // This means direction switches don't briefly trigger StopWalk
        bool isMoving = Mathf.Abs(moveInput) > 0.01f;
        animator.SetBool(IsMovingHash, isMoving);

        if (groundChecker != null)
            animator.SetBool(IsGroundedHash, groundChecker.IsGrounded);
    }

    private void HandleFlip()
    {
        if (rb.linearVelocity.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (rb.linearVelocity.x < -0.01f)
            spriteRenderer.flipX = true;
    }
}