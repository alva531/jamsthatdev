using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    [Header("Eventos básicos")]
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    [Header("Evento cooperativo (cuando todos se presionan a la vez)")]
    public UnityEvent OnAllPressed;

    [Header("Opciones")]
    public bool singleUse = false;
    public bool cooperative = false;

    [Header("Restricciones de activación")]
    public bool boxOnly = false;    // Solo se activa con objetos con tag "Grabbable"
    public bool playerOnly = false; // Solo se activa con objetos con tag "Player"

    [Tooltip("Solo se usa si cooperative = true. Lista de botones relacionados.")]
    public List<ButtonController> relatedButtons;

    private int objectsOnButton = 0;
    private bool used = false;
    private bool coopLocked = false;
    private bool isPressed = false;

    private bool allPressedInvoked = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used && singleUse) return;
        if (coopLocked) return;

        if (!IsValidActivator(other)) return; // 🔹 filtro por boxOnly / playerOnly

        objectsOnButton++;

        if (objectsOnButton == 1 && !isPressed)
        {
            OnPressed.Invoke();
            isPressed = true;

            if (singleUse)
                used = true;
        }

        if (other.CompareTag("Grabbable"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (cooperative)
            CheckCooperativeGroup();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (used && singleUse) return;
        if (coopLocked) return;

        if (!IsValidActivator(other)) return; // 🔹 filtro por boxOnly / playerOnly

        objectsOnButton = Mathf.Max(0, objectsOnButton - 1);

        if (objectsOnButton == 0 && isPressed)
        {
            OnReleased.Invoke();
            isPressed = false;
        }

        if (cooperative)
            CheckCooperativeGroup();
    }

    void CheckCooperativeGroup()
    {
        if (coopLocked) return;

        List<ButtonController> group = new List<ButtonController>(relatedButtons);
        if (!group.Contains(this))
            group.Add(this);

        int pressedCount = 0;
        foreach (var button in group)
        {
            if (button != null && button.objectsOnButton > 0)
                pressedCount++;
        }

        bool allPressed = pressedCount == group.Count;

        if (allPressed)
        {
            foreach (var button in group)
            {
                button.coopLocked = true;

                if (!button.allPressedInvoked)
                {
                    button.OnAllPressed.Invoke();
                    button.allPressedInvoked = true;
                }
            }
        }
        else
        {
            foreach (var button in group)
                button.allPressedInvoked = false;
        }
    }

    private bool IsValidActivator(Collider2D other)
    {
        if (boxOnly && !other.CompareTag("Grabbable")) return false;
        if (playerOnly && !other.CompareTag("Player")) return false;

        if (!other.CompareTag("Grabbable") && !other.CompareTag("Player")) return false;

        return true;
    }
}
