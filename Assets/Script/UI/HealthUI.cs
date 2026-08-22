using UnityEngine;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private PlayerHealth playerHealth;

    private List<HeartUI> hearts = new List<HeartUI>();
    private int lastHealth = -1;

    private void Start()
    {
        BuildHearts();
    }

    private void Update()
    {
        // Only refresh when health actually changes, not every frame
        if (playerHealth.CurrentHealth != lastHealth)
        {
            UpdateHearts();
            lastHealth = playerHealth.CurrentHealth;
        }
    }

    private void BuildHearts()
    {
        // Clear any existing hearts first
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        hearts.Clear();

        // Spawn one heart per max health point
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

        Debug.Log($"[HealthUI] Updated — {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}");
    }
}