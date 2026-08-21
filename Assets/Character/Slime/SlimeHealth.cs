using UnityEngine;

public class SlimeHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    public bool IsDead { get; private set; }

    private Animator animator;
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"[SlimeHealth] Took {amount} damage — {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        IsDead = true;
        animator.SetBool(IsDeadHash, true);
        Debug.Log("[SlimeHealth] Slime died");

        // Destroy after dead animation finishes (adjust time to match clip length)
        Destroy(gameObject, 1.5f);
    }
}