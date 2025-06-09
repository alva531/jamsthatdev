using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerConfigurationManager : MonoBehaviour
{
    private List<PlayerConfiguration> playerConfigs;

    [SerializeField] private int MaxPlayers = 2;
    [SerializeField] private TextMeshProUGUI countdownText;

    public static PlayerConfigurationManager Instance { get; private set; }

    private Coroutine countdownCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("SINGLETON - Trying to create another instance of singleton");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            playerConfigs = new List<PlayerConfiguration>();
        }
    }

    public List<PlayerConfiguration> GetPlayerConfigs()
    {
        return playerConfigs;
    }

    public void SetPlayerAnim(int index, AnimatorOverrideController anim)
    {
        playerConfigs[index].PlayerSkin = anim;
    }

    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = !playerConfigs[index].IsReady;
        Debug.Log($"Player {index} ready state: {playerConfigs[index].IsReady}");

        // Si todos los jugadores actuales están ready, iniciar la cuenta regresiva
        if (playerConfigs.Count > 0 && playerConfigs.All(p => p.IsReady))
        {
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(StartCountdownCoroutine());
            }
        }
        else
        {
            // Cancelar cuenta si alguien no está listo
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
                if (countdownText != null) countdownText.text = "";
            }
        }
    }

    private IEnumerator StartCountdownCoroutine()
    {
        int count = 3;

        while (count > 0)
        {
            // Si se desreadya alguien o se une alguien no ready, cancelar
            if (!playerConfigs.All(p => p.IsReady))
            {
                if (countdownText != null)
                    countdownText.text = "";
                countdownCoroutine = null;
                yield break;
            }

            if (countdownText != null)
                countdownText.text = count.ToString();

            yield return new WaitForSeconds(1f);
            count--;
        }

        // Verificamos nuevamente antes de avanzar
        if (playerConfigs.All(p => p.IsReady))
        {
            if (countdownText != null)
                countdownText.text = "¡START!";

            yield return new WaitForSeconds(0.5f);

            string sceneToLoad = playerConfigs.Count == 1 ? "Valve" : "CoopInput";
            SceneManager.LoadScene(sceneToLoad);
        }

        countdownCoroutine = null;
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        Debug.Log("Player Joined " + pi.playerIndex);
        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            playerConfigs.Add(new PlayerConfiguration(pi));

            // Cancelar cuenta si alguien se une sin estar listo
            if (countdownCoroutine != null && !playerConfigs.All(p => p.IsReady))
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
                if (countdownText != null) countdownText.text = "";
            }
        }
    }
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }

    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public AnimatorOverrideController PlayerSkin { get; set; }
}