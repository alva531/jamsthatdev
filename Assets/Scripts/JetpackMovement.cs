using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float pushAmount;
    [SerializeField] private float maxSpeed;

    [SerializeField] private JetpackParticle jetpackParticle;

    [Header("Input")]
    private float horizontallInput;
    private float verticalInput;

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
            jetpackParticle.PSEmission();
        }
        else if (horizontallInput < -0.01f)
        {
            rb.AddForce(Vector2.left * pushAmount);
            jetpackParticle.PSEmission();
        }


        if (verticalInput > 0.01f)
        {
            rb.AddForce(Vector2.up * pushAmount);
            jetpackParticle.PSEmission();
        }
        else if (verticalInput < -0.01f)
        {
            rb.AddForce(Vector2.down * pushAmount);
            jetpackParticle.PSEmission();
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
