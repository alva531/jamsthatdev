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

    public SpriteRenderer spriteRenderer;
    private Animator _animator;

    private InputActions inputActions;
    private Vector2 moveInput;

    void Awake()
    {
        inputActions = new InputActions();

        // Vincular acción de movimiento
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator = GetComponentInChildren<Animator>();
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

            if (horizontalInput > 0.01f)
            {
                spriteRenderer.flipX = false;
            }
            else if (horizontalInput < -0.01f)
            {
                spriteRenderer.flipX = true;
            }

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
            rb.velocity = Vector2.zero;
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}