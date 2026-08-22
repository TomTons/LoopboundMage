using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float lifetime = 3f;

    private float direction = 1f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Apply damage upgrades from UpgradeManager if it exists
        int bonusDamage = UpgradeManager.Instance != null ? UpgradeManager.Instance.BonusFireballDamage : 0;
        damage += bonusDamage;

        Destroy(gameObject, lifetime);
        Debug.Log($"[Fireball] Damage: {damage} (bonus: {bonusDamage})");
    }

    public void SetDirection(float dir)
    {
        direction = dir;

        // Flip sprite to match direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;

        if (rb != null)
            rb.linearVelocity = new Vector2(direction * speed, 0f);

        Debug.Log($"[Fireball] Direction: {(direction > 0 ? "Right" : "Left")} | velocity: {rb.linearVelocity}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[Fireball] Hit: {collision.gameObject.name}");

        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(damage);

        Destroy(gameObject);
    }
}