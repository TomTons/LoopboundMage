using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, IGroundChecker
{
    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private int raycastCount = 3;

    [Header("Debug")]
    [SerializeField] private bool drawDebugRays = true;

    private Collider2D col;
    private bool isGrounded;

    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        isGrounded = CheckGrounded();
    }

    private bool CheckGrounded()
    {
        Bounds bounds = col.bounds;
        float originY = bounds.min.y;

        for (int i = 0; i < raycastCount; i++)
        {
            float t = raycastCount == 1 ? 0.5f : (float)i / (raycastCount - 1);
            float originX = Mathf.Lerp(bounds.min.x, bounds.max.x, t);

            Vector2 origin = new Vector2(originX, originY);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance, groundLayer);

            if (drawDebugRays)
            {
                Color rayColor = hit.collider != null ? Color.green : Color.red;
                Debug.DrawRay(origin, Vector2.down * raycastDistance, rayColor);
            }

            if (hit.collider != null)
            {
                Debug.Log($"[GroundChecker] Ray {i} hit ground: {hit.collider.gameObject.name}");
                return true;
            }
        }

        return false;
    }
}