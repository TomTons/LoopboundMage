using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private float spawnDelay = 0f;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private List<GameObject> activeSlimes = new List<GameObject>();
    private bool isFirstSpawn = true;
    private bool isSpawning = false; // prevents double spawn

    private void Start()
    {
        if (PlayerSpawner.Instance != null)
        {
            PlayerSpawner.Instance.OnPlayerDied += HandlePlayerDied;
            PlayerSpawner.Instance.OnPlayerSpawned += HandlePlayerSpawned;
            Debug.Log("[SlimeSpawner] Hooked into PlayerSpawner events");
        }
        else
        {
            Debug.LogWarning("[SlimeSpawner] No PlayerSpawner found — spawning immediately");
        }

        if (spawnDelay <= 0f)
            SpawnAll();
        else
            StartCoroutine(SpawnWithDelay());
    }

    private void OnDestroy()
    {
        if (PlayerSpawner.Instance != null)
        {
            PlayerSpawner.Instance.OnPlayerDied -= HandlePlayerDied;
            PlayerSpawner.Instance.OnPlayerSpawned -= HandlePlayerSpawned;
        }
    }

    // ---------- EVENT HANDLERS ----------

    private void HandlePlayerDied()
    {
        // Reset spawning flag so next spawn is allowed
        isSpawning = false;
        Debug.Log("[SlimeSpawner] Player died — despawning all slimes");
        DespawnAll();
    }

    private void HandlePlayerSpawned()
    {
        // Skip first spawn — already handled in Start()
        if (isFirstSpawn)
        {
            isFirstSpawn = false;
            Debug.Log("[SlimeSpawner] First spawn event skipped");
            return;
        }

        // Prevent double spawn if already spawning
        if (isSpawning)
        {
            Debug.Log("[SlimeSpawner] Already spawning — skipped duplicate event");
            return;
        }

        Debug.Log("[SlimeSpawner] Player respawned — spawning slimes");

        if (spawnDelay <= 0f)
            SpawnAll();
        else
            StartCoroutine(SpawnWithDelay());
    }

    // ---------- SPAWN ----------

    private void SpawnAll()
    {
        // Guard against double spawn
        if (isSpawning)
        {
            Debug.Log("[SlimeSpawner] SpawnAll blocked — already spawning");
            return;
        }

        isSpawning = true;

        Debug.Log($"[SlimeSpawner] SpawnAll called — spawn points: {(spawnPoints != null ? spawnPoints.Length : 0)}");

        if (slimePrefab == null)
        {
            Debug.LogWarning("[SlimeSpawner] No slime prefab assigned!");
            isSpawning = false;
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Spawn(transform.position);
            return;
        }

        foreach (Transform point in spawnPoints)
        {
            if (point != null)
                Spawn(point.position);
            else
                Debug.LogWarning("[SlimeSpawner] Spawn point is null — skipping");
        }
    }

    private void Spawn(Vector3 position)
    {
        GameObject slime = Instantiate(slimePrefab, position, Quaternion.identity);
        activeSlimes.Add(slime);
        Debug.Log($"[SlimeSpawner] Spawned slime at {position} | active: {activeSlimes.Count}");
    }

    // ---------- DESPAWN ----------

    private void DespawnAll()
    {
        activeSlimes.RemoveAll(s => s == null);

        foreach (GameObject slime in activeSlimes)
        {
            if (slime != null)
                Destroy(slime);
        }

        activeSlimes.Clear();
        Debug.Log("[SlimeSpawner] All slimes despawned");
    }

    private IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnAll();
    }

    // ---------- PUBLIC ----------

    public void SpawnAtPoint(int pointIndex)
    {
        if (spawnPoints == null || pointIndex >= spawnPoints.Length)
        {
            Debug.LogWarning("[SlimeSpawner] Spawn point index out of range!");
            return;
        }

        Spawn(spawnPoints[pointIndex].position);
    }

    // ---------- GIZMOS ----------

    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        Gizmos.color = Color.green;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.3f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}