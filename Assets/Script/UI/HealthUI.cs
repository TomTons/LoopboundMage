using UnityEngine;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject heartPrefab;

    private List<HeartUI> hearts = new List<HeartUI>();
    private PlayerHealth playerHealth;
    private int lastHealth = -1;

    private void Start()
    {
        FindAndHookPlayer();
    }

    private void FindAndHookPlayer()
    {
        // Find the player health in the scene dynamically
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning("[HealthUI] No PlayerHealth found in scene — retrying");
            return;
        }

        // Hook into death event so we know when to find the new player
        playerHealth.OnPlayerDied += OnPlayerDied;

        BuildHearts();
        Debug.Log("[HealthUI] Hooked into PlayerHealth");
    }

    private void OnPlayerDied()
    {
        // Unhook old player
        if (playerHealth != null)
            playerHealth.OnPlayerDied -= OnPlayerDied;

        playerHealth = null;
        lastHealth = -1;

        // Wait for spawner to respawn player then re-hook
        Invoke(nameof(FindAndHookPlayer), PlayerSpawner.Instance != null ?
            PlayerSpawner.Instance.RespawnDelay + 0.1f : 2.1f);

        Debug.Log("[HealthUI] Player died — waiting for respawn to re-hook");
    }

    private void Update()
    {
        if (playerHealth == null) return;

        if (playerHealth.CurrentHealth != lastHealth)
        {
            UpdateHearts();
            lastHealth = playerHealth.CurrentHealth;
        }
    }

    private void BuildHearts()
    {
        // Clear old hearts
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        hearts.Clear();

        for (int i = 0; i < playerHealth.MaxHealth; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, transform);
            HeartUI heart = heartObj.GetComponent<HeartUI>();
            hearts.Add(heart);
        }

        UpdateHearts();
        Debug.Log($"[HealthUI] Built {hearts.Count} hearts");
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < playerHealth.CurrentHealth)
                hearts[i].SetFull();
            else
                hearts[i].SetEmpty();
        }
    }
}