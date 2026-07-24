using UnityEngine;

public class GroundChecker : MonoBehaviour, IGroundChecker
{
    [Header("Raycast Settings")]
    [SerializeField] private float rayLength = 0.1f;
    [SerializeField] private float raySpacing = 0.2f;
    [SerializeField] private int rayCount = 3;
    [SerializeField] private Vector2 rayOffset;
    [SerializeField] private LayerMask groundLayer;

    public bool IsGrounded { get; private set; }

    private void FixedUpdate()
    {
        IsGrounded = CastRays();
    }

    private bool CastRays()
    {
        Vector2 origin = (Vector2)transform.position + rayOffset;

        float totalWidth = raySpacing * (rayCount - 1);
        float startX = origin.x - totalWidth / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = new Vector2(startX + raySpacing * i, origin.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);

            if (hit.collider != null)
            {
                Debug.Log($"[GroundChecker] Ray {i} hit: {hit.collider.gameObject.name}");
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 origin = (Vector2)transform.position + rayOffset;
        float totalWidth = raySpacing * (rayCount - 1);
        float startX = origin.x - totalWidth / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = new Vector2(startX + raySpacing * i, origin.y);
            Gizmos.color = IsGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector2.down * rayLength);
        }
    }
}