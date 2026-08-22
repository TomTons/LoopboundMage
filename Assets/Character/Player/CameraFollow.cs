using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    private CinemachineCamera cinemachineCamera;

    private void Awake()
    {
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }

    private void Start()
    {
        // Hook into respawn event for future spawns
        if (PlayerSpawner.Instance != null)
        {
            PlayerSpawner.Instance.OnPlayerSpawned += FindAndFollowPlayer;
            Debug.Log("[CameraFollow] Hooked into PlayerSpawner");
        }

        // Find player directly on start instead of waiting for event
        // This handles the first spawn since the event fires before we subscribe
        FindAndFollowPlayer();
    }

    private void OnDestroy()
    {
        if (PlayerSpawner.Instance != null)
            PlayerSpawner.Instance.OnPlayerSpawned -= FindAndFollowPlayer;
    }

    private void FindAndFollowPlayer()
    {
        StartCoroutine(SetFollowTarget());
    }

    private IEnumerator SetFollowTarget()
    {
        // Wait one frame for player to fully initialize
        yield return null;

        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            cinemachineCamera.Follow = player.transform;
            Debug.Log("[CameraFollow] Now following: " + player.name);
        }
        else
        {
            Debug.LogWarning("[CameraFollow] No player found with tag 'Player'");
        }
    }
}