using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Upgrade Costs")]
    [SerializeField] private int fireballDamageCost = 20;
    [SerializeField] private int maxHealthCost = 30;

    [Header("Upgrade Values")]
    [SerializeField] private int fireballDamageIncrease = 5;
    [SerializeField] private int maxHealthIncrease = 1;

    // Current upgrade totals
    public int BonusFireballDamage { get; private set; }
    public int BonusMaxHealth { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[UpgradeManager] Initialized");
    }

    public bool UpgradeFireballDamage()
    {
        if (CoinManager.Instance == null) return false;

        if (!CoinManager.Instance.TrySpendCoins(fireballDamageCost))
        {
            Debug.Log("[UpgradeManager] Not enough coins for fireball upgrade");
            return false;
        }

        BonusFireballDamage += fireballDamageIncrease;
        Debug.Log($"[UpgradeManager] Fireball damage upgraded — bonus: {BonusFireballDamage}");
        return true;
    }

    public bool UpgradeMaxHealth()
    {
        if (CoinManager.Instance == null) return false;

        if (!CoinManager.Instance.TrySpendCoins(maxHealthCost))
        {
            Debug.Log("[UpgradeManager] Not enough coins for health upgrade");
            return false;
        }

        BonusMaxHealth += maxHealthIncrease;
        Debug.Log($"[UpgradeManager] Max health upgraded — bonus: {BonusMaxHealth}");
        return true;
    }
}