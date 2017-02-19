using UnityEngine;
using System.Collections;

public class Dialog
{
    public Statement startingStatement;
    public ArrayList questions;
    public int personId;
    public TimeState timeState;

    public Dialog(Statement ss, ArrayList q, int id, TimeState t)
    {
        startingStatement = ss;
        questions = q;
        personId = id;
        timeState = t;
    }

    public ArrayList filteredQuestions(ArrayList seenKeywords)
    {
        ArrayList filtered = new ArrayList();

        foreach(Question question in questions)
        {
            if (question.keywordsSatisfied(seenKeywords) && !question.answer.hasBeenSeen)
            {
                filtered.Add(question);
            }
        }

        return filtered;
    }

    public int getQuestionIndex(Question pQuestion) 
    {
        for( int i= 0; i < questions.Count; i++) 
        {
            if( pQuestion == questions[i]) 
            {
                return i;
            }
        }

        return -1;
    }

    public int getFilteredQuestionIndex(Question pQuestion, ArrayList seenKeyword) 
    {
        ArrayList filtered = filteredQuestions(seenKeyword);
        for( int i=0; i < filtered.Count; i++) 
        {
            if( pQuestion == filtered[i] ) 
            {
                return i;
            }
        }
        return -1;
    }
}
