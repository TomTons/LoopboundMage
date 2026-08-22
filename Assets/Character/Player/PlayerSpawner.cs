using UnityEngine;
using System;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float respawnDelay = 2f;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int defaultSpawnIndex = 0;

    public event Action OnPlayerDied;
    public event Action OnPlayerSpawned;

    public float RespawnDelay => respawnDelay;

    private int currentSpawnIndex = 0;
    private GameObject currentPlayer;
    private bool isSpawning = false; // prevents double spawn

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        currentSpawnIndex = defaultSpawnIndex;
        SpawnPlayer();
    }

    // ---------- SPAWN ----------

    private void SpawnPlayer()
    {
        // Guard against double spawn
        if (isSpawning)
        {
            Debug.Log("[PlayerSpawner] Already spawning — blocked duplicate call");
            return;
        }

        isSpawning = true;

        if (playerPrefab == null)
        {
            Debug.LogWarning("[PlayerSpawner] No player prefab assigned!");
            isSpawning = false;
            return;
        }

        // Destroy existing player if still alive (e.g. NPC reset)
        if (currentPlayer != null)
        {
            Debug.Log("[PlayerSpawner] Destroying existing player before respawn");
            Destroy(currentPlayer);
            currentPlayer = null;
        }

        Vector3 spawnPos = spawnPoints != null && spawnPoints.Length > 0
            ? spawnPoints[currentSpawnIndex].position
            : transform.position;

        currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // Hook into player death
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.OnPlayerDied += HandlePlayerDied;

        OnPlayerSpawned?.Invoke();

        Debug.Log($"[PlayerSpawner] Player spawned at {spawnPos}");
    }

    private void HandlePlayerDied()
    {
        if (currentPlayer != null)
        {
            currentPlayer = null;
        }

        // Reset spawning flag so respawn is allowed
        isSpawning = false;

        Debug.Log($"[PlayerSpawner] Player died — respawning in {respawnDelay}s");
        OnPlayerDied?.Invoke();

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnPlayer();
    }

    // ---------- PUBLIC ----------

    public void SetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
        {
            currentSpawnIndex = index;
            Debug.Log($"[PlayerSpawner] Spawn point updated to index {index}");
        }
    }

    // ---------- GIZMOS ----------

    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] == null) continue;
            Gizmos.color = i == currentSpawnIndex ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(spawnPoints[i].position, 0.3f);
        }
    }
}