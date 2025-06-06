using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public int uses = 4;

    void FixedUpdate()
    {
        if (uses <= 0)
        {
            Destroy(transform.parent.gameObject);
        }
    }

    public void Used()
    {
        uses = uses - 1;
    }

}
