using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerSkinSelector : MonoBehaviour
{
    public Animator playerDisplay;
    public AnimatorOverrideController[] playerSkins;

    private int currentIndex = 0;
    private float inputCooldown = 0.3f;
    private float lastInputTime = 0f;

    public PlayerInput input;


    void Start()
    {
        UpdateVisual();
    }

    public void OnNavigate(InputValue value)
    {
        Vector2 inputVector = value.Get<Vector2>();

        if (Time.time - lastInputTime < inputCooldown)
            return;

        if (inputVector.x > 0.5f)
        {
            Debug.Log("Der");
            NextSkin();
            lastInputTime = Time.time;
        }
        else if (inputVector.x < -0.5f)
        {
            Debug.Log("Izq");
            PreviousSkin();
            lastInputTime = Time.time;
        }
    }

    // Opcional si querés usar submit (como enter o A)
    // public void OnSubmit(InputValue value)
    // {
    //     Debug.Log("Submit pressed");
    // }

    public void SetPlayerInput(PlayerInput inp)
    {
        input.uiInputModule  = inp.uiInputModule;
    }

    void NextSkin()
    {
        currentIndex = (currentIndex + 1) % playerSkins.Length;
        ApplySkin();
    }

    void PreviousSkin()
    {
        currentIndex = (currentIndex - 1 + playerSkins.Length) % playerSkins.Length;
        ApplySkin();
    }

    void ApplySkin()
    {
        UpdateVisual();
        PlayerConfigurationManager.Instance.SetPlayerAnim(input.playerIndex, playerSkins[currentIndex]);
    }

    void UpdateVisual()
    {
        if (playerDisplay != null && playerSkins.Length > 0)
            playerDisplay.runtimeAnimatorController = playerSkins[currentIndex];
    }
}