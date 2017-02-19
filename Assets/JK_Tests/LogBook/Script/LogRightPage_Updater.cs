using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LogRightPage_Updater : MonoBehaviour
{
    [SerializeField] Image portrait;
    [SerializeField] Text DetailedDialog;

    public void UpdateContent (Sprite portraitSprite, string DialogueText)
    {
        //print("---LogRightPage_Updater.UpdateContent() portraitSprite: " + portraitSprite + ", DialogueText: " + DialogueText);
        portrait.sprite = portraitSprite;
        DetailedDialog.text = DialogueText;
    }
}