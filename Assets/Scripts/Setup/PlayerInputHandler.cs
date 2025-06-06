using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerConfiguration playerConfig;
    private JetpackMovement jetpackMovement;
    private PlayerGrab playerGrab;

    [SerializeField]
    Animator playerAnim;

    InputActions playerControls;

    private void Awake()
    {
        jetpackMovement = GetComponent<JetpackMovement>();
        playerGrab = GetComponent<PlayerGrab>();

        playerControls = new InputActions();
    }

    public void InitializePlayer(PlayerConfiguration pc)
    {
        playerConfig = pc;
        playerAnim.runtimeAnimatorController = pc.PlayerSkin;
        playerConfig.Input.onActionTriggered += Input_onActionTriggered;
    }

    void Input_onActionTriggered(CallbackContext obj)
    {
        if (obj.action.name == playerControls.Player.Move.name)
        {
            OnMove(obj);
        }
        if (obj.action.name == playerControls.Player.Grab.name)
        {
            OnGrab(obj);
        }
    }


    public void OnMove(CallbackContext context)
    {
        if (jetpackMovement != null)
            jetpackMovement.SetInputVector(context.ReadValue<Vector2>());
    }

    public void OnGrab(CallbackContext context)
    {
        if (playerGrab != null)
            playerGrab.TryGrab(context.ReadValue<float>() > 0.5f);
    }
}