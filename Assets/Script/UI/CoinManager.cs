using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    public int TotalCoins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log($"[CoinManager] +{amount} coins — Total: {TotalCoins}");
    }

    // Returns true if successful, false if not enough coins
    public bool TrySpendCoins(int amount)
    {
        if (TotalCoins < amount)
        {
            Debug.Log($"[CoinManager] Not enough coins — have {TotalCoins}, need {amount}");
            return false;
        }

        TotalCoins -= amount;
        Debug.Log($"[CoinManager] -{amount} coins — Total: {TotalCoins}");
        return true;
    }
}