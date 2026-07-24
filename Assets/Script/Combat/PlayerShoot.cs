using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float firePointOffsetX = 0.5f;

    private PlayerControls controls;
    private float fireRateTimer;
    private float facingDirection = 1f;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Shoot.performed += OnShoot;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Shoot.performed -= OnShoot;
        controls.Player.Disable();
    }

    private void Update()
    {
        if (fireRateTimer > 0f)
            fireRateTimer -= Time.deltaTime;

        // Move firePoint in front of player based on facing direction
        if (firePoint != null)
            firePoint.localPosition = new Vector3(firePointOffsetX * facingDirection, firePoint.localPosition.y, 0f);
    }

    // ---------- INPUT CALLBACKS ----------

    private void OnMove(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        // Only update facing when actually moving, not on release
        if (Mathf.Abs(value) > 0.01f)
        {
            facingDirection = Mathf.Sign(value);
            Debug.Log($"[Shoot] Facing: {(facingDirection > 0 ? "Right" : "Left")}");
        }
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

    // ---------- SPAWN ----------

    private void SpawnFireball()
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;

        GameObject fireball = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

        Fireball fireballScript = fireball.GetComponent<Fireball>();
        if (fireballScript != null)
            fireballScript.SetDirection(facingDirection);

        fireRateTimer = fireRate;
        Debug.Log($"[Shoot] Fired — direction: {(facingDirection > 0 ? "Right" : "Left")}");
    }
}