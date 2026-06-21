using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, IGroundChecker
{
    [SerializeField] private LayerMask groundLayer;

    private int groundContactCount = 0;

    public bool IsGrounded => groundContactCount > 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject) && IsContactFromBelow(collision))
        {
            groundContactCount++;
            Debug.Log($"[GroundChecker] Entered ground: {collision.gameObject.name} | contacts: {groundContactCount}");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (IsGroundLayer(collision.gameObject))
        {
            groundContactCount = Mathf.Max(0, groundContactCount - 1);
            Debug.Log($"[GroundChecker] Exited ground: {collision.gameObject.name} | contacts: {groundContactCount}");
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        return ((1 << obj.layer) & groundLayer) != 0;
    }

    private bool IsContactFromBelow(Collision2D collision)
    {
        // Checks that at least one contact normal points upward (player landed ON TOP of ground, not hit a wall)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
                return true;
        }
        return false;
    }
}