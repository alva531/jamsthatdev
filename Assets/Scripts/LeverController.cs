using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HingeJoint2D))]
public class LeverController : MonoBehaviour
{
    [Header("Eventos")]
    public UnityEvent OnMinReached;
    public UnityEvent OnMaxReached;

    private HingeJoint2D hinge;
    private Rigidbody2D rb;

    [SerializeField] private float tolerance = 2f;
    private bool minInvoked = false;
    private bool maxInvoked = false;

    private Transform player;
    private SpringJoint2D springJoint;
    private bool isGrabbed = false;

    [SerializeField] float rotationSpeed = 180f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hinge = GetComponent<HingeJoint2D>();
    }

    private void FixedUpdate()
    {
        if (isGrabbed && player != null)
        {
            // Dirección del player relativa al pivot de la palanca
            Vector2 localDir = transform.InverseTransformPoint(player.position);

            // Ángulo deseado en grados, invertido
            float targetAngle = -Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

            // Limitamos según los límites del HingeJoint2D
            targetAngle = Mathf.Clamp(targetAngle, hinge.limits.min, hinge.limits.max);

            // Interpolamos suavemente hacia el ángulo deseado
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);

            // Aplicamos rotación suavizada
            rb.MoveRotation(newAngle);
        }

        // Chequeo de eventos
        float currentAngle = transform.localEulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        if (Mathf.Abs(currentAngle - hinge.limits.min) <= tolerance)
        {
            if (!minInvoked)
            {
                OnMinReached.Invoke();
                minInvoked = true;
                maxInvoked = false;
            }
        }
        else if (Mathf.Abs(currentAngle - hinge.limits.max) <= tolerance)
        {
            if (!maxInvoked)
            {
                OnMaxReached.Invoke();
                maxInvoked = true;
                minInvoked = false;
            }
        }
        else
        {
            minInvoked = false;
            maxInvoked = false;
        }
    }

    public void Grab(Transform playerTransform)
    {
        player = playerTransform;
        isGrabbed = true;

        if (springJoint == null)
        {
            springJoint = gameObject.AddComponent<SpringJoint2D>();
            springJoint.connectedBody = player.GetComponent<Rigidbody2D>();
            springJoint.autoConfigureDistance = false;
            springJoint.distance = 0.25f;
            springJoint.dampingRatio = 0.8f;
            springJoint.frequency = 5f;
        }
    }

    public void Release()
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
            springJoint = null;
        }

        player = null;
        isGrabbed = false;
    }
}