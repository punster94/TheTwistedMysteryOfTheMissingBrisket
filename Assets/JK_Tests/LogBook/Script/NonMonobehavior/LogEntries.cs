using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class LogEntry
{


    public string ShortenedText;
    public string FullText;
    public int IndexInList;

    public LogEntry(string shortenedText, string fullText, int index = 0)
    {
        ShortenedText = shortenedText;
        FullText = fullText;
        IndexInList = index;
    }    
}