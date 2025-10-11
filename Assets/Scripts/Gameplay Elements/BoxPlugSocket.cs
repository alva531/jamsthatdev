using System.Collections.Generic;
using UnityEngine;

public class BoxPlugSocket : MonoBehaviour
{
    [Header("Cables")]
    public Cable[] cablesToActivate;

    [Header("Estado de energía")]
    public int currentEnergy = 0;

    private BatteryBox connectedBattery;
    private bool isPowered = false;

    [Header("Snapping")]
    [SerializeField] private float snapSpeed = 5f;

    private List<PlugSocket> activeSockets = new List<PlugSocket>();

    private void OnTriggerEnter2D(Collider2D other) => TrySnapBattery(other);
    private void OnTriggerStay2D(Collider2D other) => TrySnapBattery(other);

    private void TrySnapBattery(Collider2D other)
    {
        var grabbable = other.GetComponent<GrabbableObj>();
        if (grabbable != null && grabbable.isGrabbed) return;

        BatteryBox battery = other.GetComponent<BatteryBox>();
        if (battery == null) return;

        Rigidbody2D rb = battery.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        battery.transform.position = Vector2.Lerp(battery.transform.position, transform.position, Time.deltaTime * snapSpeed);
        battery.transform.rotation = Quaternion.Lerp(battery.transform.rotation, transform.rotation, Time.deltaTime * snapSpeed);

        if (connectedBattery == null)
            ConnectBattery(battery);
    }

    public void ConnectBattery(BatteryBox battery)
    {
        connectedBattery = battery;
        currentEnergy = Mathf.FloorToInt(battery.charge);
        isPowered = true;
        UpdateCablesVisual();
    }

    public void DisconnectBattery()
    {
        if (connectedBattery != null)
        {
            // Restauramos la carga original de la batería
            connectedBattery.RestoreOriginalCharge();
        }

        connectedBattery = null;
        isPowered = false;
        currentEnergy = 0;
        activeSockets.Clear();

        // Actualizamos visual de todos los cables
        foreach (var cable in cablesToActivate)
        {
            if (cable != null)
                cable.SetCharge(0f, true, null);
        }

        // Si algún plug seguía activo, avisamos que perdió energía
        foreach (var plug in activeSockets)
            plug.OnEnergyLost();
        
        activeSockets.Clear();
    }

    public bool TryConsumeEnergy(PlugSocket plug)
    {
        if (!isPowered) return false;
        if (activeSockets.Contains(plug)) return true;

        int used = 0;
        foreach (var ps in activeSockets)
            used += (int)ps.requiredEnergy;

        int available = currentEnergy - used;

        if (plug.requiredEnergy <= available)
        {
            activeSockets.Add(plug);
            UpdateCablesVisual();
            plug.OnEnergyAvailable();
            return true;
        }

        return false;
    }

    public void ReleaseEnergy(PlugSocket plug)
    {
        if (activeSockets.Contains(plug))
        {
            activeSockets.Remove(plug);
            UpdateCablesVisual();
            plug.OnEnergyLost();
        }
    }

    public void UpdateCablesVisual()
    {
        int used = 0;
        foreach (var ps in activeSockets)
            used += (int)ps.requiredEnergy;

        int remaining = Mathf.Max(0, currentEnergy - used);

        foreach (var cable in cablesToActivate)
        {
            if (cable == null) continue;

            bool isUsing = false;
            foreach (var ps in activeSockets)
            {
                if (ps.connectedCable == cable)
                {
                    isUsing = true;
                    break;
                }
            }

            int displayCharge = isUsing ? Mathf.FloorToInt(cable.charge) : remaining;
            cable.SetCharge(displayCharge, true, this);
        }
    }
}
