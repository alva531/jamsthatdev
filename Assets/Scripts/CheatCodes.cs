using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCodes : MonoBehaviour
{
    private string inputBuffer = "";
    private Dictionary<string, System.Action> cheatCodes;


    [Header("Base Player")]
    [SerializeField] GameObject _Player1;
    [SerializeField] GameObject _Player2;
    [SerializeField] Animator _Player1Anim;
    [SerializeField] Animator _Player2Anim;

    RuntimeAnimatorController _original1;
    RuntimeAnimatorController _original2;

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

        _original1 = _Player1Anim.runtimeAnimatorController;
        _original2 = _Player2Anim.runtimeAnimatorController;
    }

    void FixedUpdate()
    {
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

    void UndertaleMod()
    {
        _Player1Anim.runtimeAnimatorController = _undertale1;
        _Player2Anim.runtimeAnimatorController = _undertale2;

        Debug.Log("OMG is that guy from untertale, what was it called? Sons??");
    }

    void IsaacMod()
    {
        _Player1Anim.runtimeAnimatorController = _isaac1;
        _Player2Anim.runtimeAnimatorController = _isaac2;
        Debug.Log("OMG is Isaac from The Binding of Isaac: Repentance+ Beta");
    }

    void NoMod()
    {
        _Player1Anim.runtimeAnimatorController = _original1;
        _Player2Anim.runtimeAnimatorController = _original2;
        Debug.Log("Mods Cleared");
    }

}