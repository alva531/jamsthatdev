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
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
            }
        }
    }
}