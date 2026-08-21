using UnityEngine;

public class SlimeHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    private Collider2D hitboxCollider;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.enabled = false; // off by default
    }

    // Called by Animation Event when attack hitbox should wbe active
    public void EnableHitbox()
    {
        hitboxCollider.enabled = true;
        Debug.Log("[SlimeHitbox] Enabled");
    }

    // Called by Animation Event when attack hitbox should deactivate
    public void DisableHitbox()
    {
        hitboxCollider.enabled = false;
        Debug.Log("[SlimeHitbox] Disabled");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitboxCollider.enabled) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log($"[SlimeHitbox] Hit: {other.gameObject.name} for {damage}");
            damageable.TakeDamage(damage);
        }
    }
}