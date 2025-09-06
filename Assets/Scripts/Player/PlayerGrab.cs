using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Grab Settings")]
    public float distance = 3f;
    public float angle = 60f;
    public int rayCount = 8;
    public LayerMask grabbableMask;
    public Transform rayOrigin;
    public JetpackMovement jetpackMovement;

    private GameObject heldObject;
    private int _direction;
    private bool isGrabbing;

    // Para saber si lo que agarramos es palanca
    private LeverController heldLever;

    public void TryGrab(bool isPressed)
    {
        isGrabbing = isPressed;
    }

    void FixedUpdate()
    {
        _direction = jetpackMovement.spriteRenderer.flipX ? -1 : 1;
        Vector2 baseDirection = Vector2.right * _direction;
        float halfAngle = angle / 2f;

        if (heldObject == null && isGrabbing)
        {
            for (int i = 0; i < rayCount; i++)
            {
                float t = i / (float)(rayCount - 1);
                float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
                float finalAngle = currentAngle * Mathf.Deg2Rad;

                Vector2 dir = new Vector2(
                    baseDirection.x * Mathf.Cos(finalAngle) - baseDirection.y * Mathf.Sin(finalAngle),
                    baseDirection.x * Mathf.Sin(finalAngle) + baseDirection.y * Mathf.Cos(finalAngle)
                );

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, dir, distance, grabbableMask);
                Debug.DrawRay(rayOrigin.position, dir * distance, Color.red);

                if (hit.collider != null)
                {
                    // ---- Grabbable (cajas) ----
                    if (hit.collider.CompareTag("Grabbable"))
                    {
                        heldObject = hit.collider.gameObject;
                        heldLever = null;

                        FixedJoint2D joint = heldObject.GetComponent<FixedJoint2D>();
                        joint.connectedBody = GetComponent<Rigidbody2D>();
                        joint.enabled = true;

                        Rigidbody2D heldRb = heldObject.GetComponent<Rigidbody2D>();
                        Rigidbody2D playerRb = GetComponent<Rigidbody2D>();
                        heldRb.velocity = playerRb.velocity;

                        var anim = heldObject.GetComponentInChildren<Animator>();
                        if (anim != null) anim.SetBool("Grab", true);

                        jetpackMovement.soundController.BoxGrabSFX();
                        break;
                    }

                    // ---- Lever (palancas) ----
                    if (hit.collider.CompareTag("Lever"))
                    {
                        heldObject = hit.collider.gameObject;
                        heldLever = heldObject.GetComponent<LeverController>();

                        if (heldLever != null)
                        {
                            heldLever.Grab(transform);   // SpringJoint2D en la palanca se conecta al player
                            jetpackMovement.soundController.BoxGrabSFX();

                            var anim = heldObject.GetComponentInChildren<Animator>();
                            if (anim != null) anim.SetBool("Grab", true);
                        }

                        break;
                    }
                }
            }
        }

        // ---- Soltar ----
        if (heldObject != null && !isGrabbing)
        {
            if (heldObject.CompareTag("Grabbable"))
            {
                FixedJoint2D joint = heldObject.GetComponent<FixedJoint2D>();
                joint.enabled = false;
                joint.connectedBody = null;

                var anim = heldObject.GetComponentInChildren<Animator>();
                if (anim != null) anim.SetBool("Grab", false);
            }
           else if (heldObject.CompareTag("Lever") && heldLever != null)
            {
                heldLever.Release();   // ✅ ahora liberamos el control
                var anim = heldObject.GetComponentInChildren<Animator>();
                if (anim != null) anim.SetBool("Grab", false);
            }

            heldObject = null;
            heldLever = null;
        }
    }

    void OnDrawGizmos()
    {
        if (rayOrigin == null) return;

        int direction = (transform.localScale.x >= 0) ? 1 : -1;
        Vector2 baseDirection = Vector2.right * direction;
        float halfAngle = angle / 2f;

        for (int i = 0; i < rayCount; i++)
        {
            float t = i / (float)(rayCount - 1);
            float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
            float finalAngle = currentAngle * Mathf.Deg2Rad;

            Vector2 dir = new Vector2(
                baseDirection.x * Mathf.Cos(finalAngle) - baseDirection.y * Mathf.Sin(finalAngle),
                baseDirection.x * Mathf.Sin(finalAngle) + baseDirection.y * Mathf.Cos(finalAngle)
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayOrigin.position, rayOrigin.position + (Vector3)(dir * distance));
        }
    }
}