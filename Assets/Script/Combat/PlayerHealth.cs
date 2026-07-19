using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
        Debug.Log($"[Health] Initialized — {currentHealth}/{maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"[Health] Took {amount} damage — {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            DestroyPlayer();
    }

    private void DestroyPlayer()
    {
        Debug.Log("[Health] Player is dead — destroying");
        Destroy(gameObject);
    }
}