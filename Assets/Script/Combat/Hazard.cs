using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            Debug.Log($"[Hazard] Hit: {collision.gameObject.name} — dealing {damageAmount} damage");
            playerHealth.TakeDamage(damageAmount);
        }
    }
}