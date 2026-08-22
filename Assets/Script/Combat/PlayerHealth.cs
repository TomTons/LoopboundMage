using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashInterval = 0.1f;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForceX = 8f;
    [SerializeField] private float knockbackForceY = 5f;
    [SerializeField] private float knockbackDuration = 0.2f;

    // Fired when player dies — PlayerSpawner listens to this
    public event Action OnPlayerDied;

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
        // Apply health upgrades from UpgradeManager if it exists
        int bonusHealth = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusMaxHealth : 0;
        maxHealth += bonusHealth;
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        Debug.Log($"[Health] Initialized — {currentHealth}/{maxHealth} (bonus: {bonusHealth})");
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
        if (amount <= 0 || isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Health] Took {amount} damage — {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartInvincibility();
        if (hitDirection != Vector2.zero)
            ApplyKnockback(hitDirection);
    }

    // ---------- PRIVATE ----------

    private void Die()
    {
        Debug.Log("[Health] Player died");

        // Fire death event before destroying
        OnPlayerDied?.Invoke();

        Destroy(gameObject);
    }

    private void ApplyKnockback(Vector2 hitDirection)
    {
        if (rb == null) return;

        float knockbackDirection = Mathf.Sign(-hitDirection.x);

        rb.linearVelocity = new Vector2(
            knockbackDirection * knockbackForceX,
            knockbackForceY
        );

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        Debug.Log($"[Knockback] Direction: {(knockbackDirection > 0 ? "Right" : "Left")} | Force X: {knockbackForceX} | Force Y: {knockbackForceY}");
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

    private void StartInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        flashTimer = flashInterval;
        isVisible = true;
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
        }
    }
    
    public void ForceKill()
    {
        currentHealth = 0;
        Die();
    }
}