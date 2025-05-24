using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float pushAmount;

    [SerializeField] private JetpackParticle jetpackParticle;

    [Header("Input")]
    public float horizontallInput;
    public float verticalInput;



    // Start is called before the first frame update
    void Start()
    {
        
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
    }
}
