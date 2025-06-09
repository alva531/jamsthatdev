using System.Collections;
using System.Collections.Generic;
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

    float ignoreInputTime = 0.25f;
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
    }

    void Start()
    {
        PlayerConfigurationManager.Instance.SetPlayerAnim(PlayerIndex, playerAnimatorSkins[currentIndex]);
        UpdateVisual();
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
        currentIndex = (currentIndex + 1) % playerSprites.Length;
        ApplySkin();
    }

    void PreviousSkin()
    {
        currentIndex = (currentIndex - 1 + playerSprites.Length) % playerSprites.Length;
        ApplySkin();
    }

    void ApplySkin()
    {
        UpdateVisual();
        PlayerConfigurationManager.Instance.SetPlayerAnim(PlayerIndex, playerAnimatorSkins[currentIndex]);
    }

    void UpdateVisual()
    {
        if (playerDisplay != null && playerSprites.Length > 0)
            playerDisplay.sprite = playerSprites[currentIndex];
    }
}
