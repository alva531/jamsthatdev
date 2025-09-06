using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    int PlayerIndex;

    [Header("UI References")]
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] Button readyButton;

    [SerializeField] GameObject _keyboardUI;
    [SerializeField] GameObject _gamepadUI;

    [Header("Selector de Parte Activa")]
    [SerializeField] RectTransform partSelector;
    [SerializeField] Vector3[] partSelectorPositions; 

    [Header("Player Preview (Images en la escena)")]
    [SerializeField] Image bodyDisplay;
    [SerializeField] Image headDisplay;
    [SerializeField] Image legsDisplay;
    [SerializeField] Image jetpackDisplay;

    [Header("Preview Sprites")]
    [SerializeField] Sprite[] bodySprites;
    [SerializeField] Sprite[] headSprites;
    [SerializeField] Sprite[] legsSprites;
    [SerializeField] Sprite[] jetpackSprites;

    [Header("Player Parts Skins")]
    public AnimatorOverrideController[] bodySkins;
    public AnimatorOverrideController[] headSkins;
    public AnimatorOverrideController[] legsSkins;
    public AnimatorOverrideController[] jetpackSkins;

    private int currentBodyIndex = 0;
    private int currentHeadIndex = 0;
    private int currentLegsIndex = 0;
    private int currentJetpackIndex = 0;

    private enum Part { Head, Body, Legs, Jetpack }
    private Part currentPart = Part.Head;

    private float inputCooldown = 0.3f;
    private float lastInputTime = 0f;
    private float ignoreInputTime = 0.2f;
    private bool inputEnabled;
    private bool freeSkinChange = false; // Nuevo flag

    public PlayerInput playerInput;

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        titleText.SetText("Player " + (pi + 1));
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    void Start()
    {
        SetupInputType();
        readyButton.Select();

        playerInput.actions["Cancel"].performed += OnCancelPressed;

        ApplySkin();
        UpdatePartSelector();
    }

    void Update()
    {
        if (Time.time > ignoreInputTime) inputEnabled = true;
        Vector2 moveInput = GetMoveInput();

        // --- Cambiar parte (siempre con cooldown) ---
        if (Time.time - lastInputTime > inputCooldown)
        {
            if (moveInput.y > 0.5f)
            {
                PreviousPart();
                lastInputTime = Time.time;
            }
            else if (moveInput.y < -0.5f)
            {
                NextPart();
                lastInputTime = Time.time;
            }
        }

        // --- Cambiar skin (con excepción del primer cambio libre) ---
        bool canUseSkinInput = (Time.time - lastInputTime > inputCooldown) || freeSkinChange;

        if (canUseSkinInput)
        {
            if (moveInput.x > 0.5f)
            {
                NextSkin();
                lastInputTime = Time.time;
                freeSkinChange = false; // gastamos el primer cambio libre
            }
            else if (moveInput.x < -0.5f)
            {
                PreviousSkin();
                lastInputTime = Time.time;
                freeSkinChange = false;
            }
        }

        readyButton.Select();
    }

    void UpdatePartSelector()
    {
        if (partSelector != null && partSelectorPositions.Length == Enum.GetNames(typeof(Part)).Length)
        {
            partSelector.localPosition = partSelectorPositions[(int)currentPart];
        }
    }

    public void SetupInputType()
    {
        if (playerInput == null) return;
        string controlScheme = playerInput.currentControlScheme;
        _keyboardUI.SetActive(controlScheme.Contains("Keyboard"));
        _gamepadUI.SetActive(controlScheme.Contains("Gamepad"));
    }

    public void SetSkin()
    {
        if (!inputEnabled) return;
        ApplySkin();
        readyPanel.SetActive(true);
        menuPanel.SetActive(false);
        readyButton.Select();
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) return;
        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);

        Animator rbAnim = readyButton.GetComponent<Animator>();
        if (rbAnim.GetCurrentAnimatorStateInfo(0).IsName("Ready_on"))
            rbAnim.SetTrigger("off");
        else
            rbAnim.SetTrigger("on");
    }

    private void OnCancelPressed(InputAction.CallbackContext context)
    {
        var config = PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex];
        if (config.IsReady)
        {
            PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
            readyButton.GetComponent<Animator>().SetTrigger("off");
            readyButton.Select();
        }
        else
        {
            playerInput.actions["Cancel"].performed -= OnCancelPressed;
            PlayerConfigurationManager.Instance.RemovePlayer(PlayerIndex);
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        if (playerInput != null)
            playerInput.actions["Cancel"].performed -= OnCancelPressed;
    }

    Vector2 GetMoveInput()
    {
        if (playerInput == null) return Vector2.zero;
        InputAction navigateAction = playerInput.actions["Navigate"];
        return navigateAction != null ? navigateAction.ReadValue<Vector2>() : Vector2.zero;
    }

    // =============================
    // Cambiar parte activa
    void NextPart()
    {
        currentPart = (Part)(((int)currentPart + 1) % Enum.GetNames(typeof(Part)).Length);
        Debug.Log("Parte seleccionada: " + currentPart);
        UpdatePartSelector();
        freeSkinChange = true; // habilitamos el primer cambio rápido de skin
    }

    void PreviousPart()
    {
        int max = Enum.GetNames(typeof(Part)).Length;
        currentPart = (Part)(((int)currentPart - 1 + max) % max);
        Debug.Log("Parte seleccionada: " + currentPart);
        UpdatePartSelector();
        freeSkinChange = true;
    }

    // =============================
    // Cambiar skin de la parte activa
    void NextSkin()
    {
        if (PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex].IsReady) return;

        switch (currentPart)
        {
            case Part.Body:
                currentBodyIndex = (currentBodyIndex + 1) % bodySkins.Length;
                break;
            case Part.Head:
                currentHeadIndex = (currentHeadIndex + 1) % headSkins.Length;
                break;
            case Part.Legs:
                currentLegsIndex = (currentLegsIndex + 1) % legsSkins.Length;
                break;
            case Part.Jetpack:
                currentJetpackIndex = (currentJetpackIndex + 1) % jetpackSkins.Length;
                break;
        }

        ApplySkin();
    }

    void PreviousSkin()
    {
        if (PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex].IsReady) return;

        switch (currentPart)
        {
            case Part.Body:
                currentBodyIndex = (currentBodyIndex - 1 + bodySkins.Length) % bodySkins.Length;
                break;
            case Part.Head:
                currentHeadIndex = (currentHeadIndex - 1 + headSkins.Length) % headSkins.Length;
                break;
            case Part.Legs:
                currentLegsIndex = (currentLegsIndex - 1 + legsSkins.Length) % legsSkins.Length;
                break;
            case Part.Jetpack:
                currentJetpackIndex = (currentJetpackIndex - 1 + jetpackSkins.Length) % jetpackSkins.Length;
                break;
        }

        ApplySkin();
    }

    // =============================
    // Aplicar cambios
    void ApplySkin()
    {
        UpdateVisual();

        PlayerConfigurationManager.Instance.SetPlayerAnim(
            PlayerIndex,
            bodySkins[currentBodyIndex],
            headSkins[currentHeadIndex],
            legsSkins[currentLegsIndex],
            jetpackSkins[currentJetpackIndex]
        );

        readyButton.Select();
    }

    void UpdateVisual()
    {
        if (bodyDisplay != null && bodySprites.Length > 0)
            bodyDisplay.sprite = bodySprites[currentBodyIndex];

        if (headDisplay != null && headSprites.Length > 0)
            headDisplay.sprite = headSprites[currentHeadIndex];

        if (legsDisplay != null && legsSprites.Length > 0)
            legsDisplay.sprite = legsSprites[currentLegsIndex];

        if (jetpackDisplay != null && jetpackSprites.Length > 0)
            jetpackDisplay.sprite = jetpackSprites[currentJetpackIndex];
    }
}
