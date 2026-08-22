using UnityEngine;

public class Slime_Behaviour : MonoBehaviour
{
    #region Variables
    [Header("Detection Settings")]
    [SerializeField] private Transform rayCastOrigin;
    [SerializeField] private LayerMask rayCastMask;
    [SerializeField] private float rayCastLength = 5f;
    [SerializeField] private float attackDistance = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;

    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance;
    private float cooldownTimer;

    private bool inRange;
    private bool isAttacking;
    private bool isCooling;

    private float facingDirection = 1f;
    #endregion

    private void Awake()
    {
        anim = GetComponent<Animator>();
        cooldownTimer = attackCooldown;
        facingDirection = 1f; // adjust to match your sprite default
    }

    private void Update()
    {
        if (target == null)
        {
            inRange = false;
        }

        if (inRange)
        {
            Vector2 directionToPlayer = (target.transform.position - rayCastOrigin.position).normalized;
            hit = Physics2D.Raycast(rayCastOrigin.position, directionToPlayer, rayCastLength, rayCastMask);

            Debug.DrawRay(rayCastOrigin.position, directionToPlayer * rayCastLength,
                hit.collider != null ? Color.green : Color.red);
        }

        // Count down cooldown
        if (isCooling)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isCooling = false;
                isAttacking = false; // fully reset attack state after cooldown
                cooldownTimer = attackCooldown;
                anim.SetBool(IsAttackingHash, false);
                Debug.Log("[Slime] Cooldown finished — ready to move again");
            }
        }

        if (inRange && hit.collider != null)
        {
            EnemyLogic();
        }
        else
        {
            Idle();
        }
    }

    private void EnemyLogic()
    {
        if (target == null) return;

        distance = Vector2.Distance(transform.position, target.transform.position);

        // While attacking or cooling down, do nothing — wait it out
        if (isAttacking || isCooling)
        {
            anim.SetBool(IsWalkingHash, false);
            return;
        }

        if (distance > attackDistance)
        {
            Move();
        }
        else
        {
            Attack();
        }
    }

    private void Move()
    {
        anim.SetBool(IsWalkingHash, true);
        anim.SetBool(IsAttackingHash, false);

        Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Flip toward player only when direction changes
        float newDirection = target.transform.position.x > transform.position.x ? -1f : 1f;
        if (newDirection != facingDirection)
        {
            facingDirection = newDirection;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingDirection;
            transform.localScale = scale;
        }
    }

    private void Attack()
    {
        isAttacking = true;
        isCooling = true;
        cooldownTimer = attackCooldown;

        anim.SetBool(IsWalkingHash, false);
        anim.SetBool(IsAttackingHash, true);

        Debug.Log("[Slime] Attacking!");
    }

    private void Idle()
    {
        anim.SetBool(IsWalkingHash, false);
        anim.SetBool(IsAttackingHash, false);
    }

    // Called by Animation Event on last frame of SlimeAttack clip
    public void OnAttackFinished()
    {
        anim.SetBool(IsAttackingHash, false);
        Debug.Log("[Slime] Attack animation finished");
    }

    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            target = trig.gameObject;
            inRange = true;
            Debug.Log("[Slime] Player entered range");
        }
    }

    private void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("Player"))
        {
            inRange = false;
            target = null;
            isAttacking = false;
            isCooling = false;
            Idle();
            Debug.Log("[Slime] Player left range");
        }
    }
}