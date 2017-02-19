using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class CanvasResizing : MonoBehaviour {
    public TextAsset xmlFile;
    public TextAsset tempXmlFile;
    string tmpFileAddress = "Assets/Resources/tmpDialog.xml";
    string originalFileAddress = "Assets/Resources/Dialog.xml";
    float screenX = 0.0f, screenY = 0.0f;
	// Use this for initialization
	void Start () {
        
        dialogues = DialogParser.parse(xmlFile.text);
    }

    // Update is called once per frame
    void Update() {
        if( screenX != Screen.width || screenY != Screen.height) {
            screenX = Screen.width;
            screenY = Screen.height;

            CalculateRect();
            
        }
    }

    private class GUIStatement {
        public Rect StatementGroupRect;
        public Rect StatementBGRect;

        public Rect StatementTitleRect;
        public string StatementTitle;

        public List<Rect> KeyWordsLabels;
        public List<Rect> KeyWordsInput;

        public List<string> keywords;

        public List<Rect> DeleteKeywordBtn;

        public Rect AddKeyWordBtn;

        public Rect TextLabelRect;
        public Rect TextInputRect;
        public string text;

        public Rect ShortTextLabelRect;
        public Rect ShortTextInputRect;
        public string shortText;

        public float height;
        public GUIStatement() {
            KeyWordsLabels = new List<Rect>();
            KeyWordsInput = new List<Rect>();
            keywords = new List<string>();

            DeleteKeywordBtn = new List<Rect>();

            StatementTitle = "Starting Statement";

        }
    }

    private class GUIQuestion {
        public Rect QuestionGroup;
        public Rect QuestionBGRect;
        public Rect DeleteQuestionBtn;
        public Rect AddEnablingKeywordBtn;

        public List<Rect> EnablingKeywordsLabelList;
        public List<Rect> EnablingKeywordsInputList;
        public List<Rect> RemoveEnablingKeyword;

        public List<Keyword> EnablingKeywords;

        public Rect titleRect;

        public GUIStatement question, answer;

        public GUIQuestion() {
            EnablingKeywords = new List<Keyword>();

            EnablingKeywordsLabelList = new List<Rect>();
            EnablingKeywordsInputList = new List<Rect>();
            RemoveEnablingKeyword = new List<Rect>();
        }
    }
    private class GUIDialog {
        public Rect DialogGroup;
        public Rect DialogBG;
        public int PersonID;
        public int TimeState;

        public Rect DeleteDialogBtn;

        public GUIStatement StartingStatement;

        public List<GUIQuestion> questions;

        public GUIDialog() {
            questions = new List<GUIQuestion>();

            StartingStatement = new GUIStatement();
        }
    }

    private List<GUIDialog> guiDialogs;

    
    public GUIStyle titleLabelStyle;
    public GUIStyle dialogLabelStyle;
    public GUIStyle labelStyle;

    ArrayList dialogues;

    float dialogHeight = 700;

    Rect TitleLabel;
    Rect AddDialogBtn;
    Rect SaveBtn;

    Rect DialogRect;
    Rect DialogLabel;
    Rect personIDLabel;
    Rect personIDInput;
    Rect timeStateLabel;
    Rect timeStateInput;

    Rect AddQuestionBtn;
    

    Rect ScrollViewRect;
    Vector2 scrollPosition = Vector2.zero;
    Rect ScrollViewArea;
    void CalculateRect() {
        TitleLabel = new Rect(0, 0, screenX, 30);
        AddDialogBtn = new Rect( screenX - 310, 0, 150, TitleLabel.height);
        SaveBtn = new Rect(AddDialogBtn.x + AddDialogBtn.width + 5, 0, 150, TitleLabel.height);

        guiDialogs = new List<GUIDialog>();

        ScrollViewRect = new Rect( 0, AddDialogBtn.height+5, screenX, screenY - AddDialogBtn.height - 5);

        DialogRect = new Rect(0, 0, screenX - 20, dialogHeight);
        DialogLabel = new Rect(10, 10, 200, 40);
        personIDLabel = new Rect(210, 10, 100, 20);
        personIDInput = new Rect(300, 10, 100, 20);

        timeStateLabel = new Rect(410, 10, 100, 20);
        timeStateInput = new Rect(timeStateLabel.x + timeStateLabel.width, 10, 100, 20);

        AddQuestionBtn = new Rect(DialogRect.width - 310, 10, 150, 30);

        Rect prevDialogRect = new Rect(0, 0, 0, 0);
        
        for ( int i = 0; i <dialogues.Count; i++) {
            Dialog dialog = ((Dialog)dialogues[i]);
            GUIDialog d = new GUIDialog();
            
            Rect tmp = new Rect(0, prevDialogRect.y + prevDialogRect.height, screenX - 10, dialogHeight);

            d.DeleteDialogBtn = new Rect( tmp.width - 165, 10, 150, 30 );

            d.PersonID = dialog.personId;
            d.TimeState = (int)dialog.timeState;

            d.StartingStatement = GenerateStatement(((Dialog)dialogues[i]).startingStatement, DialogLabel, tmp);
            dialogHeight = d.StartingStatement.StatementGroupRect.height + 5;

            Rect prevQuestionRect = d.StartingStatement.StatementGroupRect;
            
            for ( int j = 0; j < dialog.questions.Count; j++) {
                GUIQuestion question = new GUIQuestion();
                Rect questionGroup = new Rect(prevQuestionRect.x, prevQuestionRect.y + prevQuestionRect.height + 5, prevQuestionRect.width, 300);
                Rect questionBG = new Rect(0, 0, questionGroup.width, questionGroup.height);

                question.DeleteQuestionBtn = new Rect(questionGroup.width-155, 5, 150, 30);

                question.QuestionGroup = questionGroup;
                question.QuestionBGRect = questionBG;

                question.titleRect = new Rect(10, 0, 150, 40);
                question.AddEnablingKeywordBtn = new Rect(questionGroup.width - 310, 5, 150, 30);
                Rect offsetRect = question.titleRect;
                
                for (int k = 0; k < ((Question)dialog.questions[j]).enablingKeywords.Count; k++) {
                    question.EnablingKeywords.Add(((Keyword)((Question)dialog.questions[j]).enablingKeywords[k]));

                    Rect enablingKeywordLabel = new Rect(question.titleRect.x, question.titleRect.y + question.titleRect.height + k * (25), 200, 25);
                    Rect enablingKeywordInput = new Rect(enablingKeywordLabel.x + enablingKeywordLabel.width, enablingKeywordLabel.y, questionGroup.width - enablingKeywordLabel.x - enablingKeywordLabel.width - 30, 20);
                    Rect removeEnablingKeyword = new Rect(enablingKeywordInput.x + enablingKeywordInput.width + 5, enablingKeywordInput.y , 20, 20);

                    question.EnablingKeywordsLabelList.Add(enablingKeywordLabel);
                    question.EnablingKeywordsInputList.Add(enablingKeywordInput);
                    question.RemoveEnablingKeyword.Add(removeEnablingKeyword);

                    offsetRect = enablingKeywordLabel;
                }


                question.question = GenerateStatement(((Question)dialog.questions[j]).question, offsetRect, questionGroup, "Question");
                
                Rect questionTmpRect = new Rect(question.question.StatementGroupRect);
                questionTmpRect.height += 5;
                question.answer = GenerateStatement(((Question)dialog.questions[j]).answer, questionTmpRect, questionGroup, "Answer");

                questionGroup.height = question.titleRect.y + question.titleRect.height + question.question.StatementGroupRect.height + question.answer.StatementGroupRect.height + question.EnablingKeywords.Count * 25 + 10 + 20;
                question.QuestionGroup = questionGroup;
                questionBG.height = questionGroup.height;
                question.QuestionBGRect = questionBG;

                d.questions.Add(question);
                Rect tmpQuestion = new Rect(questionGroup);
                //tmpQuestion.height += 30 * (j + 1);
                prevQuestionRect = tmpQuestion;
                dialogHeight += prevQuestionRect.height + 5;
            }


            tmp.height = dialogHeight + 50;
            d.DialogGroup = tmp;
            DialogRect.height = d.DialogGroup.height;
            d.DialogBG = DialogRect;

            guiDialogs.Add(d);
            tmp.height += 5;
            prevDialogRect = tmp;
        }



        float dialogsHeight = 0.0f;
        foreach( GUIDialog guiDialog in guiDialogs) {
            dialogsHeight += guiDialog.DialogGroup.height;
        }

        dialogsHeight += guiDialogs.Count * 5;
        ScrollViewArea = new Rect(0, 0, screenX-20, dialogsHeight);
    }

    void OnGUI() {
        GUI.Label( TitleLabel, "Story Tool", titleLabelStyle);
        if( GUI.Button(AddDialogBtn, "Add Dialog")) {
            AddDialog();
        }

        if( GUI.Button(SaveBtn, "Save")) {
            SaveXMLFile();
        }

        scrollPosition = GUI.BeginScrollView(ScrollViewRect, scrollPosition, ScrollViewArea);
        for( int i= 0; i < guiDialogs.Count; i++) {
            GUI.BeginGroup(guiDialogs[i].DialogGroup);

            GUI.Box(guiDialogs[i].DialogBG, "");
            GUI.Label(DialogLabel, "Dialog #"+(i+1), dialogLabelStyle);

            if (GUI.Button(guiDialogs[i].DeleteDialogBtn, "Delete Dialog")) {
                DeleteDialog(i);
            }

            GUI.Label( personIDLabel, "PersonID:", labelStyle);
            guiDialogs[i].PersonID = System.Int32.Parse(GUI.TextField( personIDInput, guiDialogs[i].PersonID+""));

            GUI.Label(timeStateLabel, "Time State:", labelStyle);
            guiDialogs[i].TimeState = System.Int32.Parse(GUI.TextField(timeStateInput, guiDialogs[i].TimeState + ""));

            if( GUI.Button(AddQuestionBtn, "Add Question")) {
                AddQuestion(i);
            }

            // draw starting statement
            DrawStatement(guiDialogs[i].StartingStatement, i);

            // draw questions

            for ( int j = 0; j<guiDialogs[i].questions.Count; j++) {
                GUIQuestion question = guiDialogs[i].questions[j];

                GUI.BeginGroup(question.QuestionGroup);
                GUI.Box( question.QuestionBGRect, "" );

                if (GUI.Button(question.DeleteQuestionBtn, "Delete Question")) {
                    DeleteQuestion(i, j);
                }

                if (GUI.Button(question.AddEnablingKeywordBtn, "Add enabling keyword")) {
                    AddEnablingKeyword(i, j);
                }

                GUI.Label(question.titleRect, "Question #"+(j+1), labelStyle);

                for(int k = 0; k < question.EnablingKeywords.Count; k++) {
                    Keyword enablingWord = (Keyword)question.EnablingKeywords[k];

                    GUI.Label(question.EnablingKeywordsLabelList[k], "Enable Keyword #"+(k+1)+": ", labelStyle);
                    enablingWord.text = GUI.TextField(question.EnablingKeywordsInputList[k], enablingWord.text);

                    if (GUI.Button(question.RemoveEnablingKeyword[k], "X")) {
                        RemoveEnablingKeyword(i, j, k);
                    }

                    //question.EnablingKeywords[k] = enablingWord;
                }
                
                DrawStatement(question.question, i, j, true);
                DrawStatement(question.answer, i, j, false);

                GUI.EndGroup(); // end of question group
            }

            GUI.EndGroup(); // end of dialog
            
        }

        GUI.EndScrollView();
    }  
    
    void AddDialog() {
        SyncData();
        ArrayList keywords = new ArrayList();
        Statement startingStatement = new Statement(keywords, "", "");

        ArrayList questions = new ArrayList();
        Dialog dialog = new Dialog(startingStatement, questions, 1, TimeState.Time1);

        dialogues.Add(dialog);

        CalculateRect();
    }

    void DeleteDialog(int dialogIndex) {
        SyncData();
        dialogues.RemoveAt(dialogIndex);

        CalculateRect();
    }

    void AddQuestion(int dialogIndex) {
        SyncData();
        ArrayList keywordsQuestion = new ArrayList();
        Statement questionStatement = new Statement(keywordsQuestion, "Default question?", "Question");

        ArrayList keywordsAnswer = new ArrayList();
        Statement answer = new Statement(keywordsAnswer, "Default answer.", "Answer");

        ArrayList enablingKeywords = new ArrayList();
        Keyword enabler = new Keyword("", 1);
        enablingKeywords.Add(enabler);

        Question question = new Question(enablingKeywords, questionStatement, answer);

        ((Dialog)dialogues[dialogIndex]).questions.Add(question);
        CalculateRect();
    }

    void DeleteQuestion(int dialogIndex, int questionIndex) {
        SyncData();
        ((Dialog)dialogues[dialogIndex]).questions.RemoveAt(questionIndex);
        CalculateRect();
    }

    void AddEnablingKeyword(int dialogIndex, int questionIndex) {
        SyncData();
        Keyword enabler = new Keyword("", 1);
        ((Question)((Dialog)dialogues[dialogIndex]).questions[questionIndex]).enablingKeywords.Add(enabler);
        CalculateRect();
    }

    void RemoveEnablingKeyword(int dialogIndex, int questionIndex, int enablingKeywordIndex) {
        SyncData();
        ((Question)((Dialog)dialogues[dialogIndex]).questions[questionIndex]).enablingKeywords.RemoveAt(enablingKeywordIndex);
        CalculateRect();
    }

    void AddKeyword(int dialogIndex, int questionIndex, bool isQuestion) {
        SyncData();
        Keyword keyword = new Keyword("Default keyword", 1);
        if (questionIndex == -1) {
            // starting statement
            ((Dialog)dialogues[dialogIndex]).startingStatement.containedKeywords.Add(keyword);
        }
        else {
            if (isQuestion) {
                ((Question)((Dialog)dialogues[dialogIndex]).questions[questionIndex]).question.containedKeywords.Add(keyword);
            }
            else {
                ((Question)((Dialog)dialogues[dialogIndex]).questions[questionIndex]).answer.containedKeywords.Add(keyword);
            }
        }

        CalculateRect();
    }

    void DeleteKeyword(int dialogIndex, int questionIndex, int keywordIndex, bool isQuestion) {
        SyncData();
        Keyword keyword = new Keyword("Default keyword", 1);
        if (questionIndex == -1) {
            // starting statement
            ((Dialog)dialogues[dialogIndex]).startingStatement.containedKeywords.RemoveAt(keywordIndex);
        }
        else {
            if (isQuestion) {
                ((Question)((Dialog)dialogues[dialogIndex]).questions[questionIndex]).question.containedKeywords.RemoveAt(keywordIndex);
            }
            else {
                ((Question)((Dialog)dialogues[dialogIndex]).questions[questionIndex]).answer.containedKeywords.RemoveAt(keywordIndex);
            }
        }

        CalculateRect();
    }

   void DrawStatement(GUIStatement statement, int dialogIndex, int questionIndex = -1, bool isQuestion = false) {
        GUI.BeginGroup(statement.StatementGroupRect);
        GUI.Box(statement.StatementBGRect, "");
        GUI.Label(statement.StatementTitleRect, statement.StatementTitle, labelStyle);

        if (GUI.Button(statement.AddKeyWordBtn, "Add Keyword")) {
            AddKeyword(dialogIndex, questionIndex, isQuestion);
        }

        // draw keywords
        for (int j = 0; j < statement.keywords.Count; j++) {
            GUI.Label(statement.KeyWordsLabels[j], "Keyword: ", labelStyle);
            statement.keywords[j] = GUI.TextField(statement.KeyWordsInput[j], statement.keywords[j]);

            if( GUI.Button(statement.DeleteKeywordBtn[j], "X" )) {
                DeleteKeyword(dialogIndex, questionIndex, j, isQuestion);
            }
        }

        GUI.Label(statement.TextLabelRect, "Text: ", labelStyle);
        statement.text = GUI.TextField(statement.TextInputRect, statement.text);

        GUI.Label(statement.ShortTextLabelRect, "Short Text: ", labelStyle);
        statement.shortText = GUI.TextField(statement.ShortTextInputRect, statement.shortText);

        GUI.EndGroup(); // end of starting statement
    }

    GUIStatement GenerateStatement( Statement pStatement, Rect offsetRect, Rect ParentGroup, string title = "Starting Statement") {
        GUIStatement statement = new GUIStatement();

        statement.StatementGroupRect = new Rect(20, offsetRect.y + offsetRect.height, ParentGroup.width - 40, 100);

        statement.StatementTitleRect = new Rect(10, 0, statement.StatementBGRect.width, 40);
        statement.StatementTitle = title;

        statement.AddKeyWordBtn = new Rect(statement.StatementGroupRect.width - 155, 10, 150, 30);

        statement.height = statement.StatementTitleRect.y + statement.StatementTitleRect.height;

        Rect prevRect = new Rect(0, 0, 0, statement.StatementTitleRect.y + statement.StatementTitleRect.height);
        foreach (Keyword keyword in pStatement.containedKeywords) {
            statement.keywords.Add(keyword.text);
            Rect statementRectLabel = new Rect(10, prevRect.y + prevRect.height + 5, 100, 20);
            Rect statementRectInput = new Rect(statementRectLabel.x + statementRectLabel.width, prevRect.y + prevRect.height + 5, statement.StatementGroupRect.width - (statementRectLabel.x + statementRectLabel.width) - 40, 20);
            Rect deleteBtn = new Rect(statementRectInput.x + statementRectInput.width + 10, statementRectInput.y , 20, 20);

            statement.KeyWordsLabels.Add(statementRectLabel);
            statement.KeyWordsInput.Add(statementRectInput);
            statement.DeleteKeywordBtn.Add(deleteBtn);

            prevRect = statementRectLabel;

            statement.height += statementRectLabel.height + 5;
        }

        statement.TextLabelRect = new Rect(10, prevRect.y + prevRect.height + 5, 100, 20);
        statement.TextInputRect = new Rect(statement.TextLabelRect.x + statement.TextLabelRect.width, statement.TextLabelRect.y, statement.StatementGroupRect.width - (statement.TextLabelRect.x + statement.TextLabelRect.width) - 10, 20);
        statement.text = pStatement.displayText;

        statement.ShortTextLabelRect = new Rect(10, statement.TextLabelRect.y + statement.TextLabelRect.height + 5, 100, 20);
        statement.ShortTextInputRect = new Rect(statement.ShortTextLabelRect.x + statement.ShortTextLabelRect.width, statement.ShortTextLabelRect.y, statement.TextInputRect.width, 20);
        statement.shortText = pStatement.shortenedText;

        statement.height += (statement.TextLabelRect.height + 5);
        statement.height += (statement.ShortTextLabelRect.height + 5);



        statement.StatementGroupRect.height = statement.height + 5;

        statement.StatementBGRect = new Rect(0, 0, statement.StatementGroupRect.width, statement.StatementGroupRect.height);

        return statement;
    }

    void SyncData() {
        SaveTmpXMLFile();

        dialogues = DialogParser.parse(tempXmlFile.text);
    }

    void SaveTmpXMLFile() {
        //SyncData();
        List<string> fileData = new List<string>();

        fileData.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        fileData.Add("<Dialogues>");

        foreach (GUIDialog dialog in guiDialogs) {
            fileData.Add("\t<Dialog personId = \"" + dialog.PersonID + "\" timeState = \"" + ((int)dialog.TimeState) + "\">");

            fileData.Add("\t\t<Statement>");
            foreach (string keyword in dialog.StartingStatement.keywords) {
                fileData.Add("\t\t\t<Keyword>" + keyword + "</Keyword>");
            }
            fileData.Add("\t\t\t<Text>" + dialog.StartingStatement.text + "</Text>");
            fileData.Add("\t\t\t<ShortenedText>" + dialog.StartingStatement.shortText + "</ShortenedText>");

            fileData.Add("\t\t</Statement>\n");

            // questions
            foreach (GUIQuestion question in dialog.questions) {
                fileData.Add("\t\t<Question>");

                foreach(Keyword keyword in question.EnablingKeywords) {
                    fileData.Add("\t\t\t<EnablingKeyword>" + keyword.text + "</EnablingKeyword>");
                }
                

                fileData.Add("\t\t\t<QuestionStatement>");
                foreach (string keyword in question.question.keywords) {
                    fileData.Add("\t\t\t\t<Keyword>" + keyword + "</Keyword>");
                }
                fileData.Add("\t\t\t\t<Text>" + question.question.text + "</Text>");
                fileData.Add("\t\t\t\t<ShortenedText>" + question.question.shortText + "</ShortenedText>");
                fileData.Add("\t\t\t</QuestionStatement>");

                fileData.Add("\t\t\t<AnswerStatement>");
                foreach (string keyword in question.answer.keywords) {
                    fileData.Add("\t\t\t\t<Keyword>" + keyword+ "</Keyword>");
                }
                fileData.Add("\t\t\t\t<Text>" + question.answer.text + "</Text>");
                fileData.Add("\t\t\t\t<ShortenedText>" + question.answer.shortText + "</ShortenedText>");

                fileData.Add("\t\t\t</AnswerStatement>");


                fileData.Add("\t\t</Question>\n");
            }

            fileData.Add("\t</Dialog>\n");
        }

        fileData.Add("</Dialogues>");

        Debug.Log("Writing into temp file.");

        System.IO.File.WriteAllLines(tmpFileAddress, fileData.ToArray());

    }

    void SaveXMLFile() {
        SyncData();
        List<string> fileData = new List<string>();

        fileData.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        fileData.Add("<Dialogues>");

        foreach (Dialog dialog in dialogues) {
            fileData.Add("\t<Dialog personId = \""+dialog.personId+"\" timeState = \""+ ((int)dialog.timeState) + "\">");

            fileData.Add("\t\t<Statement>");
            foreach(Keyword keyword in dialog.startingStatement.containedKeywords) {
                fileData.Add("\t\t\t<Keyword>"+keyword.text+"</Keyword>");
            }
            fileData.Add("\t\t\t<Text>"+dialog.startingStatement.displayText+"</Text>");
            fileData.Add("\t\t\t<ShortenedText>" + dialog.startingStatement.shortenedText + "</ShortenedText>");

            fileData.Add("\t\t</Statement>\n");

            // questions
            foreach(Question question in dialog.questions) {
                fileData.Add("\t\t<Question>");

                foreach (Keyword keyword in question.enablingKeywords) {
                    fileData.Add("\t\t\t<EnablingKeyword>" + keyword.text + "</EnablingKeyword>");
                }

                fileData.Add("\t\t\t<QuestionStatement>");
                foreach (Keyword keyword in question.question.containedKeywords) {
                    fileData.Add("\t\t\t\t<Keyword>" + keyword.text + "</Keyword>");
                }
                fileData.Add("\t\t\t\t<Text>" + question.question.displayText + "</Text>");
                fileData.Add("\t\t\t\t<ShortenedText>" + question.question.shortenedText + "</ShortenedText>");
                fileData.Add("\t\t\t</QuestionStatement>");

                fileData.Add("\t\t\t<AnswerStatement>");
                foreach (Keyword keyword in question.answer.containedKeywords) {
                    fileData.Add("\t\t\t\t<Keyword>" + keyword.text + "</Keyword>");
                }
                fileData.Add("\t\t\t\t<Text>" + question.answer.displayText + "</Text>");
                fileData.Add("\t\t\t\t<ShortenedText>" + question.answer.shortenedText + "</ShortenedText>");

                fileData.Add("\t\t\t</AnswerStatement>");


                fileData.Add("\t\t</Question>\n");
            }

            fileData.Add("\t</Dialog>\n");
        }

        fileData.Add("</Dialogues>");

        Debug.Log("Saving into original TextDialog.XML");

        System.IO.File.WriteAllLines(originalFileAddress, fileData.ToArray());
        
    }
}
