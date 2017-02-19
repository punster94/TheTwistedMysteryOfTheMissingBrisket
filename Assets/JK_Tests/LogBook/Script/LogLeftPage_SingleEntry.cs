using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogLeftPage_SingleEntry : MonoBehaviour
{
    public delegate void EntryButtonPressed(int pressedEntryButtonIndex);
    public static event EntryButtonPressed OnEntryButtonPressed;

    [SerializeField] Text abbreviation;
    LogEntry logEntry;
    int indexInList;

    public void InitializeContent (LogEntry newEntry, int index)
    {
        logEntry = newEntry;
        abbreviation.text = newEntry.ShortenedText;
        indexInList = index;
    }    

    public void Pressed()
    {
        if (OnEntryButtonPressed != null)
            OnEntryButtonPressed(indexInList);
    }
}