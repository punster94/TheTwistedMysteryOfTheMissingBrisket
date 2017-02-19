using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class LogManager : MonoBehaviour
{
    public enum DetectiveCursorMode { Idle, Pinning, MovingPin };

    public delegate void DetectiveWallOpenHandler();
    public static event DetectiveWallOpenHandler DetectiveWallOpens;

    public static Color[] ToggleColors =
    {
        Color.white,
        new Color(1f, 0.434f, 0.434f),
        new Color(0.455f, 1f, 0.706f)
    };

    [Header("REFERENCES")]
    [SerializeField]
    CanvasGroup logCanvasGroup;
    [SerializeField]
    CanvasGroup detectiveWallCanvasGroup;
    [SerializeField]
    Animator centerAnimator;
    [SerializeField]
    Animator detectiveWallAnimator;
    [SerializeField]
    Animator pageFlipAnimator;
    [SerializeField]
    GameObject leftButton;
    [SerializeField]
    GameObject rightButton;
    [SerializeField]
    GameObject pinStringPf;

    [Space(20)]
    [Header("PAGES")]
    [SerializeField]
    LogLeftPage_Updater actualLeftPage;
    [SerializeField]
    LogRightPage_Updater actualRightPage;
    [SerializeField]
    LogLeftPage_Updater fakeLeftPage;
    [SerializeField]
    LogRightPage_Updater fakeRightPage;
    [SerializeField]
    Transform flippingPageParent_CurrentPage;
    [SerializeField]
    Transform flippingPageParent_NextPage;

    [Space(20)]
    [Header("LOG CONTENTS")]
    [SerializeField]
    List<string> personNames;
    [SerializeField]
    List<Sprite> personHeadshots;

    Dictionary<int, List<LogEntry>> allLog = new Dictionary<int, List<LogEntry>>();
    public DetectiveCursorMode CursorMode { get; private set; }

    bool DetectiveWallOpened = false;

    //Flip page
    const float pageFlipAnimationDuration = 0.5f; //Currently this is hardcoded, so when you speed up/down the flip speed, we need to update here.
    const int finalPersonID = 11;
    int activePersonID = 0;
    bool canFlip = true;
    int activeLeftPageEntry = 0;
    bool isLogOpen = false;

    //Fading
    const float logFadeDuration = 0.3f;
    bool logFadeToggle_FadeInNext = true;

    bool logCanFade = true;

    //Pin    
    PingString activePinStringGrp;

    void Start()
    {
        DialogManager.OnAddLogEntry += AddLogEntry;
        LogLeftPage_SingleEntry.OnEntryButtonPressed += ButtonClicked;

        CursorMode = DetectiveCursorMode.Idle;
        logCanvasGroup.alpha = 0;
        UpdateArrowsVisibility();
    }

    void Update()
    {
        if (DetectiveWallOpened)
        {
            DetectiveWallUpdate();
        }

        //Debug
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

        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleDetectiveWallVisibility();
        }
    }

    #region Pinning
    public void MovePin(PingString pinGrp)
    {
        activePinStringGrp = pinGrp;
        CursorMode = DetectiveCursorMode.MovingPin;
    }

    public void ToggleDetectiveWallVisibility()
    {
        if (!DetectiveWallOpened)
        {
            detectiveWallAnimator.Play("Open");
            detectiveWallCanvasGroup.interactable = true;
            detectiveWallCanvasGroup.blocksRaycasts = true;
            DetectiveWallOpened = true;
            if (DetectiveWallOpens != null)
            {
                DetectiveWallOpens();
            }
        }
        else
        {
            detectiveWallAnimator.Play("Close");
            detectiveWallCanvasGroup.interactable = false;
            detectiveWallCanvasGroup.blocksRaycasts = false;
            DetectiveWallOpened = false;
            DestroyActivePins();
        }

        CursorMode = DetectiveCursorMode.Idle;
        activePinStringGrp = null;
    }

    void DetectiveWallUpdate()
    {
        //Pinning
        if (CursorMode == DetectiveCursorMode.Idle || CursorMode == DetectiveCursorMode.Pinning)
        {
            //Try pinning
            if (Input.GetMouseButtonDown(0))
            {
                TryPinning();
            }
            //Change color
            else if (Input.GetMouseButtonDown(1))
            {
                if (activePinStringGrp != null)
                {
                    activePinStringGrp.ToggleColor();
                }
            }
        }
        //Destroy pin
        else if (CursorMode == DetectiveCursorMode.Pinning && Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyActivePins();
            CursorMode = DetectiveCursorMode.Idle;
        }
        //Moving pin
        else if (CursorMode == DetectiveCursorMode.MovingPin)
        {
            //Finish moving
            if (Input.GetMouseButtonUp(0))
            {
                //maybe put these in a seperate TryFinishMovePin() method
                activePinStringGrp.FinishMoving();
                activePinStringGrp = null;
                CursorMode = DetectiveCursorMode.Idle;
            }
            //Change color
            else if (Input.GetMouseButtonDown(1))
            {
                activePinStringGrp.ToggleColor();
            }
            //Destroy
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyActivePins();
            }
        }
    }

    void TryPinning()
    {
        //Do a raycast to see what kind of object is being hit. 
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);
        //print("raycastResults.Count: " + raycastResults.Count);

        foreach (var obj in raycastResults)
        {
            if ((CursorMode == DetectiveCursorMode.Idle && obj.gameObject.tag == "DWall_Pins") ||
                obj.gameObject.tag == "DWall_QuitButton")
            {
                return;
            }
        }

        if (CursorMode != DetectiveCursorMode.Pinning)
        {
            //print("Start");

            GameObject pinStringGo = Instantiate(pinStringPf, Input.mousePosition, Quaternion.identity, detectiveWallCanvasGroup.transform) as GameObject;
            activePinStringGrp = pinStringGo.GetComponent<PingString>();
            activePinStringGrp.BeginPlacement(this);
            CursorMode = DetectiveCursorMode.Pinning;
        }
        else if (CursorMode == DetectiveCursorMode.Pinning && activePinStringGrp != null)
        {
            //print("Finished");

            activePinStringGrp.FinishPlacement();
            activePinStringGrp = null;
            CursorMode = DetectiveCursorMode.Idle;
        }
    }

    void DestroyActivePins()
    {
        if (activePinStringGrp != null)
        {
            activePinStringGrp.DestroyAll();
            activePinStringGrp = null;
            CursorMode = DetectiveCursorMode.Idle;
        }
    }
    #endregion

    #region Page Content Update 
    void UpdateSpecifiedLeftPage(LogLeftPage_Updater leftPage, int personID, float scrollbarValue)
    {
        //print("UpdateSpecifiedLeftPage. personID: " + personID + " , activePersonID: " + activePersonID);
        List<LogEntry> entries = new List<LogEntry>();
        if (allLog.ContainsKey(personID))
        {
            entries = allLog[personID];
        }

        leftPage.UpdateContent(personNames[personID], entries, scrollbarValue);
    }

    void UpdateSpecifiedRightPage(LogRightPage_Updater rightPage, int personID, int activeEntryIndex = 0)
    {
        //print("UpdateSpecifiedRightPage. personID: " + personID + " , activePersonID: " + activePersonID);
        string fullText = "";

        if (allLog.ContainsKey(personID) && allLog[personID].Count > 0)
        {
            try
            {
                fullText = allLog[personID][activeEntryIndex].FullText;
            }
            catch (Exception e)
            {
                print("ERROR: personID: " + personID + ", activeEntryIndex: " + activeEntryIndex + ", avaliable entry: " + allLog[personID].Count);
            }
        }

        rightPage.UpdateContent(personHeadshots[personID], fullText);
    }

    void AddLogEntry(int personID, string shortenedText, string fullText)
    {
        //Add a new Dictionary entry if personID (i.e. the Key) does not already exist in the Dictionary.
        if (!allLog.ContainsKey(personID))
        {
            allLog.Add(personID, new List<LogEntry>());
        }

        //Check if there is a duplicate entry in log. 
        foreach (LogEntry entry in allLog[personID])
        {
            if (entry.ShortenedText == shortenedText)
            {
                print("LogEntry already exist. personID: " + personID + ", shortenedText" + shortenedText + ", fullText: " + fullText);
                return;
            }
        }

        //print("AddLogEntry. personID: " + personID + ", shortenedText" + shortenedText + ", fullText: " + fullText);

        activeLeftPageEntry = allLog[personID].Count;
        allLog[personID].Add(new LogEntry(shortenedText, fullText, activeLeftPageEntry));
        UpdateSpecifiedLeftPage(actualLeftPage, activePersonID, 1f);
        UpdateSpecifiedRightPage(actualRightPage, activePersonID, activeLeftPageEntry);
    }

    void ButtonClicked(int pressedEntryButtonIndex)
    {
        //print("pressedEntryButtonIndex: " + pressedEntryButtonIndex);
        activeLeftPageEntry = pressedEntryButtonIndex;
        UpdateSpecifiedRightPage(actualRightPage, activePersonID, activeLeftPageEntry);
    }
    #endregion

    #region LOG TOGGLE
    public void ToggleLogVisibility()
    {
        if (logCanFade)
        {
            isLogOpen = !isLogOpen;
            Cursor.visible = isLogOpen;
            Cursor.lockState = isLogOpen ? CursorLockMode.None : CursorLockMode.Locked;
            StartCoroutine(DoLogFade());
        }
    }

    IEnumerator DoLogFade()
    {
        logCanFade = false;
        if (logFadeToggle_FadeInNext)
        {
            UpdateSpecifiedLeftPage(actualLeftPage, activePersonID, 1f);
            UpdateSpecifiedRightPage(actualRightPage, activePersonID, activeLeftPageEntry);

            centerAnimator.Play("Expand");
            while (logCanvasGroup.alpha < 1)
            {
                logCanvasGroup.alpha += (1 / logFadeDuration) * Time.deltaTime;
                yield return null;
            }

            logCanvasGroup.alpha = 1;
        }
        else
        {
            centerAnimator.Play("Shrink");
            while (logCanvasGroup.alpha > 0)
            {
                logCanvasGroup.alpha -= (1 / logFadeDuration) * Time.deltaTime;
                yield return null;
            }

            logCanvasGroup.alpha = 0;
        }

        logCanFade = true;
        logFadeToggle_FadeInNext = !logFadeToggle_FadeInNext;
    }
    #endregion

    #region PAGE FLIPPING
    public void NextPage()
    {
        StartCoroutine(DoFlipNextPage());
    }

    public void PreviousPage()
    {
        StartCoroutine(DoFlipPreviousPage());
    }

    IEnumerator DoFlipNextPage()
    {
        if (canFlip && activePersonID < finalPersonID) //Flipping from right to left.
        {
            canFlip = false;
            activePersonID++;
            activeLeftPageEntry = 0;

            //1. Update the fakeRightPage to match RightPage
            //   Update the fakeLeftPage to the NEXT-LeftPage's content

            UpdateSpecifiedRightPage(fakeRightPage, activePersonID - 1, activeLeftPageEntry);
            UpdateSpecifiedLeftPage(fakeLeftPage, activePersonID, 1f);

            //2 Play the fakePage flip animation. 
            fakeRightPage.transform.parent = flippingPageParent_CurrentPage;
            fakeRightPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            fakeLeftPage.transform.parent = flippingPageParent_NextPage;
            fakeLeftPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            pageFlipAnimator.Play("RightToLeft");
            //Right page is now covered by the page flip.
            UpdateSpecifiedRightPage(actualRightPage, activePersonID);
            yield return new WaitForSeconds(pageFlipAnimationDuration);

            // 3.The LeftPage is now being covered. Now update it to the new content.
            UpdateSpecifiedLeftPage(actualLeftPage, activePersonID, 1f);
            pageFlipAnimator.Play("Hide");

            UpdateArrowsVisibility();
            leftButton.SetActive(true);
            canFlip = true;

        }
    }

    IEnumerator DoFlipPreviousPage()
    {
        if (canFlip && activePersonID > 0) //Flipping from left to right
        {
            canFlip = false;
            activePersonID--;

            //1. Update the fakeLeftPage to match LeftPage(current-state)
            //   Update the fakeRightPage to the RightPage(next-state)'s content
            UpdateSpecifiedLeftPage(fakeLeftPage, activePersonID + 1, actualLeftPage.ScrollbarValue);
            UpdateSpecifiedRightPage(fakeRightPage, activePersonID);

            //2 Play the fakePage flip animation and then instantly update the actualLeftPage (now covered) to next-state.
            fakeRightPage.transform.parent = flippingPageParent_NextPage;
            fakeRightPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            fakeLeftPage.transform.parent = flippingPageParent_CurrentPage;
            fakeLeftPage.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

            pageFlipAnimator.Play("LeftToRight");
            UpdateSpecifiedLeftPage(actualLeftPage, activePersonID, 1f);
            yield return new WaitForSeconds(pageFlipAnimationDuration);

            // 3.The Right is now being covered. Now update it to the new content.
            UpdateSpecifiedRightPage(actualRightPage, activePersonID);
            pageFlipAnimator.Play("Hide");

            UpdateArrowsVisibility();
            rightButton.SetActive(true);
            canFlip = true;
        }
    }

    void UpdateArrowsVisibility()
    {
        if (activePersonID >= finalPersonID)
        {
            rightButton.SetActive(false);
        }
        if (activePersonID <= 0)
        {
            leftButton.SetActive(false);
        }
    }
    #endregion

    #region CleanUp
    void OnDisable()
    {
        DialogManager.OnAddLogEntry -= AddLogEntry;
        LogLeftPage_SingleEntry.OnEntryButtonPressed -= ButtonClicked;
    }
    #endregion
}

