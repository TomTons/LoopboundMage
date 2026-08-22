using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public int TotalCoins { get; private set; }

    private void Awake()
    {
        // Singleton — only one exists, survives scene changes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[CoinManager] Initialized");
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"[CoinManager] +{amount} coins — Total: {TotalCoins}");
    }

    public void SpendCoins(int amount)
    {
        TotalCoins = Mathf.Max(0, TotalCoins - amount);
        Debug.Log($"[CoinManager] -{amount} coins — Total: {TotalCoins}");
    }
}