using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantsController : MonoBehaviour
{
    public int plantsDone = 0;

    [SerializeField] GameObject _plant1;
    [SerializeField] GameObject _plant2;
    [SerializeField] GameObject _plant3;
    [SerializeField] GameObject _plant4;
    [SerializeField] GameObject _plant5;
    [SerializeField] GameObject _plant6;
    [SerializeField] GameObject _plant7;
    [SerializeField] GameObject _plant8;

    [SerializeField] GameObject _door;

    [SerializeField] GameObject _plantBox1;
    [SerializeField] GameObject _plantBox2;


    [SerializeField] SoundController _soundController;

    public void MorePlants()
    {
        plantsDone = plantsDone + 1;
        if (plantsDone >= 8)
        {
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject.FindWithTag("Fade").GetComponent<Animator>().SetTrigger("Out");

        yield return new WaitForSeconds(1.5f);

        _door.SetActive(false);

        _plant1.GetComponent<Animator>().SetTrigger("Idle");
        _plant2.GetComponent<Animator>().SetTrigger("Idle");
        _plant3.GetComponent<Animator>().SetTrigger("Idle");
        _plant4.GetComponent<Animator>().SetTrigger("Idle");
        _plant5.GetComponent<Animator>().SetTrigger("Idle");
        _plant6.GetComponent<Animator>().SetTrigger("Idle");
        _plant7.GetComponent<Animator>().SetTrigger("Idle");
        _plant8.GetComponent<Animator>().SetTrigger("Idle");

        _plantBox1.SetActive(true);
        _plantBox2.SetActive(true);

        _soundController.DoorSFX();

        GameObject.FindWithTag("Fade").GetComponent<Animator>().SetTrigger("In");

    }

}
