using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackParticle : MonoBehaviour
{
    public Rigidbody2D playerRb;
    public ParticleSystem jetpackParticles;

    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        emission = jetpackParticles.emission;
        jetpackParticles.Play();
    }

    public void PSEmission()
    {
        Vector2 moveDir = playerRb.velocity;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);

        emission.enabled = true;
    }

    public void PSStopEmission()
    {
        emission.enabled = false;
    }
}
