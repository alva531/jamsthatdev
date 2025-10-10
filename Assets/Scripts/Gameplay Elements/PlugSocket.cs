using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlugSocket : MonoBehaviour
{
    [Header("Energia Requerida")]
    public float requiredEnergy = 50f;

    [Header("Eventos")]
    public UnityEvent OnPoweredConnect;
    public UnityEvent OnDisconnect;

    private Cable connectedCable;
    private bool isConnected = false;

    [Header("Snapping")]
    [SerializeField] private float snapSpeed = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cable"))
        {
            TrySnapCable(other);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cable"))
        {
            TrySnapCable(other);
        }
    }

    private void TrySnapCable(Collider2D other)
    {
        GrabbableObj grabbable = other.GetComponent<GrabbableObj>();
        if (grabbable != null && grabbable.isGrabbed) return;

        Cable cable = other.GetComponent<Cable>();
        if (cable == null) return;

        Rigidbody2D rb = cable.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        cable.transform.position = Vector2.Lerp(
            cable.transform.position,
            transform.position,
            Time.deltaTime * snapSpeed
        );

        cable.transform.rotation = Quaternion.Lerp(
            cable.transform.rotation,
            transform.rotation,
            Time.deltaTime * snapSpeed
        );

        if (!isConnected)
            ConnectCable(cable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Cable"))
        {
            Cable cable = other.GetComponent<Cable>();
            if (cable != null && isConnected && cable == connectedCable)
                DisconnectCable();
        }
    }

    private void ConnectCable(Cable cable)
    {
        connectedCable = cable;
        isConnected = true;

        float energy = cable.charge;

        if (energy >= requiredEnergy)
        {
            OnPoweredConnect?.Invoke();

            if (cable.sourceSocket != null)
                cable.sourceSocket.OnPlugSocketConnect();
        }
    }

    private void DisconnectCable()
    {
        if (!isConnected) return;

        OnDisconnect?.Invoke();

        if (connectedCable != null && connectedCable.sourceSocket != null)
        {
            connectedCable.sourceSocket.RestoreEnergyFromPlug();
            connectedCable.sourceSocket.OnPlugSocketDisconnect();
        }

        connectedCable = null;
        isConnected = false;
    }
}
