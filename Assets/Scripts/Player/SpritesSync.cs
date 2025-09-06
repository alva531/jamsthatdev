using System.Collections.Generic;
using UnityEngine;

public class SpritesSync : MonoBehaviour
{
    [Header("Velocidad del Player")]
    [SerializeField] Rigidbody2D rb;

    [Header("Squash & Stretch")]
    [SerializeField] float stretchFactor = 0.3f;
    [SerializeField] float squashFactor = 0.2f;
    [SerializeField] float lerpSpeed = 8f;

    [Header("Tilt / Inclinación")]
    [SerializeField] float maxTilt = 15f;

    private Vector3 baseScale;
    private float baseRotation;

    private SpriteRenderer parentRenderer;
    private Animator parentAnimator;

    private List<SpriteRenderer> childSprites = new List<SpriteRenderer>();
    private List<Animator> childAnimators = new List<Animator>();
    private List<AnimatorControllerParameter> parameters = new List<AnimatorControllerParameter>();

    void Start()
    {
        baseScale = transform.localScale;
        baseRotation = transform.rotation.eulerAngles.z;

        parentRenderer = GetComponent<SpriteRenderer>();
        parentAnimator = GetComponent<Animator>();

        SpriteRenderer[] allSprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in allSprites)
        {
            if (sr != parentRenderer) childSprites.Add(sr);
        }

        Animator[] allAnimators = GetComponentsInChildren<Animator>(true);
        foreach (var anim in allAnimators)
        {
            if (anim != parentAnimator) childAnimators.Add(anim);
        }

        if (parentAnimator != null)
            parameters.AddRange(parentAnimator.parameters);
    }

    void Update()
    {
        ApplySquashStretch();
        SyncChildren();
        SyncAnimators();
    }

    void ApplySquashStretch()
    {
        if (rb == null) return;

        Vector2 velocity = rb.velocity;
        float speed = velocity.magnitude;

        float stretch = 1f + (speed * stretchFactor * 0.1f);
        float squash = 1f - (speed * squashFactor * 0.05f);

        if (speed < 0.1f && rb.velocity.sqrMagnitude > 0.001f)
        {
            stretch = 1.1f;
            squash = 0.9f;
        }

        Vector3 targetScale = new Vector3(baseScale.x * squash, baseScale.y * stretch, baseScale.z);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * lerpSpeed);

        float direction = Mathf.Sign(velocity.x);
        float targetRotation = (Mathf.Abs(velocity.x) > 0.1f) ? maxTilt * -direction : baseRotation;

        Quaternion rot = Quaternion.Euler(0, 0, targetRotation);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lerpSpeed);
    }

    void SyncChildren()
    {
        if (parentRenderer == null) return;

        foreach (var child in childSprites)
        {
            if (child == null) continue;
            child.flipX = parentRenderer.flipX;
            child.flipY = parentRenderer.flipY;
        }
    }

    void SyncAnimators()
    {
        if (parentAnimator == null || childAnimators.Count == 0) return;

        foreach (var p in parameters)
        {
            switch (p.type)
            {
                case AnimatorControllerParameterType.Bool:
                    bool b = parentAnimator.GetBool(p.nameHash);
                    foreach (var anim in childAnimators) anim.SetBool(p.nameHash, b);
                    break;

                case AnimatorControllerParameterType.Float:
                    float f = parentAnimator.GetFloat(p.nameHash);
                    foreach (var anim in childAnimators) anim.SetFloat(p.nameHash, f);
                    break;

                case AnimatorControllerParameterType.Int:
                    int i = parentAnimator.GetInteger(p.nameHash);
                    foreach (var anim in childAnimators) anim.SetInteger(p.nameHash, i);
                    break;

                case AnimatorControllerParameterType.Trigger:
                    if (parentAnimator.GetBool(p.nameHash))
                    {
                        foreach (var anim in childAnimators) anim.SetTrigger(p.nameHash);
                    }
                    break;
            }
        }
    }
}
