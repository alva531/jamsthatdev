using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonController : MonoBehaviour
{
    [Header("Eventos del botón")]
    public UnityEvent OnPressed;
    public UnityEvent OnReleased;

    [Header("Opciones especiales")]
    public bool singleUse = false;      // Se activa solo una vez
    public bool cooperative = false;    // Si es true, funciona en grupo cooperativo

    [Tooltip("Solo se usa si cooperative = true. Lista de botones relacionados.")]
    public List<ButtonController> relatedButtons;

    int objectsOnButton = 0;
    bool used = false;
    bool coopLocked = false; // Queda activado de forma permanente

    void OnTriggerEnter2D(Collider2D other)
    {
        if (used && singleUse) return;
        if (coopLocked && cooperative) return;

        if (other.CompareTag("Grabbable") || other.CompareTag("Player"))
        {
            objectsOnButton++;

            if (objectsOnButton == 1)
            {
                OnPressed.Invoke();

                if (singleUse)
                {
                    used = true;
                    //Destroy(gameObject); // opcional
                }
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
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (used && singleUse) return;
        if (coopLocked && cooperative) return;

        if (other.CompareTag("Grabbable") || other.CompareTag("Player"))
        {
            objectsOnButton = Mathf.Max(0, objectsOnButton - 1);

            if (!cooperative)
            {
                if (objectsOnButton == 0)
                {
                    OnReleased.Invoke();
                }
            }
            else
            {
                CheckCooperativeGroup();
            }
        }
    }

    void CheckCooperativeGroup()
    {
        if (coopLocked) return;

        // Construimos el grupo: este botón + los relacionados
        List<ButtonController> group = new List<ButtonController>(relatedButtons);
        if (!group.Contains(this))
            group.Add(this);

        // Contamos cuántos botones del grupo están presionados
        int pressedCount = 0;
        foreach (var button in group)
        {
            if (button != null && button.objectsOnButton > 0)
                pressedCount++;
        }

        // Si hay al menos 2 presionados → se bloquea el grupo completo
        if (pressedCount >= 2)
        {
            foreach (var button in group)
            {
                if (button != null)
                {
                    button.OnPressed.Invoke();
                    button.coopLocked = true;
                }
            }
        }
        else
        {
            // Si no hay 2, los que no están presionados se liberan
            foreach (var button in group)
            {
                if (button != null && button.objectsOnButton == 0)
                {
                    button.OnReleased.Invoke();
                }
            }
        }
    }
}
