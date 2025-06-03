using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObstacleController : MonoBehaviour
{
    private SoundController soundController;

    void Start()
    {
        soundController = GameObject.FindWithTag("SoundController").GetComponent<SoundController>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        soundController.BoxHitSFX();


    if (collision.gameObject.CompareTag("Ship"))
    {
        float relativeSpeed = collision.relativeVelocity.magnitude;
        soundController.ThudSFX(relativeSpeed);

        if (relativeSpeed > 0.01f)
        {
            Vector2 knockbackDir = collision.contacts[0].normal;
            
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            float knockbackForce = 0.03f;
            rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        }
    }

    }
}
