using UnityEngine;
using System.Collections;
using System.Xml;

public static class DialogParser
{
    public static ArrayList parse(string xmlText)
    {
        ArrayList dialogues = new ArrayList();

        XmlDocument dialogFile = new XmlDocument();
        dialogFile.LoadXml(xmlText);

        foreach(XmlNode dialog in dialogFile.DocumentElement.ChildNodes)
        {
            int personId = System.Int32.Parse(dialog.Attributes["personId"].InnerText);
            Statement startingStatement = null;
            ArrayList questions = new ArrayList();
            TimeState timeState = (TimeState)(System.Int32.Parse(dialog.Attributes["timeState"].InnerText));

            foreach(XmlNode question in dialog.ChildNodes)
            {
                if (question.Name == "Statement")
                {
                    startingStatement = generateStatement(question);
                }
                else
                {
                    ArrayList enablingKeywords = new ArrayList();
                    Statement questionStatement = null, answerStatement = null;

                    foreach(XmlNode questionComponent in question.ChildNodes)
                    {
                        switch(questionComponent.Name)
                        {
                            case "EnablingKeyword":
                                enablingKeywords.Add(Keyword.getKeyword(questionComponent.InnerText));
                                break;
                            case "QuestionStatement":
                                questionStatement = generateStatement(questionComponent);
                                break;
                            case "AnswerStatement":
                                answerStatement = generateStatement(questionComponent);
                                break;
                        }
                    }

                    questions.Add(new Question(enablingKeywords, questionStatement, answerStatement));
                }
            }

            dialogues.Add(new Dialog(startingStatement, questions, personId, timeState));
        }

        return dialogues;
    }

    public static Statement generateStatement(XmlNode node)
    {
        string ssText = "", shortText = "";
        Keyword enablingKeyword = null;
        ArrayList keywords = new ArrayList();

        foreach (XmlNode keyword in node.ChildNodes)
        {
            switch(keyword.Name)
            {
                case "Text":
                    ssText = keyword.InnerText;
                    break;
                case "ShortenedText":
                    shortText = keyword.InnerText;
                    break;
                case "EnablingKeyword":
                    enablingKeyword = Keyword.getKeyword(keyword.InnerText);
                    break;
                default:
                    keywords.Add(Keyword.getKeyword(keyword.InnerText));
                    break;
            }
        }

        return new Statement(keywords, ssText, shortText, enablingKeyword);
    }
}
