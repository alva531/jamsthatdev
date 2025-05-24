using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabTrigger : MonoBehaviour
{
    public PlayerGrab grabSystem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Grabbable"))
        {
            grabSystem.canGrab = true;
            grabSystem.grabbableTarget = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Grabbable"))
        {
            grabSystem.canGrab = false;
            grabSystem.grabbableTarget = null;
        }
    }
}