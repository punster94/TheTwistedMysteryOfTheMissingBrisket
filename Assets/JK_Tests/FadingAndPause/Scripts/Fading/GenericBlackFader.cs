using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class GenericBlackFader : MonoBehaviour, IFade
{
    [SerializeField]
    bool fadeInOnSceneStart = true;
    [SerializeField]
    float initialWaitTime = 1f;
    [SerializeField]
    float fadeSpeed = 1f;
    [SerializeField]
    CanvasGroup cvsGrp;
    
    IEnumerator fadeToOpaque;
    IEnumerator fadeToTransparent;

    void ReferenceCoroutines ()
    {
        fadeToOpaque = Fader.Canvas_FadeToOpaque(cvsGrp, fadeSpeed);
        fadeToTransparent = Fader.Canvas_FadeToTransparent(cvsGrp, fadeSpeed);
    }

    public IEnumerator FadeIn()
    {
        ReferenceCoroutines();
        StopCoroutine("FadeOut");
        StopCoroutine(fadeToTransparent);

        yield return StartCoroutine(fadeToOpaque);
    }

    public IEnumerator FadeOut()
    {
        ReferenceCoroutines();
        StopCoroutine("FadeIn");
        StopCoroutine(fadeToOpaque);

        yield return StartCoroutine(fadeToTransparent);
    }

    public void InstantFadeIn()
    {
        Fader.Canvas_InstantOpaque(cvsGrp);
    }

    public void InstantFadeOut()
    {
        Fader.Canvas_InstantTransparent(cvsGrp);
    }

    #region Mono
    void Awake ()
    {
        if (cvsGrp == null)
        {
            cvsGrp = GetComponent<CanvasGroup>();
        }        
    }

    IEnumerator Start()
    {
        if (fadeInOnSceneStart)
        {
            InstantFadeIn();
            yield return new WaitForSeconds(initialWaitTime);
            StartCoroutine("FadeOut");
        }
    }

    //void Update ()
    //{
    //    //Debug
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        StartCoroutine(FadeIn());
    //    }
    //}
    #endregion
}