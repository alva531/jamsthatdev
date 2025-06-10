using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectiveController : MonoBehaviour
{
    public int totalBox = 0;
    [SerializeField] GameObject door;
    [SerializeField] GameObject doorCollider;
    bool doorClosed = true;

    SoundController audioSource;


    void Update()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.FindWithTag("SoundController").GetComponent<SoundController>();
        }

        if (totalBox == 3)
        {
            if (doorClosed)
            {
                door.SetActive(false);
                doorCollider.SetActive(false);
                audioSource.DoorSFX();
                doorClosed = false;
            }
        }
        if (totalBox >= 9)
        {
            Debug.Log("GAME OVER");
            StartCoroutine(FinishGame());
        }
    }

    IEnumerator FinishGame()
    {

        yield return new WaitForSeconds(0.5f);

        GameObject.FindWithTag("Fade").GetComponent<Animator>().SetTrigger("Out");

        yield return new WaitForSeconds(1f);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Credits");
    }
}
