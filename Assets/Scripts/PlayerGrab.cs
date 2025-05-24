using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    /*
      [I like to grab stuff]
                    \
                     \
                     .--.
                   .'    `.
                  :  _  _  ;
                .-|  _  _  |-.
               ((_| (O)(O) |_))
                `-|  .--.  |-'
                .-' (    ) `-.
               / .-._`--'_.-. \
              ( (n   uuuu   n) )
               `.`"=nnnnnn="'.'
                 `-.______.-'
                 __/\|  |/\__
              .='w/\ \__/ /\w`=.
            .-\ww(( \/88\/ ))ww/-.
           /  |www\\ \88/ //www|  \
          |   |wwww\\/88\//wwww|   |
          |   |wwwww\\88//wwwww|   |
          |   /wwwwww\\//wwwwww\hjw|
    */

public bool canGrab = false;
    public GameObject grabbableTarget = null;

    private GameObject currentlyGrabbedObject = null;
    private FixedJoint2D currentJoint = null;

    void Update()
    {
        if (canGrab && grabbableTarget != null && Input.GetKey(KeyCode.Space))
        {
            if (currentJoint == null)
            {
                FixedJoint2D joint = grabbableTarget.GetComponent<FixedJoint2D>();
                Rigidbody2D rb = grabbableTarget.GetComponent<Rigidbody2D>();

                rb.velocity = GetComponent<Rigidbody2D>().velocity;
                rb.angularVelocity = 0f;

                joint.connectedBody = GetComponent<Rigidbody2D>();
                joint.enabled = true;

                currentJoint = joint;
                currentlyGrabbedObject = grabbableTarget;
            }
        }
        else
        {
            if (currentJoint != null && currentlyGrabbedObject != null)
            {
                Rigidbody2D rb = currentlyGrabbedObject.GetComponent<Rigidbody2D>();
                rb.velocity = GetComponent<Rigidbody2D>().velocity;

                currentJoint.enabled = false;
                currentJoint.connectedBody = null;

                currentJoint = null;
                currentlyGrabbedObject = null;
            }
        }
    }
}