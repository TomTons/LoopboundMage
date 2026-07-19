using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    [SerializeField] private Transform spriteObject; // drag the Sprite child here
    [SerializeField] private Transform firePoint;    // drag FirePoint here

    [SerializeField] private float firePointOffsetX = 0.5f; // distance from center

    public float FacingDirection { get; private set; } = 1f;

    public void SetFacing(float direction)
    {
        if (Mathf.Abs(direction) < 0.01f) return;
        if (Mathf.Sign(direction) == Mathf.Sign(FacingDirection)) return;

        FacingDirection = Mathf.Sign(direction);

        // Flip only the sprite child, not the root player
        if (spriteObject != null)
        {
            Vector3 scale = spriteObject.localScale;
            scale.x = Mathf.Abs(scale.x) * FacingDirection;
            spriteObject.localScale = scale;
        }

        // Explicitly move FirePoint to the correct side
        if (firePoint != null)
        {
            Vector3 fp = firePoint.localPosition;
            fp.x = firePointOffsetX * FacingDirection;
            firePoint.localPosition = fp;
            Debug.Log($"[Facing] Flipped to: {(FacingDirection > 0 ? "Right" : "Left")} | FirePoint X: {fp.x}");
        }
    }
}