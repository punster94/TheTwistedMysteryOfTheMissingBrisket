using UnityEngine;
using System.Collections;

public class Keyword
{
    static ArrayList keywords = new ArrayList();

    public string text;
    public int id;

    public Keyword(string t, int i)
    {
        text = t;
        id = i;
    }

    public static Keyword getKeyword(string t)
    {
        foreach(Keyword keyword in keywords)
        {
            if (keyword.text.Equals(t))
                return keyword;
        }

        Keyword newKeyword = new Keyword(t, keywords.Count);
        keywords.Add(newKeyword);

        return newKeyword;
    }
}
