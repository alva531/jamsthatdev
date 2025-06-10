using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    int PlayerIndex;
    [SerializeField]
    TextMeshProUGUI titleText;
    [SerializeField]
    GameObject readyPanel;
    [SerializeField]
    GameObject menuPanel;

    [SerializeField]
    Button readyButton;

    float ignoreInputTime = 1f;
    bool inputEnabled;

    [Header ("Player")]
    public Sprite[] playerSprites;
    public Image playerDisplay;
    public AnimatorOverrideController[] playerAnimatorSkins;

    private int currentIndex = 0;
    private float inputCooldown = 0.3f;
    private float lastInputTime = 0f;


    private PlayerConfiguration config;

    public PlayerInput playerInput;

    [SerializeField]
    GameObject _keyboardUI;
    [SerializeField]
    GameObject _gamepadUI;

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        titleText.SetText("Player " + (pi + 1).ToString());
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }

        Vector2 moveInput = GetMoveInput();

        if (Time.time - lastInputTime > inputCooldown)
        {
            if (moveInput.x > 0.5f)
            {
                NextSkin();
                lastInputTime = Time.time;
            }
            else if (moveInput.x < -0.5f)
            {
                PreviousSkin();
                lastInputTime = Time.time;
            }
        }

        readyButton.Select();

    }

    public void SetupInputType()
    {
        if (playerInput == null)
            return;

        string controlScheme = playerInput.currentControlScheme;

        if (controlScheme == "Keyboard&Mouse" || controlScheme == "Keyboard")
        {
            _keyboardUI.SetActive(true);
            _gamepadUI.SetActive(false);
        }
        else if (controlScheme == "Gamepad")
        {
            _keyboardUI.SetActive(false);
            _gamepadUI.SetActive(true);
        }
    }

    public void SetSkin(AnimatorOverrideController anim)
    {
        if (!inputEnabled) { return; }

        anim = playerAnimatorSkins[currentIndex];

        PlayerConfigurationManager.Instance.SetPlayerAnim(PlayerIndex, playerAnimatorSkins[currentIndex]);
        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return; }

        PlayerConfigurationManager.Instance.ReadyPlayer(PlayerIndex);
        //readyButton.gameObject.SetActive(false);
        if (readyButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Ready_on"))
        {
            readyButton.GetComponent<Animator>().SetTrigger("off");
        }
        else
        {
            readyButton.GetComponent<Animator>().SetTrigger("on");
        }
    }

    void Start()
    {
        PlayerConfigurationManager.Instance.SetPlayerAnim(PlayerIndex, playerAnimatorSkins[currentIndex]);
        SetupInputType();
        UpdateVisual();
        readyButton.Select();

        playerInput.actions["Cancel"].performed += OnCancelPressed;
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
            // ✅ DESUSCRIBIR EVENTO ANTES DE DESTRUIR
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
        if (playerInput == null)
            return Vector2.zero;

        InputAction navigateAction = playerInput.actions["Navigate"];

            if (navigateAction != null)
                return navigateAction.ReadValue<Vector2>();


        return Vector2.zero;
    }

    void NextSkin()
    {
        if (PlayerIndex >= 0 && PlayerIndex < PlayerConfigurationManager.Instance.GetPlayerConfigs().Count &&
            !PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex].IsReady)
        {
            currentIndex = (currentIndex + 1) % playerSprites.Length;
            ApplySkin();
        }
        readyButton.Select();
    }

    void PreviousSkin()
    {
        if (PlayerConfigurationManager.Instance.GetPlayerConfigs()[PlayerIndex].IsReady == false)
        {
            currentIndex = (currentIndex - 1 + playerSprites.Length) % playerSprites.Length;
            ApplySkin();
        } 
        readyButton.Select();
    }

    void ApplySkin()
    {
        UpdateVisual();
        PlayerConfigurationManager.Instance.SetPlayerAnim(PlayerIndex, playerAnimatorSkins[currentIndex]);
        readyButton.Select();
    }

    void UpdateVisual()
    {
        if (playerDisplay != null && playerSprites.Length > 0)
            playerDisplay.sprite = playerSprites[currentIndex];
    }
}
