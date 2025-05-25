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

    [Header("Input Settings")]
    public string actionMapName = "Player1";

    private InputActions inputActions;
    private InputAction grabAction;

    private GameObject heldObject;
    private int _direction;

    private void Awake()
    {
        inputActions = new InputActions();

        inputActions.asset.FindActionMap(actionMapName).Enable();

        grabAction = inputActions.asset.FindActionMap(actionMapName).FindAction("Grab");
    }

    private void OnEnable()
    {
        grabAction?.Enable();
    }

    private void OnDisable()
    {
        grabAction?.Disable();
    }

    void Update()
    {
        _direction = jetpackMovement.spriteRenderer.flipX ? -1 : 1;
        Vector2 baseDirection = Vector2.right * _direction;
        float halfAngle = angle / 2f;

        if (heldObject == null && grabAction.IsPressed())
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

                if (hit.collider != null && hit.collider.CompareTag("Grabbable"))
                {
                    heldObject = hit.collider.gameObject;

                    FixedJoint2D joint = heldObject.GetComponent<FixedJoint2D>();
                    joint.connectedBody = GetComponent<Rigidbody2D>();
                    joint.enabled = true;

                    var anim = heldObject.GetComponentInChildren<Animator>();
                    if (anim != null) anim.SetBool("Grab", true);
                    GetComponent<JetpackMovement>().soundController.BoxGrabSFX();
                    break;
                }
            }
        }

        if (heldObject != null && !grabAction.IsPressed())
        {
            FixedJoint2D joint = heldObject.GetComponent<FixedJoint2D>();
            joint.enabled = false;
            joint.connectedBody = null;

            var anim = heldObject.GetComponentInChildren<Animator>();
            if (anim != null) anim.SetBool("Grab", false);

            heldObject = null;
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