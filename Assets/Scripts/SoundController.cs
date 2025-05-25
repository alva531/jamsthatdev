using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] private AudioClip thudSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip menuSound;
    [SerializeField] private AudioClip boxSlideSound;


    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;
    }

    public void PickupSFX()
    {
        audioSource.PlayOneShot(pickupSound);
    }

    public void MenuSFX()
    {
        audioSource.PlayOneShot(menuSound);
    }
    public void ThudSFX()
    {
        audioSource.PlayOneShot(thudSound);
    }

    public void BoxSlideSFX()
    {
        audioSource.PlayOneShot(boxSlideSound);
    }

    public void MenuMusic()
    {
        audioSource.PlayOneShot(menuMusic);
    }
    public void GameMusic()
    {
        audioSource.PlayOneShot(gameMusic);
    }


}
