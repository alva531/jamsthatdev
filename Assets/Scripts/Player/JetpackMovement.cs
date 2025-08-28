using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetpackMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float pushAmount;
    [SerializeField] private float maxSpeed;
    [SerializeField] private JetpackParticle jetpackParticle;

    public SoundController soundController;

    public SpriteRenderer spriteRenderer;
    private Animator _animator;

    private Vector2 moveInput;

    void Start()
    {
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
        soundController = GameObject.FindWithTag("SoundController").GetComponent<SoundController>();
    }

    public void SetInputVector(Vector2 input)
    {
        bool wasMoving = moveInput != Vector2.zero;
        bool isMoving = input != Vector2.zero;

        moveInput = input;

        if (!wasMoving && isMoving)
            soundController.SetAirSFXActive(true);
        else if (wasMoving && !isMoving)
            soundController.SetAirSFXActive(false);
    }

    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y;

        Vector2 force = new Vector2(horizontalInput, verticalInput).normalized * pushAmount;

        if (force.magnitude > 0.01f)
        {
            rb.AddForce(force);
            jetpackParticle.PSEmission(force);
            spriteRenderer.flipX = horizontalInput < -0.01f;

            // if (horizontalInput < -0.01f)
            // {
            //     spriteRenderer.flipX = true;
            // }
            // else if (horizontalInput > 0.01f)
            // {
            //     spriteRenderer.flipX = false;
            // }

            if (verticalInput > 0.01f)
                _animator.SetTrigger("Up");
            else if (verticalInput < -0.01f)
                _animator.SetTrigger("Down");
        }
        else
        {
            jetpackParticle.PSStopEmission();
            _animator.SetTrigger("Idle");
        }

        if (rb.velocity.magnitude < 0.01f)
        {
            //rb.velocity = Vector2.zero; xxTomiAcuxx Fix
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            float relativeSpeed = other.relativeVelocity.magnitude;
            soundController.ThudSFX(relativeSpeed);

            if (relativeSpeed > 0.01f)
            {
                Vector2 knockbackDir = other.contacts[0].normal;
                rb.AddForce(knockbackDir * 0.05f, ForceMode2D.Impulse);

                //StartCoroutine(ImpactSquash(relativeSpeed, knockbackDir));
            }
        }
    }

    

    // ChatGPT esto es exagerado y espameable 

    private IEnumerator ImpactSquash(float impactForce, Vector2 hitNormal)
    {
        // El squash depende de la fuerza del golpe, con un límite
        float squashAmount = Mathf.Clamp(impactForce * 0.1f, 0.1f, 0.4f);

        // Si el impacto fue lateral, aplastamos en X, si fue vertical, en Y
        bool lateralHit = Mathf.Abs(hitNormal.x) > Mathf.Abs(hitNormal.y);

        Vector3 targetScale = lateralHit
            ? new Vector3(1f - squashAmount, 1f + squashAmount, 1f) // aplasta en X
            : new Vector3(1f + squashAmount, 1f - squashAmount, 1f); // aplasta en Y

        Vector3 originalScale = transform.localScale;

        float t = 0f;
        float squashDuration = 0.15f;

        // Ir hacia el squash
        while (t < 1f)
        {
            t += Time.deltaTime / squashDuration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Volver al tamaño original
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / squashDuration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }
    }
}