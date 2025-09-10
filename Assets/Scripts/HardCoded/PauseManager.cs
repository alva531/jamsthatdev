using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class PauseManager : MonoBehaviour
{
    private InputActions inputActions;
    private Coroutine currentCoroutine;

    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject pauseUI;

    [SerializeField] private bool isPaused = false;

    [SerializeField] private Button resumeButton;

    private float duration = 1f;
    private float startScale = 1f;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Pause.Enable();
        inputActions.Pause.Newaction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        inputActions.Pause.Newaction.performed -= OnPausePerformed;
        inputActions.Pause.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        gameplayUI.SetActive(false);
        pauseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        resumeButton.Select();
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
    public void LoadMenu()
    {
        isPaused = false;
        gameplayUI.SetActive(false);
        pauseUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(LoadMenuRoutine());
    }

    public void LoadCredits()
    {
        currentCoroutine = StartCoroutine(LoadCreditsRoutine());
    }

    private IEnumerator LoadMenuRoutine()
    {
        Animator fade = GameObject.FindWithTag("Fade").GetComponent<Animator>();
        yield return StartCoroutine(SpeedUpTime());

        Time.timeScale = 1f;

        fade.SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator LoadCreditsRoutine()
    {
        Animator fade = GameObject.FindWithTag("Fade").GetComponent<Animator>();

        fade.SetTrigger("Out");
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
