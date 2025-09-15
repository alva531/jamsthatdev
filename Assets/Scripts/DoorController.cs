using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        animator.SetBool("Open", true);
        animator.SetBool("Close", false);
    }

    public void Close()
    {
        animator.SetBool("Open", false);
        animator.SetBool("Close", true);
    }

    public void StayOpen()
    {
        animator.SetBool("StayOpen", true);
    }
    public void ResetOpen()
    {
        animator.SetBool("StayOpen", false);
    }
}
