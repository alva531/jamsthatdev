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

    public float distance = 1f;
    public LayerMask grabbableMask;

    GameObject box;

    // Update is called once per frame
    void Update()
    {

        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, grabbableMask);

        if (hit.collider != null && hit.collider.gameObject.tag == "Grabbable" && Input.GetKeyDown(KeyCode.Space))
        {
            box = hit.collider.gameObject;
            box.GetComponent<FixedJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();
            box.GetComponent<FixedJoint2D>().enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            box.GetComponent<FixedJoint2D>().enabled = false;
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * distance);
    }

}
