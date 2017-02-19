using UnityEngine;
using System.Collections;

public class Accusation
{
    public ArrayList choices;
    public TimeState timeState;
    public Statement transitionStatement;

    private int filteredSize;
    private ArrayList lastFiltered;

    public Accusation(ArrayList c, TimeState t, Statement ts)
    {
        choices = c;
        timeState = t;
        transitionStatement = ts;
    }

    public ArrayList filteredChoices(ArrayList seenKeywords)
    {
        ArrayList filtered = new ArrayList();

        foreach (AccusationChoice choice in choices)
        {
            filtered.Add(choice);

            foreach (Keyword enabler in choice.enablingKeywords)
            {
                if (!(seenKeywords.Contains(enabler) || enabler.text == "" || seenKeywords.Contains(Keyword.getKeyword("Every Keyword"))))
                {
                    filtered.Remove(choice);
                    break;
                }
            }
        }

        filteredSize = filtered.Count;
        lastFiltered = filtered;

        return filtered;
    }

    public int sizeOfLastFilteredChoices()
    {
        return filteredSize;
    }

    public AccusationChoice getAccusationChoice(int indexIntoFiltered)
    {
        if (lastFiltered == null)
            lastFiltered = choices;

        if (lastFiltered.Count > indexIntoFiltered)
            return (AccusationChoice)lastFiltered[indexIntoFiltered];

        return null;
    }
}
