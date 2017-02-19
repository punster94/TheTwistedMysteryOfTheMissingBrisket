using UnityEngine;
using System.Collections;

public class Statement
{
    public ArrayList containedKeywords;
    public string displayText;
    public string shortenedText;
    public bool hasBeenSeen;
    public Keyword enablingKeyword;

    private string keywordColor = "green";
    private string selectedColor = "yellow";
    private string selectedKeywordColor = "#9acd32";
    private string transitionStatementColor = "red";

    public Statement(ArrayList contained, string dt, string st, Keyword ek = null)
    {
        containedKeywords = contained;
        displayText = dt;
        shortenedText = st;
        hasBeenSeen = false;
        enablingKeyword = ek;
    }

    public bool keywordSatisfied(ArrayList seenKeywords)
    {
        return enablingKeyword == null || enablingKeyword.text == "" || seenKeywords.Contains(enablingKeyword) || seenKeywords.Contains(Keyword.getKeyword("Every Keyword"));
    }

    public string transitionStatementText()
    {
        string colorizedText = "<color=" + transitionStatementColor + ">" + displayText + "</color>";

        foreach (Keyword keyword in containedKeywords)
        {
            if (keyword.text != "")
                colorizedText = colorizedText.Replace(keyword.text, "</color><color=" + keywordColor + ">" + keyword.text + "</color><color=" + transitionStatementColor + ">");
        }

        return colorizedText;
    }

    public string colorizedShortenedText()
    {
        string colorizedText = "<color=" + selectedColor + ">" + shortenedText + "</color>";
        
        foreach(Keyword keyword in containedKeywords)
        {
            if (keyword.text != "")
                colorizedText = colorizedText.Replace(keyword.text, "</color><color=" + selectedKeywordColor + ">" + keyword.text + "</color><color=" + selectedColor + ">");
        }

        return colorizedText;
    }

    public string highlightedDisplayText()
    {
        return highlightString(displayText);
    }

    public string highlightedShortenedText()
    {
        return highlightString(shortenedText);
    }

    private string highlightString(string stringToHighlight)
    {
        string highlightedText = stringToHighlight;

        foreach (Keyword keyword in containedKeywords)
        {
            if (keyword.text != "")
                highlightedText = highlightedText.Replace(keyword.text, "<color=" + keywordColor + ">" + keyword.text + "</color>");
        }

        return highlightedText;
    }
}
