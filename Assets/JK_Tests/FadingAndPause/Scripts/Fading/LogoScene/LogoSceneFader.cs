using UnityEngine;
using System.Collections;

public class LogoSceneFader : MonoBehaviour
{
    public CanvasGroup cvsGrp;
    public string nextSceneName = "Scene_Menu";
    public float fadeSpeed = 1f;

    [Header("FADING")]
    public float initialWait = 0.5f;
    public float stayVisibleDuration = 1.5f;


    void Awake ()
    {
        cvsGrp.alpha = 0f;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(initialWait);
        yield return StartCoroutine(Fader.Canvas_FadeToOpaque(cvsGrp, fadeSpeed));
        yield return new WaitForSeconds(stayVisibleDuration);
        yield return StartCoroutine(Fader.Canvas_FadeToTransparent(cvsGrp, fadeSpeed));
        Application.LoadLevel(nextSceneName);
    }
}
