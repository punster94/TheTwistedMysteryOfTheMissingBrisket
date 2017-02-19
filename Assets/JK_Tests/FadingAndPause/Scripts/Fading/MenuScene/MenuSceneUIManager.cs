using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuSceneUIManager : MonoBehaviour
{
    #region Fields
    public MenuStates currentState = MenuStates.MAIN;

    [Space(10)]
    [Header("REFERENCING CANVAS GROUPS")]
    public CanvasGroup cvsMenu;
    public CanvasGroup cvsInfo;
    public CanvasGroup cvsSettings;
    public GenericBlackFader fader;

    [Space(10)]
    [Header("FADING")]
    public float initialWait = 0.5f;    
    public float fadeSpeed = 1f;

    [Space(10)]
    [Header("NEXT LEVEL")]
    public string[] levelNames;

    
    bool canChangeState = true;
    #endregion

    #region Initialization
    void Awake()
    {
        //Init
        cvsMenu.alpha = 0f;

        //Hide all canvases
        Fader.Canvas_InstantTransparent(GetCanvasBasedOnState(MenuStates.MAIN));
        Fader.Canvas_InstantTransparent(GetCanvasBasedOnState(MenuStates.CREDITS));
        Fader.Canvas_InstantTransparent(GetCanvasBasedOnState(MenuStates.SETTINGS));
    }

    CanvasGroup GetCanvasBasedOnState(MenuStates state)
    {
        switch (state)
        {
            case MenuStates.MAIN:
                return cvsMenu;
            case MenuStates.CREDITS:
                return cvsInfo;
            case MenuStates.SETTINGS:
                return cvsSettings;
            default:
                Debug.Log("ERROR: No such state or canvas group exist.");
                return null;
        }
    }    

    IEnumerator Start ()
    {
        //Fade in menu canvas group
        yield return new WaitForSeconds(initialWait);
        StartCoroutine(Fader.Canvas_FadeToOpaque(cvsMenu, fadeSpeed));
    }
    #endregion


    public void ToMain()
    {
        StartCoroutine(StateChange(MenuStates.MAIN));
    }

    public void ToInfo()
    {
        StartCoroutine(StateChange(MenuStates.CREDITS));
    }

    public void ToSettings()
    {
        StartCoroutine(StateChange(MenuStates.SETTINGS));
    }

    public void ToQuitGame()
    {
        StartCoroutine(DoQuitGame());
    }

    public void ToTargetLevel(int sceneIndex)
    {
        StartCoroutine(GoToLevel(sceneIndex));
    }

    IEnumerator StateChange(MenuStates targetState, float waitTimeBetweenFading = 0f)
    {
        if (currentState == targetState || !canChangeState) //If end and start state are the same, or currently transitioning...
        {
            yield break; //then do not make the transition.
        }

        canChangeState = false;

        StartCoroutine(FadeOutCurrentCanvas());
        yield return new WaitForSeconds(waitTimeBetweenFading);
        StartCoroutine(FadeInNewCanvas(targetState));

        currentState = targetState;

        canChangeState = true;
    }

    IEnumerator GoToLevel (int sceneIndex)
    {
        canChangeState = false;

        StartCoroutine(fader.FadeIn());
        yield return FadeOutCurrentCanvas();
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator FadeOutCurrentCanvas ()
    {
        yield return StartCoroutine(Fader.Canvas_FadeToTransparent(GetCanvasBasedOnState(currentState), fadeSpeed));
    }

    IEnumerator FadeInNewCanvas (MenuStates targetState)
    {
        yield return StartCoroutine(Fader.Canvas_FadeToOpaque(GetCanvasBasedOnState(targetState), fadeSpeed));
    }

    IEnumerator DoQuitGame ()
    {
        yield return StartCoroutine(FadeOutCurrentCanvas());

        Application.Quit();
    }
}

public enum MenuStates
{
    MAIN,
    CREDITS,
    SETTINGS
}