using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private int lastCoinCount = -1;

    private void Update()
    {
        if (CoinManager.Instance == null) return;

        // Only update text when count actually changes
        if (CoinManager.Instance.TotalCoins != lastCoinCount)
        {
            lastCoinCount = CoinManager.Instance.TotalCoins;
            coinText.text = $"Coins: {lastCoinCount}";
        }
    }
}