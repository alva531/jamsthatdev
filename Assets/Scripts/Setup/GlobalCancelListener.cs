using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GlobalCancelListener : MonoBehaviour
{
    private InputAction cancelAction;
    [SerializeField] private GameObject _fade;

    void OnEnable()
    {
        cancelAction = new InputAction(type: InputActionType.Button);
        cancelAction.AddBinding("<Keyboard>/escape");       // ESC
        cancelAction.AddBinding("<Gamepad>/buttonEast");    // B o Círculo

        cancelAction.performed += OnCancel;
        cancelAction.Enable();
    }

    void OnDisable()
    {
        cancelAction.performed -= OnCancel;
        cancelAction.Disable();
    }

    private void OnCancel(InputAction.CallbackContext context)
    {
        // Chequear que no hay jugadores activos
        if (PlayerConfigurationManager.Instance != null &&
            PlayerConfigurationManager.Instance.GetPlayerConfigs().Count == 0)
        {
            Debug.Log("Cancel pressed with no players, returning to main menu...");
            StartCoroutine(fadeOut());
        }
    }

    private IEnumerator fadeOut()
    {
        _fade.GetComponent<Animator>().SetTrigger("Out");

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene("MainMenu");
        
    }
}