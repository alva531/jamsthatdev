using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackParticle : MonoBehaviour
{
    public ParticleSystem jetpackParticles;

    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        emission = jetpackParticles.emission;
        jetpackParticles.Play();
    }

    public void PSEmission(Vector2 inputDir)
    {
        if (inputDir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(inputDir.y, inputDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
            emission.enabled = true;
        }
        else
        {
            emission.enabled = false;
        }
    }

    public void PSStopEmission()
    {
        emission.enabled = false;
    }
}
