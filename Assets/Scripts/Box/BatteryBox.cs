using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BatteryBox : MonoBehaviour
{
    [Header("Configuración de la batería")]
    [Tooltip("Cantidad total de energía disponible en la batería.")]
    public float charge = 100f;

    private bool isConnected = false;
    private BoxPlugSocket currentSocket;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isConnected) return;

        BoxPlugSocket socket = other.GetComponent<BoxPlugSocket>();
        if (socket != null)
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
}
