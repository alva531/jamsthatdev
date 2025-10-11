using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatteryBox : MonoBehaviour
{
    public float charge = 5f;

    private bool isConnected = false;
    private BoxPlugSocket currentSocket;
    private GrabbableObj grabbable;


    private float originalCharge;

    void Start()
    {
        grabbable = GetComponent<GrabbableObj>();
        originalCharge = charge;
    }

    void Update()
    {
        if (grabbable != null && grabbable.isGrabbed && isConnected)
            Disconnect();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConnected) return;

        BoxPlugSocket socket = other.GetComponent<BoxPlugSocket>();
        if (socket != null && grabbable != null && !grabbable.isGrabbed)
            Connect(socket);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        BoxPlugSocket socket = other.GetComponent<BoxPlugSocket>();
        if (socket != null && socket == currentSocket)
            Disconnect();
    }

    private void Connect(BoxPlugSocket socket)
    {
        isConnected = true;
        currentSocket = socket;
        socket.ConnectBattery(this);
    }

    private void Disconnect()
    {
        if (currentSocket != null)
        {
            currentSocket.DisconnectBattery();
            currentSocket = null;
        }

        isConnected = false;
    }

    public void RestoreOriginalCharge()
    {
        charge = originalCharge;
    }
}

