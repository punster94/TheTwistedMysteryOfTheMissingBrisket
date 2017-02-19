using UnityEngine;
using System.Collections;

public class Question
{
    public ArrayList enablingKeywords;
    public Statement question;
    public Statement answer;

    public Question(ArrayList enablers, Statement q, Statement a)
    {
        enablingKeywords = enablers;
        question = q;
        answer = a;
    }

    public bool keywordsSatisfied(ArrayList seenKeywords)
    {
        if (seenKeywords.Contains(Keyword.getKeyword("Every Keyword")))
            return true;

        foreach (Keyword enabler in enablingKeywords)
        {
            if (!seenKeywords.Contains(enabler) && enabler.text != "")
                return false;
        }
        
        return true;
    }
}
