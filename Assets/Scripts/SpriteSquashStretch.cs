using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSquashStretch : MonoBehaviour
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

    void Start()
    {
        baseScale = transform.localScale;
        baseRotation = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
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
}