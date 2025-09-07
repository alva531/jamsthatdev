using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Animator _playButtonChild;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Animator _settingsButtonChild;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Animator _exitButtonChild;

    [SerializeField] private Button _backButton;
    [SerializeField] private Animator _backButtonChild;

    [SerializeField] private Button _creditsButton;

    GameObject playerConfig;

    GameObject _fade;

    void Start()
    {
        try
        {
            playerConfig = GameObject.FindWithTag("PlayerConfiguration");
            Destroy(playerConfig.gameObject);
        }
        catch
        {
            return;
        }
    }

    void FixedUpdate()
    {
        if (_fade == null)
        {
            _fade = GameObject.FindWithTag("Fade");
        }
    }

    public void LoadTransition()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playButton.GetComponent<Animator>().SetTrigger("Press");
        _playButtonChild.GetComponentInChildren<Animator>().SetTrigger("Press");
        StartCoroutine(GameFade());
    }

    private IEnumerator GameFade()
    {
        _fade.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Transition_MM2CS");
    }

    public void ExitGame()
    {
        _exitButton.GetComponent<Animator>().SetTrigger("Press");
        _exitButtonChild.GetComponentInChildren<Animator>().SetTrigger("Press");
        Debug.Log(_exitButton.GetComponentInChildren<Animator>());
        Application.Quit();
    }

    // public void LoadCoopGame()
    // {
    //     Time.timeScale = 1f;
    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    //     SceneManager.LoadScene("Valve2Coop");
    // }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _backButton.GetComponent<Animator>().SetTrigger("Press");
        _backButtonChild.GetComponentInChildren<Animator>().SetTrigger("Press");
        StartCoroutine(MenuFade());
    }

    public void OpenSettings()
    {
        _settingsButton.GetComponent<Animator>().SetTrigger("Press");
        _settingsButtonChild.GetComponentInChildren<Animator>().SetTrigger("Press");
        _creditsButton.Select();
    }
    
    public void LoadCredits()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(CreditsFade());
    }
    private IEnumerator MenuFade()
    {
        _fade.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }
    private IEnumerator CreditsFade()
    {
        _fade.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Credits");
    }
}