using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DetectiveWallPost : MonoBehaviour
{
    //Randomchange
    [SerializeField] bool debugMode;
    [SerializeField] int personID;
    [SerializeField] string keywords;
    [SerializeField] DialogManager dialogManager;
    Image uiImage;
    Text uiText;
    bool seen = false;

	void Start ()
    {
        uiText = GetComponentInChildren<Text>();
        uiImage = GetComponent<Image>();

        uiText.enabled = false;
        uiImage.enabled = false;
    }

    void OnEnable ()
    {
        LogManager.DetectiveWallOpens += UpdateContent;
    }

    void OnDisable ()
    {
        LogManager.DetectiveWallOpens -= UpdateContent;
    }

    void UpdateContent ()
    {
        if (!seen && dialogManager.keywordHasBeenSeen(keywords))
        {
            uiText.text = keywords;
            uiText.enabled = true;
            uiImage.enabled = true;

            seen = true;
        }
    }
}