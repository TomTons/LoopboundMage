using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float respawnDelay = 2f;
    public float RespawnDelay => respawnDelay;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int defaultSpawnIndex = 0;

    private int currentSpawnIndex = 0;
    private GameObject currentPlayer;

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
        SpawnPlayer();
    }

    // ---------- SPAWN ----------

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("[PlayerSpawner] No player prefab assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[PlayerSpawner] No spawn points assigned — spawning at spawner position");
            currentPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Vector3 spawnPos = spawnPoints[currentSpawnIndex].position;
            currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[PlayerSpawner] Spawned player at point {currentSpawnIndex}: {spawnPos}");
        }

        // Hook into player death event
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.OnPlayerDied += HandlePlayerDied;
    }

    private void HandlePlayerDied()
    {
        Debug.Log($"[PlayerSpawner] Player died — respawning in {respawnDelay}s");
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnPlayer();
        Debug.Log("[PlayerSpawner] Player respawned");
    }

    // ---------- PUBLIC ----------

    // Call this from a checkpoint to update the spawn point
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