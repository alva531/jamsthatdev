using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{
    private AudioSource audioSource;

    private AudioClip thudSound;
    private AudioClip pickupSound;
    [SerializeField] private AudioClip menuSound;
    [SerializeField] private AudioClip boxSlideSound;


    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;


    [Header("Audio Sources")]

    [SerializeField] private AudioSource thudSource;
    [SerializeField] private AudioSource airSoundSource;
    [SerializeField] private AudioSource boxGrabSource;
    [SerializeField] private AudioSource potSource;
    [SerializeField] private AudioSource boxhitSource;
    [SerializeField] private AudioSource doorSource;

    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float airMaxVolume = 0.5f;

    private Coroutine airFadeCoroutine;
    private bool isAirSoundPlaying = false;


    [Header("Impact Sounds")]

    [SerializeField] private float minVolume = 0.1f;
    [SerializeField] private float maxVolume = 1.0f;
    [SerializeField] private float minSpeed = 1f;   // Velocidad mínima para oír algo
    [SerializeField] private float maxSpeed = 6f;   // Velocidad máxima = volumen máximo

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
    public void ThudSFX(float relativeSpeed)
    {
        float volume = Mathf.InverseLerp(minSpeed, maxSpeed, relativeSpeed);
        volume = Mathf.Clamp(volume, minVolume, maxVolume);

        thudSource.volume = volume;
        thudSource.Play();
    }

    public void BoxGrabSFX()
    {
        boxGrabSource.Play();
    }

    public void PotSFX()
    {
        potSource.Play();
    }

    public void BoxHitSFX()
    {
        boxhitSource.pitch = Random.Range(0.9f, 1.1f);
        boxhitSource.Play();
    }

    public void BoxSlideSFX()
    {
        audioSource.PlayOneShot(boxSlideSound);
    }

    public void DoorSFX()
    {
        doorSource.Play();
    }

    public void MenuMusic()
    {
        audioSource.PlayOneShot(menuMusic);
    }
    public void GameMusic()
    {
        audioSource.PlayOneShot(gameMusic);
    }



    public void SetAirSFXActive(bool active)
    {
        // Si se está pidiendo sonar
        if (active)
        {
            if (airFadeCoroutine != null)
                StopCoroutine(airFadeCoroutine);
            airFadeCoroutine = StartCoroutine(FadeInAir());
        }
        else // Si se quiere detener
        {
            if (airFadeCoroutine != null)
                StopCoroutine(airFadeCoroutine);
            airFadeCoroutine = StartCoroutine(FadeOutAir());
        }
    }

    private IEnumerator FadeInAir()
    {
        isAirSoundPlaying = true;

        // Asegurarse que esté sonando
        if (!airSoundSource.isPlaying)
            airSoundSource.Play();

        float startVolume = airSoundSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            airSoundSource.volume = Mathf.Lerp(startVolume, airMaxVolume, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        airSoundSource.volume = airMaxVolume;
    }

    private IEnumerator FadeOutAir()
    {
        float startVolume = airSoundSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            airSoundSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }

        airSoundSource.volume = 0f;
        airSoundSource.Stop();
        isAirSoundPlaying = false;
    }
}


