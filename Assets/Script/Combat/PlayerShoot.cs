using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.3f;

    [Header("References")]

    private PlayerControls controls;
    private float fireRateTimer;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Shoot.performed += OnShoot;
    }

    private void OnDisable()
    {
        controls.Player.Shoot.performed -= OnShoot;
        controls.Player.Disable();
    }

    private void Update()
    {
        if (fireRateTimer > 0f)
            fireRateTimer -= Time.deltaTime;
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("[Shoot] No fireball prefab assigned!");
            return;
        }

        if (fireRateTimer > 0f)
        {
            Debug.Log($"[Shoot] On cooldown — {fireRateTimer:F2}s left");
            return;
        }

        SpawnFireball();
    }

    private void SpawnFireball()
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

        Fireball fireballScript = fireball.GetComponent<Fireball>();

        fireRateTimer = fireRate;
    }
}