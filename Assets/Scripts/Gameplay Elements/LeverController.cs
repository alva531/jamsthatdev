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

    [SerializeField] private float rotationSpeed = 180f;

    [Header("Opciones")]
    [Tooltip("Si está activado, la palanca rota invertida respecto al jugador")]
    [SerializeField] private bool invertRotation = false;

    [SerializeField] float angleOffset;
    private Transform leverAng;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hinge = GetComponent<HingeJoint2D>();
        leverAng = transform.GetChild(0);
    }

    void Start()
    {
        float zRot = Mathf.Round(leverAng.localEulerAngles.z);

        if (Mathf.Approximately(zRot, 90f))
        {
            GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.04f);
            GetComponent<BoxCollider2D>().offset = new Vector2(-0.15f, 0f);

            invertRotation = true;
            angleOffset = 0;
        }
        else if (Mathf.Approximately(zRot, 270f))
        {
            GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.04f);
            GetComponent<BoxCollider2D>().offset = new Vector2(0.15f, 0f);

            invertRotation = false;
            angleOffset = 0;
        }
        else if (Mathf.Approximately(zRot, 0f))
        {
            GetComponent<BoxCollider2D>().size = new Vector2(0.04f, 0.2f);
            GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0.15f);

            invertRotation = false;
            angleOffset = 90;
        }
        else if (Mathf.Approximately(zRot, 180f))
        {
            GetComponent<BoxCollider2D>().size = new Vector2(0.04f, 0.2f);
            GetComponent<BoxCollider2D>().offset = new Vector2(0f, -0.15f);

            invertRotation = false;
            angleOffset = -90;
        }
    }

    void FixedUpdate()
    {
        if (isGrabbed && player != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;

            float targetAngle = invertRotation
                ? Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg
                : Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            targetAngle -= angleOffset;
            targetAngle = Mathf.Clamp(targetAngle, hinge.limits.min, hinge.limits.max);

            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(newAngle);
        }

        float currentAngle = transform.localEulerAngles.z;
        if (currentAngle > 180) currentAngle -= 360;

        bool atMin = Mathf.Abs(currentAngle - hinge.limits.min) <= tolerance;
        bool atMax = Mathf.Abs(currentAngle - hinge.limits.max) <= tolerance;

        if (atMin && !minInvoked)
        {
            OnMinReached.Invoke();
            minInvoked = true;
            maxInvoked = false;
        }
        else if (atMax && !maxInvoked)
        {
            OnMaxReached.Invoke();
            maxInvoked = true;
            minInvoked = false;
        }
        else if (!atMin && !atMax)
        {
            // No hacemos nada aquí, solo se resetean al llegar al otro extremo
        }
    }

    public void Grab(Transform playerTransform)
    {
        player = playerTransform;
        isGrabbed = true;

        rb.bodyType = RigidbodyType2D.Dynamic;

        if (springJoint == null)
        {
            springJoint = gameObject.AddComponent<SpringJoint2D>();
            springJoint.connectedBody = player.GetComponent<Rigidbody2D>();
            springJoint.autoConfigureDistance = false;
            springJoint.distance = 0.35f;
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

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        player = null;
        isGrabbed = false;
    }
}
