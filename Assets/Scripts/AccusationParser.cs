using UnityEngine;
using System.Collections;
using System.Xml;

public static class AccusationParser
{
    public static ArrayList parse(string xmlText)
    {
        ArrayList accusations = new ArrayList();

        XmlDocument accusationFile = new XmlDocument();
        accusationFile.LoadXml(xmlText);

        foreach (XmlNode accusation in accusationFile.DocumentElement.ChildNodes)
        {
            ArrayList choices = new ArrayList();
            TimeState timeState = (TimeState)(System.Int32.Parse(accusation.Attributes["timeState"].InnerText));
            Statement transitionStatement = null;

            foreach (XmlNode choice in accusation.ChildNodes)
            {
                switch (choice.Name)
                {
                    case "Choice":
                        int personId = System.Int32.Parse(choice.Attributes["personId"].InnerText);
                        int defenderId = System.Int32.Parse(choice.Attributes["defenderId"].InnerText);

                        ArrayList enablingKeywords = new ArrayList();
                        ArrayList statementPairs = new ArrayList();
                        ArrayList intuitions = new ArrayList();
                        ArrayList accusationStatements = new ArrayList();
                        ArrayList resultStatements = new ArrayList();

                        foreach (XmlNode choiceComponent in choice.ChildNodes)
                        {
                            switch (choiceComponent.Name)
                            {
                                case "EnablingKeyword":
                                    enablingKeywords.Add(Keyword.getKeyword(choiceComponent.InnerText));
                                    break;
                                case "AccusationStatement":
                                    accusationStatements.Add(generateStatement(choiceComponent));
                                    break;
                                case "ResultStatement":
                                    resultStatements.Add(generateStatement(choiceComponent));
                                    break;
                                case "InvestigatorIntuition":
                                    intuitions.Add(generateStatement(choiceComponent));
                                    break;
                            }
                        }

                        for(int i = 0; i < accusationStatements.Count; i++)
                        {
                            ArrayList enablers = new ArrayList();
                            enablers.Add(Keyword.getKeyword(""));
                            statementPairs.Add(new Question(enablers, (Statement)accusationStatements[i], (Statement)resultStatements[i]));
                        }

                        choices.Add(new AccusationChoice(personId, defenderId, enablingKeywords, statementPairs, intuitions));
                        break;
                    case "TransitionStatement":
                        transitionStatement = generateStatement(choice);
                        break;
                }

            }

            accusations.Add(new Accusation(choices, timeState, transitionStatement));
        }

        return accusations;
    }

    public static Statement generateStatement(XmlNode node) {
        string ssText = "", shortText = "";
        Keyword enablingKeyword = null;
        ArrayList keywords = new ArrayList();

        foreach (XmlNode keyword in node.ChildNodes) {
            switch (keyword.Name) {
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
