using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotController : MonoBehaviour
{

    public bool Dirt = false;
    public bool Water = false;
    public bool Seed = false;

    Animator _animator;

    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dirt") && Dirt == false && Seed == false && Water == false)
        {
            Dirt = true;
            other.gameObject.GetComponent<BoxController>().Used();
            _animator.SetTrigger("Dirt");
        }

        if (other.gameObject.CompareTag("Seed") && Dirt == true && Seed == false && Water == false)
        {
            Seed = true;
            other.gameObject.GetComponent<BoxController>().Used();
            _animator.SetTrigger("Seed");
        }

        if (other.gameObject.CompareTag("Water") && Dirt == true && Seed == true && Water == false)
        {
            Water = true;
            other.gameObject.GetComponent<BoxController>().Used();
            _animator.SetTrigger("Water");
        }
    }
}
