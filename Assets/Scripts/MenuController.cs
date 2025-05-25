using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject tutorialUI;

    [SerializeField] private GameObject ExitButton;


    [SerializeField] private bool isPaused;
    [SerializeField] private bool showTutorial = true;

    private float duration = 1f;
    private float startScale = 1f;
    private Coroutine currentCoroutine;

    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            ExitButton.SetActive(false);
        }
        else
        {
            ExitButton.SetActive(true);
        }
    }

    void Update()
    {
        TutorialUI();
        ExitButton.gameObject.SetActive(false);

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
        SceneManager.LoadScene("Level1");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCredits()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Credits");
    }
    public void QuitGame()
    {
        Application.Quit();
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
