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

    [Header("Input Settings")]
    public string actionMapName = "Player1"; // o "Player2" si es el segundo jugador

    private InputActions inputActions;
    private InputAction moveAction;
    private Vector2 moveInput;

    void Awake()
    {
        inputActions = new InputActions();

        // Activar Action Map correcto
        inputActions.asset.FindActionMap(actionMapName).Enable();

        // Buscar acción "Move"
        moveAction = inputActions.asset.FindActionMap(actionMapName).FindAction("Move");

        // Registrar eventos de input
        moveAction.started += ctx => OnJetpackInput(true);
        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx =>
        {
            moveInput = Vector2.zero;
            OnJetpackInput(false);
        };
    }

    private void OnJetpackInput(bool isPressed)
    {
        soundController.SetAirSFXActive(isPressed);
    }

    void OnEnable()
    {
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
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
            rb.velocity = Vector2.zero;
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
        }
    }
}