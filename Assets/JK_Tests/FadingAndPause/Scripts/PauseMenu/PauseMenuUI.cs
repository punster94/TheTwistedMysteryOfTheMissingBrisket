using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseMenuUI : MonoBehaviour
{
    public static bool IsPaused = false;

    [SerializeField]
    string menuName;
    [SerializeField]
    float timeBeforeEnabled = 2.5f; //Need to wait for Generic black fader to fade it.

    //Canvas groups
    [SerializeField]
    CanvasGroup pauseMenu;
    [SerializeField]
    CanvasGroup quitConfirmMenu;

    [SerializeField] 
    Scene MenuScene;
    [SerializeField]
    GenericBlackFader blackFader;

    bool transitioning = true;


    #region Public methods
    public void ToStartMenu()
    {
        if (!transitioning)
        {
            TogglePause();
            transitioning = true;
            StartCoroutine(DoBackToMain());
        }
    }

    public void ToQuitConfirm()
    {
        Fader.Canvas_InstantTransparent(pauseMenu);
        Fader.Canvas_InstantOpaque(quitConfirmMenu);
    }

    public void QuitConfirm(bool b)
    {
        if (b)
        {
            Application.Quit();
        }
        else
        {
            Fader.Canvas_InstantTransparent(quitConfirmMenu);
            Fader.Canvas_InstantOpaque(pauseMenu);
        }
    }

    public void TogglePause()
    {
        if (transitioning)
            return;

        IsPaused = !IsPaused;
        print("IsPaused: " + IsPaused);

        if (IsPaused)
        {
            Pause();
        }
        //Unpause
        else
        {
            UnPause();
        }
    }
    #endregion
    
    #region Start Awake 
    void Awake()
    {
        Fader.Canvas_InstantTransparent(pauseMenu);
        Fader.Canvas_InstantTransparent(quitConfirmMenu);
    }

    IEnumerator Start ()
    {
        yield return new WaitForSeconds(timeBeforeEnabled);
        transitioning = false;
    }

    void Update()
    {
        if (!transitioning && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    #endregion

    #region Private methods
    IEnumerator DoBackToMain()
    {
        
        yield return StartCoroutine(blackFader.FadeIn());
        transitioning = false;
        SceneManager.LoadScene(menuName);
    }

    void Pause()
    {
        Fader.Canvas_InstantOpaque(pauseMenu);
        Time.timeScale = 0f;
    }

    void UnPause()
    {
        Time.timeScale = 1f;
        Fader.Canvas_InstantTransparent(pauseMenu);
    }
    #endregion
}