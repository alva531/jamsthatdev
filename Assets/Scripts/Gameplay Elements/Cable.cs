using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cable : MonoBehaviour
{
    [Tooltip("Cantidad de energía actual del cable.")]
    public float charge = 0f;
    [Tooltip("Duración de la transición visual del cambio de energía.")]
    public float chargeDuration = 1f;

    public Color unchargedColor = Color.gray;
    public Color chargedColor = Color.yellow;

    [SerializeField] private LineRenderer lineRenderer;
    private Coroutine chargeRoutine;

    [HideInInspector] public BoxPlugSocket sourceSocket;

    private void Awake() => UpdateVisual();

    public void SetCharge(float value, bool smooth = true, BoxPlugSocket source = null)
    {
        if (source != null)
            sourceSocket = source;

        value = Mathf.Max(0f, value);

        if (chargeRoutine != null)
            StopCoroutine(chargeRoutine);

        if (smooth)
            chargeRoutine = StartCoroutine(SmoothChargeTransition(value));
        else
        {
            charge = value;
            UpdateVisual();
        }
    }

    private IEnumerator SmoothChargeTransition(float targetCharge)
    {
        float startCharge = charge;
        float elapsed = 0f;

        while (elapsed < chargeDuration)
        {
            elapsed += Time.deltaTime;
            charge = Mathf.Lerp(startCharge, targetCharge, elapsed / chargeDuration);
            UpdateVisual();
            yield return null;
        }

        charge = targetCharge;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (lineRenderer == null) return;

        Color currentColor = Color.Lerp(unchargedColor, chargedColor, Mathf.InverseLerp(0f, 5f, charge));
        lineRenderer.startColor = currentColor;
        lineRenderer.endColor = currentColor;
    }
}
