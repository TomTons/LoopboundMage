using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private int spawnCount = 1;
    [SerializeField] private float spawnDelay = 0f; // 0 = instant on scene load

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints; // drag empty GameObjects in here

    private void Start()
    {
        if (spawnDelay <= 0f)
            SpawnAll();
        else
            Invoke(nameof(SpawnAll), spawnDelay);
    }

    private void SpawnAll()
    {
        if (slimePrefab == null)
        {
            Debug.LogWarning("[SlimeSpawner] No slime prefab assigned!");
            return;
        }

        // If no spawn points assigned, just spawn at this object's position
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Spawn(transform.position);
            }
            return;
        }

        // Spawn one slime per spawn point
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
                Spawn(point.position);
        }
    }

    private void Spawn(Vector3 position)
    {
        GameObject slime = Instantiate(slimePrefab, position, Quaternion.identity);
        Debug.Log($"[SlimeSpawner] Spawned slime at {position}");
    }

    // Optional: call this from other scripts to manually trigger a spawn
    public void SpawnAtPoint(int pointIndex)
    {
        if (pointIndex >= spawnPoints.Length)
        {
            Debug.LogWarning("[SlimeSpawner] Spawn point index out of range!");
            return;
        }

        Spawn(spawnPoints[pointIndex].position);
    }

    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;

        // Draw a green sphere at each spawn point in the Scene view
        Gizmos.color = Color.green;
        foreach (Transform point in spawnPoints)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, 0.3f);
        }

        // Draw the spawner itself in yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}