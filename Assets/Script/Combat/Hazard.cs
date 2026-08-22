using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth == null) return;

        // Work out which direction the hit came from using the contact normal
        Vector2 hitDirection = GetHitDirection(collision);

        Debug.Log($"[Hazard] Hit: {collision.gameObject.name} — dealing {damageAmount} damage | hit direction: {hitDirection}");

        playerHealth.TakeDamage(damageAmount, hitDirection);
    }

    private Vector2 GetHitDirection(Collision2D collision)
    {
        // Average all contact normals to get the overall hit direction
        Vector2 direction = Vector2.zero;

        foreach (ContactPoint2D contact in collision.contacts)
            direction += contact.normal;

        return direction.normalized;
    }
}