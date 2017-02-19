using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class Fader
{
    //Canvas
    public static IEnumerator Canvas_FadeToOpaque (CanvasGroup cvsGrp, float fadeSpd)
    {
        while (cvsGrp.alpha < 1)
        {
            
            cvsGrp.alpha += Time.deltaTime * fadeSpd;
            yield return null;
        }

        cvsGrp.alpha = 1f;

        cvsGrp.blocksRaycasts = true;
        cvsGrp.interactable = true;
    }

    public static IEnumerator Canvas_FadeToTransparent (CanvasGroup cvsGrp, float fadeSpd)
    {
        cvsGrp.blocksRaycasts = false;
        cvsGrp.interactable = false;
        Debug.Log("PRE cvsGrp.alpha: " + cvsGrp.alpha);
        while (cvsGrp.alpha > 0)
        {
            cvsGrp.alpha -= Time.deltaTime * fadeSpd;
            yield return null;
        }
        Debug.Log("POST cvsGrp.alpha: " + cvsGrp.alpha);
        cvsGrp.alpha = 0f;
    }

    public static void Canvas_InstantTransparent (CanvasGroup cvsGrp)
    {
        Debug.Log("Canvas_InstantTransparent");
        cvsGrp.blocksRaycasts = false;
        cvsGrp.interactable = false;
        cvsGrp.alpha = 0f;
    }

    public static void Canvas_InstantOpaque(CanvasGroup cvsGrp)
    {
        cvsGrp.blocksRaycasts = true;
        cvsGrp.interactable = true;
        cvsGrp.alpha = 1f;
    }

    //UI Image
    public static IEnumerator Image_Fade(bool fadeToOpaque, Image image, float fadeSpd)
    {
        Color c = image.color;
        if (fadeToOpaque)
        {
            while (c.a < 1f)
            {
                c.a += fadeSpd * Time.deltaTime;
                image.color = c;
                yield return null;
            }
            c.a = 1f;
            image.color = c;
        }
        //Fade to clear
        else
        {
            while (c.a > 0f)
            {
                c.a -= fadeSpd * Time.deltaTime;
                image.color = c;
                yield return null;
            }
            c.a = 0f;
            image.color = c;
        }
    }

    public static void Image_Instant(bool fadeToOpaque, Image image)
    {
        Color c = image.color;
        if (fadeToOpaque)
        {
            c.a = 1f;
            image.color = c;
        }
        //Fade to clear
        else
        {
            c.a = 0f;
            image.color = c;
        }
    }
}
