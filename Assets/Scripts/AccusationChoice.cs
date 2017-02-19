using UnityEngine;
using System.Collections;

public class AccusationChoice
{
    public int personId;
    public int defenderId;

    public ArrayList enablingKeywords;
    public ArrayList statementPairs;
    public ArrayList investigatorIntuitions;

    public AccusationChoice(int pi, int di, ArrayList eks, ArrayList sps, ArrayList ii)
    {
        personId = pi;
        defenderId = di;
        enablingKeywords = eks;
        statementPairs = sps;
        investigatorIntuitions = ii;
    }

    public bool satisfiesKeywords(ArrayList seenKeywords)
    {
        if (seenKeywords.Contains(Keyword.getKeyword("Every Keyword")))
            return true;

        foreach(Keyword enabler in enablingKeywords)
        {
            if (!seenKeywords.Contains(enabler))
                return false;
        }

        return true;
    }

    public Statement furthestIntuition(ArrayList seenKeywords)
    {
        Statement furthest = null;

        foreach (Statement intuition in investigatorIntuitions)
        {
            if (intuition.keywordSatisfied(seenKeywords))
                furthest = intuition;
            else
                break;
        }

        return furthest;
    }
}
