using UnityEngine;

public class HazardTrigger : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private string playerTag = "Player";
    public Health health;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            health.TakeDamage(damageAmount);
        }
    }
}