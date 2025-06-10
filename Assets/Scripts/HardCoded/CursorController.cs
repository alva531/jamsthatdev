using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorController : MonoBehaviour
{
    [SerializeField] private Texture2D defaultTexture;
    [SerializeField] private Texture2D clickTexture;

    private Vector2 cursorHotspot;
    private InputAction leftClick;
    private Coroutine revertCoroutine;

    void Awake()
    {
        leftClick = new InputAction("LeftClick", InputActionType.Button, "<Mouse>/leftButton");
        leftClick.started += OnClickStarted;   // Cuando se presiona
        leftClick.canceled += OnClickReleased; // Cuando se suelta
    }

    void Start()
    {
        cursorHotspot = Vector2.zero;
    }

    void OnEnable()
    {
        leftClick.Enable();
    }

    void OnDisable()
    {
        leftClick.Disable();
    }

    private void OnClickStarted(InputAction.CallbackContext context)
    {
        // Al presionar el botón
        Cursor.SetCursor(clickTexture, cursorHotspot, CursorMode.Auto);

        // Cancelar cualquier espera previa
        if (revertCoroutine != null)
            StopCoroutine(revertCoroutine);
    }

    private void OnClickReleased(InputAction.CallbackContext context)
    {
        // Al soltar el botón, esperar 0.2s y luego volver al cursor por defecto
        revertCoroutine = StartCoroutine(RevertCursorAfterDelay(0.2f));
    }

    IEnumerator RevertCursorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cursor.SetCursor(defaultTexture, cursorHotspot, CursorMode.Auto);
    }
}