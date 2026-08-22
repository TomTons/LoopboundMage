using UnityEngine;

public class SlimeHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;

    [Header("Coin Drop Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private int minCoinDrop = 1;
    [SerializeField] private int maxCoinDrop = 3;
    [SerializeField] private float dropSpread = 0.5f;

    private int currentHealth;
    private Animator animator;

    // Hashes
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
    private static readonly int IsAttackingHash = Animator.StringToHash("IsAttacking");

    public bool IsDead { get; private set; }

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
        
        animator.SetBool(IsWalkingHash, false);
        animator.SetBool(IsAttackingHash, false);
        animator.SetBool(IsDeadHash, true);

     

        StartCoroutine(CheckAnimationState());
    }

    private System.Collections.IEnumerator CheckAnimationState()
    {
        // Wait one frame
        yield return null;
        

        yield return new WaitForSeconds(1f);
        
        DropCoins();
        Destroy(gameObject);
    }

    private void DropCoins()
    {
        if (coinPrefab == null)
        {
            Debug.LogWarning("[SlimeHealth] No coin prefab assigned!");
            return;
        }

        int coinCount = Random.Range(minCoinDrop, maxCoinDrop + 1);

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-dropSpread, dropSpread),
                Random.Range(0f, dropSpread),
                0f
            );

            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[SlimeHealth] Dropped coin {i + 1}/{coinCount}");
        }
    }
}