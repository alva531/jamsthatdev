using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float pushAmount;
    [SerializeField] private float maxSpeed;

    [SerializeField] private JetpackParticle jetpackParticle;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    [Header("Input")]
    public float horizontallInput;
    public float verticalInput;




    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        _animator = gameObject.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        horizontallInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (horizontallInput > 0.01f)
        {
            rb.AddForce(Vector2.right * pushAmount);
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            jetpackParticle.PSEmission(moveInput);
            transform.localScale.x.Equals(1);
            _spriteRenderer.flipX = false;
        }
        else if (horizontallInput < -0.01f)
        {
            rb.AddForce(Vector2.left * pushAmount);
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            jetpackParticle.PSEmission(moveInput);
            _spriteRenderer.flipX = true;
        }


        if (verticalInput > 0.01f)
        {
            rb.AddForce(Vector2.up * pushAmount);
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            jetpackParticle.PSEmission(moveInput);
            _animator.SetTrigger("Up");
        }
        else if (verticalInput < -0.01f)
        {
            rb.AddForce(Vector2.down * pushAmount);
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            jetpackParticle.PSEmission(moveInput);
            _animator.SetTrigger("Down");
        }

        if (verticalInput == 0)
        {
            _animator.SetTrigger("Idle");
        }
        
        if (horizontallInput == 0 && verticalInput == 0)
        {
            jetpackParticle.PSStopEmission();
        }

        if(rb.velocity.magnitude < 0.01)
        {
            rb.velocity = Vector2.zero;
        }
        else if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
