using UnityEngine;

public class ParticlePointFlip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;

    private Vector3 baseLocalPos;

    void Start()
    {
        baseLocalPos = transform.localPosition;

        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInParent<SpriteRenderer>();
        }
    }

    void LateUpdate()
    {
        if (targetRenderer == null) return;

        transform.localPosition = new Vector3(
            targetRenderer.flipX ? -baseLocalPos.x : baseLocalPos.x,
            baseLocalPos.y,
            baseLocalPos.z
        );
    }
}