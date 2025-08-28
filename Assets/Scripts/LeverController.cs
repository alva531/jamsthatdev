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
    private TargetJoint2D grabJoint;

    [SerializeField] private float tolerance = 2f;
    private bool minInvoked = false;
    private bool maxInvoked = false;

    // 👇 referencia al jugador cuando agarra la palanca
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hinge = GetComponent<HingeJoint2D>();
    }

    private void Update()
    {
        float angle = transform.localEulerAngles.z;
        if (angle > 180) angle -= 360; // convertir a [-180,180]

        if (Mathf.Abs(angle - hinge.limits.min) <= tolerance)
        {
            if (!minInvoked)
            {
                OnMinReached.Invoke();
                minInvoked = true;
                maxInvoked = false;
            }
        }
        else if (Mathf.Abs(angle - hinge.limits.max) <= tolerance)
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

        if (grabJoint == null)
        {
            grabJoint = gameObject.AddComponent<TargetJoint2D>();
            grabJoint.autoConfigureTarget = false;
            grabJoint.maxForce = 700f;
            grabJoint.dampingRatio = 0.9f;
            grabJoint.frequency = 2f;
        }

        grabJoint.target = player.position;
    }

    public void Release()
    {
        if (grabJoint != null)
        {
            Destroy(grabJoint);
            grabJoint = null;
        }

        player = null;
    }

    private void FixedUpdate()
    {
        if (grabJoint != null && player != null)
        {
            // 👇 suavizado para que no "salte" directo
            grabJoint.target = Vector2.Lerp(
                grabJoint.target,
                player.position,
                Time.fixedDeltaTime * 10f
            );
        }
    }
}