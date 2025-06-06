using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    public GameObject PlayerSetupMenuPrefab;
    public PlayerInput input;
    void Awake()
    {
        var rootMenu = GameObject.Find("MainLayout");
        if (rootMenu != null)
        {
            var menu = Instantiate(PlayerSetupMenuPrefab, rootMenu.transform);
            input.uiInputModule = menu.GetComponentInChildren<InputSystemUIInputModule>();
            menu.GetComponent<PlayerSetupMenuController>().playerInput = input;
            menu.GetComponent<PlayerSetupMenuController>().SetPlayerIndex(input.playerIndex);
        }
    }
}
