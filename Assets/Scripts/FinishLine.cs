using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    int num = 0;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Plant"))
        {
            num = num + 1;
            other.gameObject.tag = "Untagged";
            if (num >= 2)
            {
                StartCoroutine(FinishGame());
            }
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

