using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("Invincibility Settings")]
    [SerializeField] private float invincibilityDuration = 1.5f;
    [SerializeField] private float flashInterval = 0.1f; // how fast the sprite flashes during iframes

    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;
    private float flashTimer;
    private bool isVisible;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Debug.Log($"[Health] Initialized — {currentHealth}/{maxHealth}");
    }

    private void Update()
    {
        if (!isInvincible) return;

        // Count down iframes
        invincibilityTimer -= Time.deltaTime;

        // Flash the sprite
        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f)
        {
            isVisible = !isVisible;
            if (spriteRenderer != null)
                spriteRenderer.enabled = isVisible;
            flashTimer = flashInterval;
        }

        // Iframes expired
        if (invincibilityTimer <= 0f)
        {
            isInvincible = false;

            // Make sure sprite is visible again when iframes end
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;

            Debug.Log("[Health] Invincibility ended");
        }
    }

    public void TakeDamage(int amount)
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
        // Make sure sprite is visible on death (no frozen mid-flash state)
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        Debug.Log("[Health] Player is dead — destroying");
        Destroy(gameObject);
    }

    // Public getter in case other scripts (e.g. UI) need to read health/iframes
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsInvincible => isInvincible;
}