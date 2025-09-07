using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public void LoadCharacterSelection()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("CoopInput2 Backup");
    }
}