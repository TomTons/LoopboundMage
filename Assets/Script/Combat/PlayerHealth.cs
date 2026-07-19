using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForceX = 40f;
    [SerializeField] private float knockbackForceY = 40f;
    [SerializeField] private float knockbackDuration = 0.2f;

    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;
    private float flashTimer;
    private bool isVisible;

    private bool isKnockedBack;
    private float knockbackTimer;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
    public bool IsKnockedBack => isKnockedBack;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Debug.Log($"[Health] Initialized — {currentHealth}/{maxHealth}");
    }

    private void Update()
    {
        HandleInvincibility();
        HandleKnockback();
    }

    // ---------- PUBLIC ----------
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Vector2.zero);
    }
    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        if (amount <= 0) return;

        if (isInvincible)
        {
            Debug.Log("[Health] Damage ignored — player is invincible");
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Health] Took {amount} damage — {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            DestroyPlayer();
            return;
        }

        StartInvincibility();
        ApplyKnockback(hitDirection);
    }

    // ---------- PRIVATE ----------

    private void ApplyKnockback(Vector2 hitDirection)
    {
        if (rb == null) return;

        // Opposite direction from where the hit came from
        Vector2 knockbackDirection = new Vector2(-hitDirection.x, 1f).normalized;
        Vector2 knockbackVelocity = new Vector2(
            knockbackDirection.x * knockbackForceX,
            knockbackForceY
        );

        rb.linearVelocity = knockbackVelocity;
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        Debug.Log($"[Knockback] Direction: {knockbackDirection} | Velocity: {knockbackVelocity}");
    }

    private void HandleKnockback()
    {
        if (!isKnockedBack) return;

        knockbackTimer -= Time.deltaTime;

        if (knockbackTimer <= 0f)
        {
            isKnockedBack = false;
            Debug.Log("[Knockback] Ended");
        }
    }

    private void HandleInvincibility()
    {
        if (!isInvincible) return;

        invincibilityTimer -= Time.deltaTime;

        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f)
        {
            isVisible = !isVisible;
            if (spriteRenderer != null)
                spriteRenderer.enabled = isVisible;
            flashTimer = flashInterval;
        }

        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
            Debug.Log("[Health] Invincibility ended");
        }
    }

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        flashTimer = flashInterval;
        isVisible = true;
        Debug.Log($"[Health] Invincibility started — {invincibilityDuration}s");
    }

    private void DestroyPlayer()
    {
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;
        Debug.Log("[Health] Player is dead — destroying");
        Destroy(gameObject);
    }
}