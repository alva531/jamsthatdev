using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidersObjective : MonoBehaviour
{
    public int boxNum = 0;
    [SerializeField]
    ObjectiveController objController;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Green") && gameObject.name == "ObjectiveGreen")
        {
            boxNum++;
            other.gameObject.tag = "Untagged";
            objController.totalBox++;
            GetComponentInChildren<Animator>().SetTrigger("+");
        }
        else if (other.CompareTag("Blue") && gameObject.name == "ObjectiveBlue")
        {
            boxNum++;
            other.gameObject.tag = "Untagged";
            objController.totalBox++;
            GetComponentInChildren<Animator>().SetTrigger("+");
        }
        else if (other.CompareTag("Pink") && gameObject.name == "ObjectivePink")
        {
            boxNum++;
            other.gameObject.tag = "Untagged";
            objController.totalBox++;
            GetComponentInChildren<Animator>().SetTrigger("+");
        }
    }
}
