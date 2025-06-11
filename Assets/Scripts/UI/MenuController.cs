using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject tutorialUI;



    [SerializeField] private bool isPaused;
    [SerializeField] private bool showTutorial = true;



    [SerializeField] private Button _playButton;
    [SerializeField] private Animator _playButtonChild;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Animator _settingsButtonChild;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Animator _exitButtonChild;

    [SerializeField] private Button _backButton;
    [SerializeField] private Animator _backButtonChild;

    [SerializeField] private Button _creditsButton;


    private float duration = 1f;
    private float startScale = 1f;
    private Coroutine currentCoroutine;

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

        if (isPaused = true)
        {
            isPaused = false;
        }
    }

    void FixedUpdate()
    {
        TutorialUI();

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);

            if (!isPaused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        if (_fade == null)
        {
            _fade = GameObject.FindWithTag("Fade");
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        gameplayUI.SetActive(false);
        pauseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentCoroutine = StartCoroutine(SlowDownTime());
    }

    public void ResumeGame()
    {
        isPaused = false;
        gameplayUI.SetActive(true);
        pauseUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentCoroutine = StartCoroutine(SpeedUpTime());
    }

    public void TutorialUI()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showTutorial = !showTutorial;

            if (showTutorial == true)
            {
                tutorialUI.gameObject.SetActive(true);
            }
            if (showTutorial == false)
            {
                tutorialUI.gameObject.SetActive(false);
            }
        }
    }

    public void LoadGame()
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
        SceneManager.LoadScene("CharacterSelection");
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
    private IEnumerator MenuFade()
    {
        _fade.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
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

    private IEnumerator CreditsFade()
    {
        _fade.GetComponent<Animator>().SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Credits");
    }

    private IEnumerator SlowDownTime()
    {
        float elapsed = 0f;

        while (Time.timeScale > 0.01f)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, 0f, elapsed / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 0f;
    }

    private IEnumerator SpeedUpTime()
    {
        float elapsed = 0f;

        while (Time.timeScale < 0.99f)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0f, startScale, elapsed / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
    }
}