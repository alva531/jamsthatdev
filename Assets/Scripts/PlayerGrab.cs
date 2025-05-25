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

  public float distance = 3f;// Largo de los rayos
  public float angle = 60f;// Ángulo total del cono
  public int rayCount = 8;// Cantidad de rayos
  public LayerMask grabbableMask;
  public Transform rayOrigin;// Objeto hijo del jugador que lanza los rayos

  private GameObject detectedObject;

  public JetpackMovement jetpackMovement;

  private int _direction;

  void Update()
  {
    // Dirección base del jugador (izquierda o derecha)
    _direction = jetpackMovement.spriteRenderer.flipX ? -1 : 1;

    Vector2 baseDirection = Vector2.right * _direction;
    

    // Ángulo de inicio (negativo la mitad del total)
    float halfAngle = angle / 2f;

    // Reset detección
    detectedObject = null;

    for (int i = 0; i < rayCount; i++)
    {
      // Interpolar ángulos desde -halfAngle hasta +halfAngle
      float t = i / (float)(rayCount - 1);
      float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
      float finalAngle = currentAngle * Mathf.Deg2Rad;

      // Rotar dirección base según el ángulo
      Vector2 dir = new Vector2(
          baseDirection.x * Mathf.Cos(finalAngle) - baseDirection.y * Mathf.Sin(finalAngle),
          baseDirection.x * Mathf.Sin(finalAngle) + baseDirection.y * Mathf.Cos(finalAngle)
      );

      RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, dir, distance, grabbableMask);
      Debug.DrawRay(rayOrigin.position, dir * distance, Color.red);

      if (hit.collider != null && hit.collider.CompareTag("Grabbable"))
      {
        detectedObject = hit.collider.gameObject;
        // Opcional: salirse al encontrar uno
        break;
      }
    }

    // Interacción con el objeto
    if (detectedObject != null && Input.GetKeyDown(KeyCode.Space))
    {
      FixedJoint2D joint = detectedObject.GetComponent<FixedJoint2D>();
      joint.connectedBody = GetComponent<Rigidbody2D>();
      joint.enabled = true;
    }

    if (Input.GetKeyUp(KeyCode.Space) && detectedObject != null)
    {
      FixedJoint2D joint = detectedObject.GetComponent<FixedJoint2D>();
      joint.enabled = false;
      joint.connectedBody = null;
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
}