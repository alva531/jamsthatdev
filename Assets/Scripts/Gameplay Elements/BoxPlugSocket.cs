using UnityEngine;

public class BoxPlugSocket : MonoBehaviour
{
    [Header("Cables")]
    public Cable[] cablesToActivate;

    [Header("Estado de energía")]
    public float currentEnergy = 0f;

    private BatteryBox connectedBattery;
    private bool isPowered = false;

    [Header("Snapping")]
    [SerializeField] private float snapSpeed = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrySnapBattery(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TrySnapBattery(other);
    }

    private void TrySnapBattery(Collider2D other)
    {
        GrabbableObj grabbable = other.GetComponent<GrabbableObj>();
        if (grabbable != null && grabbable.isGrabbed) return;

        BatteryBox battery = other.GetComponent<BatteryBox>();
        if (battery == null) return;

        Rigidbody2D rb = battery.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        battery.transform.position = Vector2.Lerp(
            battery.transform.position,
            transform.position,
            Time.deltaTime * snapSpeed
        );

        battery.transform.rotation = Quaternion.Lerp(
            battery.transform.rotation,
            transform.rotation,
            Time.deltaTime * snapSpeed
        );

        if (connectedBattery == null)
            ConnectBattery(battery);
    }

    public void ConnectBattery(BatteryBox battery)
    {
        connectedBattery = battery;
        currentEnergy = battery.charge;
        isPowered = true;
        UpdateCables();
    }

    public void DisconnectBattery()
    {
        connectedBattery = null;
        isPowered = false;
        currentEnergy = 0f;
        UpdateCables();
    }

    private void UpdateCables()
    {
        foreach (var cable in cablesToActivate)
        {
            if (cable != null)
                cable.SetCharge(isPowered ? currentEnergy : 0f, true, this);
        }
    }

    public void OnPlugSocketConnect()
    {
        if (!isPowered) return;
    }

    public void OnPlugSocketDisconnect()
    {
        if (!isPowered) return;
    }

    public void RestoreEnergyFromPlug()
    {
        if (connectedBattery != null)
        {
            currentEnergy = connectedBattery.charge;
            UpdateCables();
        }
    }
}
