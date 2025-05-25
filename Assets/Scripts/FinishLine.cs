using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Debug.Log("Credits");
    }
}

