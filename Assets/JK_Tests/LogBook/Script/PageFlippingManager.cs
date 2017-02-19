using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PageFlippingManager : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] CanvasGroup logCanvasGroup;
    [SerializeField] Animator centerAnimator;
    [SerializeField] Animator pageFlipAnimator;
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;

    [Space(20)]
    [Header("PAGES")]
    [SerializeField] LogLeftPage_Updater actualLeftPage;
    [SerializeField] LogRightPage_Updater actualRightPage;
    [SerializeField] LogLeftPage_Updater fakeLeftPage;
    [SerializeField] LogRightPage_Updater fakeRightPage;
    [SerializeField] Transform flippingPageParent_CurrentPage;
    [SerializeField] Transform flippingPageParent_NextPage;

    [Space(20)]
    [Header("LOG CONTENTS")]
    [SerializeField] string[] personNames;
    [SerializeField] Sprite[] profileHeadshots;

    const float pageFlipAnimationDuration = 0.5f; //Currently this is hardcoded, so when you speed up/down the flip speed, we need to update here.
    const int finalSuspectNumber = 5;
    int curSuspectNumber = 0;
    bool canFlip = true;

    const float logFadeDuration = 0.3f;
    bool logFadeToggle_FadeInNext = true;
    bool logCanFade = true;

    void Start ()
    {
        logCanvasGroup.alpha = 0;
        UpdateArrowsVisibility();

        /////actualRightPage.UpdateContent(profileHeadshots[curSuspectNumber], "Temporary dialogue");
        /////actualLeftPage.UpdateContent(personNames[curSuspectNumber]);
    }

    void Update ()
    {
	    if (Input.GetKeyDown(KeyCode.B))
        {
            PreviousPage();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            NextPage();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleLogVisibility();
        }
    }

    #region LOG FADING
    public void ToggleLogVisibility()
    {
        if (logCanFade)
        {
            StartCoroutine(DoLogFade());
        }
    }

    IEnumerator DoLogFade ()
    {
        logCanFade = false;
        if (logFadeToggle_FadeInNext)
        {
            centerAnimator.Play("Expand");
            while (logCanvasGroup.alpha < 1)
            {
                logCanvasGroup.alpha += (1/ logFadeDuration) * Time.deltaTime;
                yield return null;
            }

            logCanvasGroup.alpha = 1;
        }
        else
        {
            centerAnimator.Play("Shrink");
            while (logCanvasGroup.alpha > 0)
            {
                logCanvasGroup.alpha -= (1/ logFadeDuration) * Time.deltaTime;
                yield return null;
            }

            logCanvasGroup.alpha = 0;
        }
        
        logCanFade = true;
        logFadeToggle_FadeInNext = !logFadeToggle_FadeInNext;
    }
    #endregion

    #region PAGE FLIPPING
    public void NextPage ()
    {
        /////StartCoroutine(DoFlipNextPage());
    }

    public void PreviousPage()
    {
        /////StartCoroutine(DoFlipPreviousPage());
    }

    IEnumerator DoFlipNextPage()
    {
        if (canFlip && curSuspectNumber < finalSuspectNumber) //Flipping from right to left.
        {
            canFlip = false;
            curSuspectNumber++;

            //1. Update the fakeRightPage to match RightPage
            //   Update the fakeLeftPage to the NEXT-LeftPage's content
            fakeRightPage.UpdateContent(profileHeadshots[curSuspectNumber - 1], "Temporary dialogue");
            /////fakeLeftPage.UpdateContent(personNames[curSuspectNumber]);

            //2 Play the fakePage flip animation. 
            fakeRightPage.transform.parent = flippingPageParent_CurrentPage;
            fakeRightPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            fakeLeftPage.transform.parent = flippingPageParent_NextPage;
            fakeLeftPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            pageFlipAnimator.Play("RightToLeft");
            actualRightPage.UpdateContent(profileHeadshots[curSuspectNumber], "Temporary dialogue");
            yield return new WaitForSeconds(pageFlipAnimationDuration);

            // 3.The LeftPage is now being covered. Now update it to the new content.
            /////actualLeftPage.UpdateContent(personNames[curSuspectNumber]);
            pageFlipAnimator.Play("Hide");

            UpdateArrowsVisibility();
            leftButton.SetActive(true);
            canFlip = true;
        }        
    }

    IEnumerator DoFlipPreviousPage()
    {
        if (canFlip && curSuspectNumber > 0) //Flipping from left to right
        {
            canFlip = false;
            curSuspectNumber--;

            //1. Update the fakeLeftPage to match LeftPage(current-state)
            //   Update the fakeRightPage to the RightPage(next-state)'s content
            /////fakeLeftPage.UpdateContent(personNames[curSuspectNumber + 1]);
            fakeRightPage.UpdateContent(profileHeadshots[curSuspectNumber], "Temporary dialogue");

            //2 Play the fakePage flip animation and then instantly update the actualLeftPage (now covered) to next-state.
            fakeRightPage.transform.parent = flippingPageParent_NextPage;
            fakeRightPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            fakeLeftPage.transform.parent = flippingPageParent_CurrentPage;
            fakeLeftPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            pageFlipAnimator.Play("LeftToRight");
            /////actualLeftPage.UpdateContent(personNames[curSuspectNumber]);
            yield return new WaitForSeconds(pageFlipAnimationDuration);

            // 3.The Right is now being covered. Now update it to the new content.
            actualRightPage.UpdateContent(profileHeadshots[curSuspectNumber], "Temporary dialogue");
            pageFlipAnimator.Play("Hide");

            UpdateArrowsVisibility();
            rightButton.SetActive(true);
            canFlip = true;
        }
    }

    void UpdateArrowsVisibility ()
    {
        if (curSuspectNumber >= finalSuspectNumber)
        {
            rightButton.SetActive(false);
        }
        if (curSuspectNumber <= 0)
        {
            leftButton.SetActive(false);
        }
    }


    #endregion
}