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
    }
}
