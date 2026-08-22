using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, IGroundChecker
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private int raycastCount = 3;
    [SerializeField] private float skinWidth = 0.02f; // pulls rays slightly inside collider edge to avoid seam catches

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

        // Pull the origin slightly inside the collider bottom using skinWidth
        // This prevents rays from starting exactly on the edge and catching tile seams
        float originY = bounds.min.y + skinWidth;

        // Shrink the horizontal span slightly inward on both sides
        // so corner rays don't catch the edge of adjacent tiles
        float minX = bounds.min.x + skinWidth;
        float maxX = bounds.max.x - skinWidth;

        int hitCount = 0;

        for (int i = 0; i < raycastCount; i++)
        {
            float t = raycastCount == 1 ? 0.5f : (float)i / (raycastCount - 1);
            float originX = Mathf.Lerp(minX, maxX, t);

            Vector2 origin = new Vector2(originX, originY);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, raycastDistance + skinWidth, groundLayer);

            if (drawDebugRays)
            {
                Color rayColor = hit.collider != null ? Color.green : Color.red;
                Debug.DrawRay(origin, Vector2.down * (raycastDistance + skinWidth), rayColor);
            }

            if (hit.collider != null)
                hitCount++;
        }

        // Require at least 2 rays to hit for grounded to be true
        // Single ray hits are likely seam false positives
        bool grounded = hitCount >= Mathf.Min(2, raycastCount);

        if (isGrounded != grounded)
            Debug.Log($"[GroundChecker] isGrounded changed: {grounded} | hits: {hitCount}/{raycastCount}");

        return grounded;
    }
}