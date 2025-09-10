using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuctionPoint : MonoBehaviour
{
    [Header("Configuración del embudo")]
    [Tooltip("Dirección central del embudo (ej. (1,0) = derecha)")]
    public Vector2 direction = Vector2.right;

    [Tooltip("Distancia máxima del embudo")]
    public float maxDistance = 5f;

    [Tooltip("Ángulo de apertura en grados (ej. 45 = cono de 90° total)")]
    [Range(1f, 180f)]
    public float coneAngle = 45f;

    [Header("Fuerza")]
    [Tooltip("Positivo para atraer, negativo para repeler")]
    public float forceStrength = 10f;

    [Tooltip("Capas que afectan (solo rigidbodies en estas layers serán afectados)")]
    public LayerMask affectedLayers;

    void FixedUpdate()
    {
        // Normalizar dirección
        Vector2 dirNorm = direction.normalized;

        // Buscar todos los colliders en un radio (para optimizar en lugar de miles de raycasts)
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, maxDistance, affectedLayers);

        foreach (Collider2D hit in hits)
        {
            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb == null || rb.gameObject == gameObject) continue;

            // Vector hacia el objeto
            Vector2 toTarget = (rb.position - (Vector2)transform.position);
            float dist = toTarget.magnitude;
            if (dist > maxDistance) continue;

            // Ángulo entre la dirección del cono y el objeto
            float angle = Vector2.Angle(dirNorm, toTarget);
            if (angle > coneAngle) continue; // fuera del cono

            // Aplicar fuerza (inversa al cuadrado de la distancia para que se sienta natural)
            Vector2 forceDir = toTarget.normalized * Mathf.Sign(forceStrength);
            float strength = Mathf.Abs(forceStrength) / (1f + dist);
            rb.AddForce(forceDir * strength, ForceMode2D.Force);
        }
    }

    // Dibujar en escena el embudo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = forceStrength >= 0 ? Color.cyan : Color.red;

        Vector3 origin = transform.position;
        Vector3 dir = direction.normalized * maxDistance;

        // Línea central
        Gizmos.DrawLine(origin, origin + dir);

        // Bordes del cono
        Quaternion rotLeft = Quaternion.Euler(0, 0, coneAngle);
        Quaternion rotRight = Quaternion.Euler(0, 0, -coneAngle);

        Vector3 leftDir = rotLeft * direction.normalized * maxDistance;
        Vector3 rightDir = rotRight * direction.normalized * maxDistance;

        Gizmos.DrawLine(origin, origin + leftDir);
        Gizmos.DrawLine(origin, origin + rightDir);
    }
}