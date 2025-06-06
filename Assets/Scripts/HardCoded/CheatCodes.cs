using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    private string inputBuffer = "";
    private Dictionary<string, System.Action> cheatCodes;


    [Header("Base Player")]
    [SerializeField] private int PlayerCount = 2;

    private List<GameObject> players = new List<GameObject>();
    private List<Animator> animators = new List<Animator>();

    private List<RuntimeAnimatorController> originalControllers = new List<RuntimeAnimatorController>();

    [Header("Mod Stuff")]

    [SerializeField] AnimatorOverrideController _undertale1;
    [SerializeField] AnimatorOverrideController _undertale2;
    [SerializeField] AnimatorOverrideController _isaac1;
    [SerializeField] AnimatorOverrideController _isaac2;


    void Start()
    {
        cheatCodes = new Dictionary<string, System.Action>()
        {
            { "UNDERTALE", UndertaleMod },
            { "ISAAC", IsaacMod },
            { "CLEAR", NoMod },
        };

        UpdatePlayersIfNeeded();

        foreach (var animator in animators)
        {
            originalControllers.Add(animator.runtimeAnimatorController);
        }
    }

    void FixedUpdate()
    {
        UpdatePlayersIfNeeded();

        foreach (char c in Input.inputString)
        {
            if (char.IsLetterOrDigit(c))
            {
                inputBuffer += char.ToUpper(c);

                if (inputBuffer.Length > 30)
                    inputBuffer = inputBuffer.Substring(inputBuffer.Length - 30);

                foreach (var code in cheatCodes)
                {
                    if (inputBuffer.Contains(code.Key))
                    {
                        code.Value.Invoke();
                        inputBuffer = "";
                        break;
                    }
                }
            }
        }
    }

    void UpdatePlayersIfNeeded()
    {
        if (players.Count < PlayerCount)
        {
            GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

            players.Clear();
            animators.Clear();

            foreach (var obj in foundPlayers)
            {
                players.Add(obj);

                Animator anim = obj.GetComponent<Animator>();
                if (anim != null)
                    animators.Add(anim);
            }
        }
    }

    void UndertaleMod()
    {
        for (int i = 0; i < animators.Count; i++)
        {
            if (i == 0) animators[i].runtimeAnimatorController = _undertale1;
            else if (i == 1) animators[i].runtimeAnimatorController = _undertale2;
        }

        Debug.Log("OMG is that guy from untertale, what was it called? Sons??");
    }

    void IsaacMod()
    {
        for (int i = 0; i < animators.Count; i++)
        {
            if (i == 0) animators[i].runtimeAnimatorController = _isaac1;
            else if (i == 1) animators[i].runtimeAnimatorController = _isaac2;
        }

        Debug.Log("OMG is Isaac from The Binding of Isaac: Repentance+ Beta");
    }

    void NoMod()
    {
        for (int i = 0; i < animators.Count && i < originalControllers.Count; i++)
        {
            animators[i].runtimeAnimatorController = originalControllers[i];
        }

        Debug.Log("Mods Cleared");
    }
}