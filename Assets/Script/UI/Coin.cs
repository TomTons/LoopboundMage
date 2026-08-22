using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private float bobSpeed = 2f;       // how fast it bobs up and down
    [SerializeField] private float bobHeight = 0.1f;    // how high it bobs
    [SerializeField] private float lifetime = 10f;      // auto destroy if not picked up

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Bob up and down so it's visible
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(coinValue);

        Debug.Log($"[Coin] Collected — value: {coinValue}");
        Destroy(gameObject);
    }
}