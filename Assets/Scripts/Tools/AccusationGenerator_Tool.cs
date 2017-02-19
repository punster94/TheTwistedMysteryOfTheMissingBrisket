using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AccusationGenerator_Tool : MonoBehaviour {
    public GUIStyle titleLabelStyle;
    public GUIStyle dialogLabelStyle;
    public GUIStyle labelStyle;

    public TextAsset xmlFile;
    public TextAsset tempXmlFile;

    string tmpFileAddress = "Assets/Resources/tmpAccusations.xml";
    string originalFileAddress = "Assets/Resources/Accusations.xml";
    float screenX = 0.0f, screenY = 0.0f;

    ArrayList xmlAccusations;

    // Use this for initialization
    void Start () {
        xmlAccusations = AccusationParser.parse(xmlFile.text);
        guiAccusationList = new List<GUIAccusation>();
    }
	
	// Update is called once per frame
	void Update () {
        if (screenX != Screen.width || screenY != Screen.height) {
            screenX = Screen.width;
            screenY = Screen.height;

            CalculateRect();

        }
    }

    private class GUIStatement {
        // data
        public string StatementTitle;
        public List<string> enablingKeywords;
        public List<string> keywords;
        public string text;
        public string shortText;

        // rect
        public Rect StatementGroupRect;
        public Rect StatementBGRect;

        public Rect StatementTitleRect;
        
        public List<Rect> KeyWordsLabels;
        public List<Rect> KeyWordsInput;

        public List<Rect> DeleteKeywordBtn;

        public List<Rect> EnablingKeywordLabel;
        public List<Rect> EnablingKeywordInput;

        public Rect AddKeyWordBtn;
        public Rect RemoveStatement;

        public Rect TextLabelRect;
        public Rect TextInputRect;
        
        public Rect ShortTextLabelRect;
        public Rect ShortTextInputRect;
        

        public float height;
        public GUIStatement() {
            KeyWordsLabels = new List<Rect>();
            KeyWordsInput = new List<Rect>();
            enablingKeywords = new List<string>();
            EnablingKeywordLabel = new List<Rect>();
            EnablingKeywordInput = new List<Rect>();
            keywords = new List<string>();

            DeleteKeywordBtn = new List<Rect>();

            StatementTitle = "Starting Statement";

        }
    }

    private class GUIAccusationChoice {
        // data
        public int personID;
        public int defenderID;
        public List<string> enablingKeywords;
        public List<GUIStatement> investigatorIntution;
        public List<GUIAccusationStatementPair> accusationStatement;
        
        // rect
        public Rect accusationChoiceGroup;
        public Rect accusationChoiceBG;

        public Rect choiceTitle;
        public Rect AddEnablingKeyword;

        public Rect PersonIDLabel;
        public Rect PersonIDInput;

        public Rect DefenderIDLabel;
        public Rect DefenderIDInput;

        public Rect InvestigatorIntutionTitle;
        public Rect AddInvstigatorIntution;

        public List<Rect> EnablingKeywordsLabel;
        public List<Rect> EnablingKeywordsInput;
        public List<Rect> DeleteEnablingKeyword;

        public Rect AccusationStatementTitle;
        public Rect AddAccusationStatement;
        public GUIAccusationChoice() {
            enablingKeywords = new List<string>();
            investigatorIntution = new List<GUIStatement>();
            accusationStatement = new List<GUIAccusationStatementPair>();

            EnablingKeywordsLabel = new List<Rect>();
            EnablingKeywordsInput = new List<Rect>();
            DeleteEnablingKeyword = new List<Rect>();
        }
    }

    private class GUIAccusationStatementPair {
        public Rect AccusationStatementGroup;
        public Rect AccusationStatementBGRect;
        public Rect DeleteQuestionBtn;
        public Rect AddEnablingKeywordBtn;

        public List<Rect> EnablingKeywordsLabelList;
        public List<Rect> EnablingKeywordsInputList;
        public List<Rect> RemoveEnablingKeyword;

        public List<Keyword> EnablingKeywords;

        public Rect titleRect;
        public Rect DeleteStatementPair;

        public GUIStatement question, answer;

        public GUIAccusationStatementPair() {
            EnablingKeywords = new List<Keyword>();

            EnablingKeywordsLabelList = new List<Rect>();
            EnablingKeywordsInputList = new List<Rect>();
            RemoveEnablingKeyword = new List<Rect>();

            question = new GUIStatement();
            answer = new GUIStatement();
        }
    }

    private class GUIAccusation {
        // data
        public int timeState;

        // rect
        public Rect AccusationGroup;
        public Rect AccusationBG;

        public Rect AccusationTitle;

        public Rect TimeStampLabel;
        public Rect TimeStampInput;

        public GUIStatement TransitionStatement;
        public List<GUIAccusationChoice> AccusationChoice;

        public GUIAccusation() {
            AccusationChoice = new List<GUIAccusationChoice>();
        }
    }

    List<GUIAccusation> guiAccusationList;
    Rect TitleLabel;
    Rect SaveBtn;

    Rect ScrollViewRect;
    Vector2 scrollPosition = Vector2.zero;
    Rect ScrollViewArea;

    void CalculateRect() {
        guiAccusationList.Clear();

        TitleLabel = new Rect(0, 0, screenX, 30);
        SaveBtn = new Rect(screenX - 155, 5, 150, 30);
        ScrollViewRect = new Rect(0, TitleLabel.height + 5, screenX, screenY - TitleLabel.height - 5);
        ScrollViewArea = new Rect(0, 0, screenX - 20, 0);

        Rect prevAccusationGroup = new Rect(5, 0, screenX - 25, 0);

        for (int i = 0; i < xmlAccusations.Count; i++) {
            Accusation accusation = (Accusation)xmlAccusations[i];
            GUIAccusation guiAccusation = new GUIAccusation();

            guiAccusation.timeState = (int)accusation.timeState;
            Rect accusationGroupRect = new Rect(prevAccusationGroup.x, prevAccusationGroup.y  + prevAccusationGroup.height, prevAccusationGroup.width, 0);
            
            guiAccusation.AccusationGroup = accusationGroupRect;
            guiAccusation.AccusationBG = new Rect(0, 0, guiAccusation.AccusationGroup.width, guiAccusation.AccusationGroup.height);

            guiAccusation.AccusationTitle = new Rect(5, 5, 300, 30);
            guiAccusation.TimeStampLabel = new Rect(guiAccusation.AccusationTitle.x + guiAccusation.AccusationTitle.width, guiAccusation.AccusationTitle.y, 150, 20 );
            guiAccusation.TimeStampInput = new Rect(guiAccusation.TimeStampLabel.x + guiAccusation.TimeStampLabel.width, guiAccusation.AccusationTitle.y, 150, 20);

            guiAccusation.TransitionStatement = GenerateStatement(accusation.transitionStatement, guiAccusation.AccusationTitle, guiAccusation.AccusationGroup, "Transition Statement");

            Rect prevRect = guiAccusation.TransitionStatement.StatementGroupRect;

            ScrollViewArea.height += guiAccusation.AccusationTitle.height;
            ScrollViewArea.height += guiAccusation.TransitionStatement.StatementGroupRect.height;

            accusationGroupRect.height += guiAccusation.AccusationTitle.height;
            accusationGroupRect.height += guiAccusation.TransitionStatement.StatementGroupRect.height;

            for ( int j = 0; j < accusation.choices.Count; j++) {
                AccusationChoice accusationChoice = (AccusationChoice)accusation.choices[j];
                GUIAccusationChoice guiAccusationChoice = new GUIAccusationChoice();
                
                guiAccusationChoice.personID = accusationChoice.personId;
                guiAccusationChoice.defenderID = accusationChoice.defenderId;

                Rect accusationChoiceGroup = new Rect(20, prevRect.y + prevRect.height + 10, guiAccusation.AccusationGroup.width - 40, 0);
                guiAccusationChoice.accusationChoiceGroup = accusationChoiceGroup;
                
                guiAccusationChoice.choiceTitle = new Rect(5, 5, 150, 30);
                guiAccusationChoice.AddEnablingKeyword = new Rect(accusationChoiceGroup.width - 155, guiAccusationChoice.choiceTitle.y/2.0f, 150, 30);

                guiAccusationChoice.PersonIDLabel = new Rect(guiAccusationChoice.choiceTitle.x + guiAccusationChoice.choiceTitle.width, guiAccusationChoice.choiceTitle.y, 120, 20);
                guiAccusationChoice.PersonIDInput = new Rect(guiAccusationChoice.PersonIDLabel.x + guiAccusationChoice.PersonIDLabel.width, guiAccusationChoice.PersonIDLabel.y, 150, 20);

                //ScrollViewArea.height += 10;
                accusationGroupRect.height += guiAccusationChoice.choiceTitle.height;

                guiAccusationChoice.DefenderIDLabel = new Rect(guiAccusationChoice.PersonIDInput.x + guiAccusationChoice.PersonIDInput.width + 40, guiAccusationChoice.PersonIDInput.y, 120, 20);
                guiAccusationChoice.DefenderIDInput = new Rect(guiAccusationChoice.DefenderIDLabel.x + guiAccusationChoice.DefenderIDLabel.width, guiAccusationChoice.DefenderIDLabel.y, 150, 20);

                
                Rect tmpPrevRect = guiAccusationChoice.DefenderIDInput;
                for (int k=0; k < accusationChoice.enablingKeywords.Count; k++) {
                    guiAccusationChoice.enablingKeywords.Add(((Keyword)accusationChoice.enablingKeywords[k]).text);

                    Rect enablingKeywordLabelRect = new Rect(10, guiAccusationChoice.DefenderIDLabel.y + guiAccusationChoice.DefenderIDLabel.height + k * 25 + 10, 180, 20);
                    Rect enablingKeywordInputRect = new Rect(enablingKeywordLabelRect.x + enablingKeywordLabelRect.width, enablingKeywordLabelRect.y, guiAccusationChoice.accusationChoiceGroup.width - enablingKeywordLabelRect.x - enablingKeywordLabelRect.width - 5 - 30, enablingKeywordLabelRect.height);
                    Rect deleteEnablingKeyword = new Rect(enablingKeywordInputRect.x + enablingKeywordInputRect.width + 5, enablingKeywordInputRect.y, 20, 20);
                    //ScrollViewArea.height += enablingKeywordLabelRect.height + k * 25 + 10;
                    accusationChoiceGroup.height += enablingKeywordLabelRect.height + k * 25 + 10;

                    guiAccusationChoice.EnablingKeywordsLabel.Add(enablingKeywordLabelRect);
                    guiAccusationChoice.EnablingKeywordsInput.Add(enablingKeywordInputRect);
                    guiAccusationChoice.DeleteEnablingKeyword.Add(deleteEnablingKeyword);

                    tmpPrevRect = enablingKeywordLabelRect;
                }

                guiAccusationChoice.InvestigatorIntutionTitle = new Rect(10, tmpPrevRect.y + tmpPrevRect.height + 10, 200, 20);
                tmpPrevRect = guiAccusationChoice.InvestigatorIntutionTitle;

                guiAccusationChoice.AddInvstigatorIntution = new Rect(accusationChoiceGroup.width - 155, guiAccusationChoice.InvestigatorIntutionTitle.y - 5 , 150, 30);

                //ScrollViewArea.height += guiAccusationChoice.InvestigatorIntutionTitle.height + 10;
                accusationChoiceGroup.height += guiAccusationChoice.InvestigatorIntutionTitle.height + 10;

                for (int k = 0; k < accusationChoice.investigatorIntuitions.Count; k++) {
                    Statement investigatorIntution = (Statement)accusationChoice.investigatorIntuitions[k];
                    tmpPrevRect.height += 10;
                    GUIStatement guiInvestigatorIntution = GenerateStatement(investigatorIntution, tmpPrevRect, guiAccusationChoice.accusationChoiceGroup, "Investigator Intuitions");
                    tmpPrevRect = guiInvestigatorIntution.StatementGroupRect;

                    //ScrollViewArea.height += guiInvestigatorIntution.StatementGroupRect.height;
                    accusationChoiceGroup.height += guiInvestigatorIntution.StatementGroupRect.height;

                    guiAccusationChoice.investigatorIntution.Add(guiInvestigatorIntution);
                }

                accusationChoiceGroup.height += 5;

                guiAccusationChoice.AccusationStatementTitle = new Rect(10, tmpPrevRect.y + tmpPrevRect.height, 200, 30);
                tmpPrevRect = guiAccusationChoice.AccusationStatementTitle;

                guiAccusationChoice.AddAccusationStatement = new Rect(accusationChoiceGroup.width - 155, guiAccusationChoice.AccusationStatementTitle.y + 3, 150, 30);
                //ScrollViewArea.height += guiAccusationChoice.AccusationStatementTitle.height;
                accusationChoiceGroup.height += guiAccusationChoice.AccusationStatementTitle.height;

                for (int k = 0; k < accusationChoice.statementPairs.Count; k++) {
                    Question question = (Question)accusationChoice.statementPairs[k];
                    GUIAccusationStatementPair accusationStatementPair = new GUIAccusationStatementPair();

                    Rect accusationStatementGroup = new Rect(tmpPrevRect.x, tmpPrevRect.y + tmpPrevRect.height + 5, guiAccusationChoice.accusationChoiceGroup.width - tmpPrevRect.x - tmpPrevRect.x, 0);
                    
                    accusationStatementPair.titleRect = new Rect(10, 0, 150, 40);
                    accusationStatementGroup.height += accusationStatementPair.titleRect.x + accusationStatementPair.titleRect.height;

                    accusationStatementPair.DeleteStatementPair = new Rect(accusationStatementGroup.width - 155, 5, 150, 30);

                    Rect offsetRect = accusationStatementPair.titleRect;

                    accusationStatementPair.question = GenerateStatement(question.question, offsetRect, accusationStatementGroup, "AccusationStatement");
                    accusationStatementGroup.height += accusationStatementPair.question.StatementGroupRect.height;

                    //ScrollViewArea.height += accusationStatementPair.question.StatementGroupRect.height;

                    Rect questionTmpRect = new Rect(accusationStatementPair.question.StatementGroupRect);
                    questionTmpRect.height += 5;

                    accusationStatementPair.answer = GenerateStatement(question.answer, questionTmpRect, accusationStatementGroup, "ResultStatement");
                    accusationStatementGroup.height += accusationStatementPair.answer.StatementGroupRect.height;

                    //ScrollViewArea.height += accusationStatementPair.answer.StatementGroupRect.height;

                    guiAccusationChoice.accusationStatement.Add(accusationStatementPair);

                    Rect accusationStatementBG = new Rect(0, 0, accusationStatementGroup.width, accusationStatementGroup.height);

                    accusationStatementPair.AccusationStatementGroup = accusationStatementGroup;
                    accusationStatementPair.AccusationStatementBGRect = accusationStatementBG;

                    tmpPrevRect = accusationStatementGroup;
                    //ScrollViewArea.height += accusationStatementPair.titleRect.height + 5;
                    accusationChoiceGroup.height += accusationStatementPair.AccusationStatementGroup.height;
                }
                accusationChoiceGroup.height += 35;

                guiAccusationChoice.accusationChoiceGroup = accusationChoiceGroup;
                guiAccusationChoice.accusationChoiceBG = new Rect(0, 0, guiAccusationChoice.accusationChoiceGroup.width, guiAccusationChoice.accusationChoiceGroup.height);
                accusationGroupRect.height += guiAccusationChoice.accusationChoiceGroup.height;
                //accusationGroupRect.y = i * accusationGroupRect.height;

                

                prevRect = guiAccusationChoice.accusationChoiceGroup;

                guiAccusation.AccusationChoice.Add(guiAccusationChoice);
            }
            accusationGroupRect.height -= 200;
            guiAccusation.AccusationGroup = accusationGroupRect;
            guiAccusation.AccusationBG = new Rect(0, 0, guiAccusation.AccusationGroup.width, guiAccusation.AccusationGroup.height);
            
            ScrollViewArea.height += accusationGroupRect.height - 150;


            prevAccusationGroup.height += guiAccusation.AccusationGroup.height + 10;

            guiAccusationList.Add(guiAccusation);
            
        }

        ScrollViewArea.height += 20;


    }

    void OnGUI() {
        GUI.Label(TitleLabel, "Accusation Tool", titleLabelStyle);
        if(GUI.Button(SaveBtn, "Save")) {
            SyncData();
            SaveXMLFile(originalFileAddress);

            CalculateRect();
        }

        for(int i = 0; i < guiAccusationList.Count; i++) {
            GUIAccusation guiAccusation = guiAccusationList[i];
            scrollPosition = GUI.BeginScrollView(ScrollViewRect, scrollPosition, ScrollViewArea);

            GUI.BeginGroup(guiAccusation.AccusationGroup);

            GUI.Box(guiAccusation.AccusationBG, "");
            GUI.Label(guiAccusation.AccusationTitle, "Accusation", dialogLabelStyle);
            GUI.Label(guiAccusation.TimeStampLabel, "Time Stamp: ", labelStyle);
            guiAccusation.timeState = System.Int32.Parse(GUI.TextField(guiAccusation.TimeStampInput, guiAccusation.timeState+""));

            DrawStatement(guiAccusation.TransitionStatement, i);

            // drawing choices
            for(int choiceIndex = 0; choiceIndex < guiAccusation.AccusationChoice.Count; choiceIndex++) {
                GUIAccusationChoice guiAccusationChoice = guiAccusation.AccusationChoice[choiceIndex];
                GUI.BeginGroup(guiAccusationChoice.accusationChoiceGroup);

                GUI.Box(guiAccusationChoice.accusationChoiceBG, "");
                GUI.Label(guiAccusationChoice.choiceTitle, "Choice", dialogLabelStyle);

                if(GUI.Button(guiAccusationChoice.AddEnablingKeyword, "Add enabling keyword")) {
                    AddKeyword(i, choiceIndex);
                }

                GUI.Label(guiAccusationChoice.PersonIDLabel, "Person ID: ", labelStyle);
                guiAccusationChoice.personID = System.Int32.Parse( GUI.TextField(guiAccusationChoice.PersonIDInput, guiAccusationChoice.personID+"") );

                GUI.Label(guiAccusationChoice.DefenderIDLabel, "Defender ID: ", labelStyle);
                guiAccusationChoice.defenderID = System.Int32.Parse(GUI.TextField(guiAccusationChoice.DefenderIDInput, guiAccusationChoice.defenderID+"") );
                
                for (int j = 0; j < guiAccusationChoice.enablingKeywords.Count; j++) {
                    GUI.Label(guiAccusationChoice.EnablingKeywordsLabel[j], "Enabling keyword: ", labelStyle);
                    guiAccusationChoice.enablingKeywords[j] = GUI.TextField(guiAccusationChoice.EnablingKeywordsInput[j], guiAccusationChoice.enablingKeywords[j]);
                    if( GUI.Button(guiAccusationChoice.DeleteEnablingKeyword[j], "X")) {
                        DeleteKeyword(j, i, choiceIndex);
                    }
                }

                GUI.Label(guiAccusationChoice.InvestigatorIntutionTitle, "Investigator intuition statements:", dialogLabelStyle);
                if (GUI.Button(guiAccusationChoice.AddInvstigatorIntution, "Add intuition")) {
                    AddIntution(i, choiceIndex);
                }
                for ( int j = 0; j < guiAccusationChoice.investigatorIntution.Count; j++ ) {
                    GUIStatement guiStatement = guiAccusationChoice.investigatorIntution[j];
                    DrawStatement(guiStatement, i, choiceIndex, j, -1, false, true, "Remove intuition");
                }

                GUI.Label(guiAccusationChoice.AccusationStatementTitle, "Accusation dialogs:", dialogLabelStyle);
                if (GUI.Button(guiAccusationChoice.AddAccusationStatement, "Add accusation dialog")) {
                    AddAccusationDialog(i, choiceIndex);
                }

                for(int j= 0; j < guiAccusationChoice.accusationStatement.Count; j++) {
                    GUIAccusationStatementPair guiAccusationStatementPair = guiAccusationChoice.accusationStatement[j];
                    GUI.BeginGroup(guiAccusationStatementPair.AccusationStatementGroup);

                    GUI.Box(guiAccusationStatementPair.AccusationStatementBGRect, "");
                    GUI.Label(guiAccusationStatementPair.titleRect, "Accusation dialog", labelStyle);

                    if(GUI.Button(guiAccusationStatementPair.DeleteStatementPair, "Remove dialog")) {
                        DeleteAccusationDialog(i, choiceIndex, j);
                    }

                    DrawStatement(guiAccusationStatementPair.question, i, choiceIndex, -1, j, true);
                    DrawStatement(guiAccusationStatementPair.answer, i, choiceIndex, -1, j, false);

                    GUI.EndGroup(); // end of accusation statement group
                }

                GUI.EndGroup(); // end of accusation choice group
            }

            GUI.EndGroup(); // end of accusation group

            GUI.EndScrollView();
        }
    }
    
    void AddIntution(int pTimeState, int pChoiceIndex) {
        GUIStatement intutionStatement = new GUIStatement();
        intutionStatement.enablingKeywords.Add("");
        guiAccusationList[pTimeState].AccusationChoice[pChoiceIndex].investigatorIntution.Add(intutionStatement);

        SyncData();
        CalculateRect();
    }

    void AddKeyword(int accusationIndex, int choiceIndex, int investigatorIntutionIndex = -1, int accusationDialogIndex = -1, bool isAccusationStatement = false) {

        if (choiceIndex == -1) {
            // transition statement
            guiAccusationList[accusationIndex].TransitionStatement.keywords.Add("");
            //((Dialog)dialogues[accusationIndex]).startingStatement.containedKeywords.Add(keyword);
        }
        else {
            // choice
            if (investigatorIntutionIndex == -1 && accusationDialogIndex == -1) {
                guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].enablingKeywords.Add("");
            }
            else {
                // investigator dialog or accusation dialog
                if (investigatorIntutionIndex != -1) {
                    guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].investigatorIntution[investigatorIntutionIndex].keywords.Add("");
                }
                else {
                    // accusation 
                   if (isAccusationStatement) {
                        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].accusationStatement[accusationDialogIndex].question.keywords.Add("");
                    }
                    else {
                        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].accusationStatement[accusationDialogIndex].answer.keywords.Add("");
                    }
                }
            }
        }

        
        SyncData();
        CalculateRect();
    }

    void DeleteKeyword(int keywordIndex, int accusationIndex, int choiceIndex, int investigatorIntutionIndex = -1, int accusationDialogIndex = -1, bool isAccusationStatement = false) {

        if (choiceIndex == -1) {
            // transition statement
            guiAccusationList[accusationIndex].TransitionStatement.keywords.RemoveAt(keywordIndex);
            //((Dialog)dialogues[accusationIndex]).startingStatement.containedKeywords.Add(keyword);
        }
        else {
            // choice
            if (investigatorIntutionIndex == -1 && accusationDialogIndex == -1) {
                guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].enablingKeywords.RemoveAt(keywordIndex);
            }
            else {
                // investigator dialog or accusation dialog
                if (investigatorIntutionIndex != -1) {
                    guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].investigatorIntution[investigatorIntutionIndex].keywords.RemoveAt(keywordIndex);
                }
                else {
                    // accusation 
                    if (isAccusationStatement) {
                        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].accusationStatement[accusationDialogIndex].question.keywords.RemoveAt(keywordIndex);
                    }
                    else {
                        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].accusationStatement[accusationDialogIndex].answer.keywords.RemoveAt(keywordIndex);
                    }
                }
            }
        }
        SyncData();
        CalculateRect();
    }
    
    void DeleteStatement(int accusationIndex, int choiceIndex, int investigatorIntutionIndex) {
        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].investigatorIntution.RemoveAt(investigatorIntutionIndex);

        SyncData();
        CalculateRect();
    }

    void AddAccusationDialog(int accusationIndex, int choiceIndex) {
        GUIAccusationStatementPair dialog = new GUIAccusationStatementPair();
        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].accusationStatement.Add(dialog);

        SyncData();
        CalculateRect();
    }

    void DeleteAccusationDialog(int accusationIndex, int choiceIndex, int accusationDialogIndex) {
        guiAccusationList[accusationIndex].AccusationChoice[choiceIndex].accusationStatement.RemoveAt(accusationDialogIndex);
        
        SyncData();
        CalculateRect();
    }

    void DrawStatement(GUIStatement guiStatement, int accusationIndex, int choiceIndex = -1, int investigatorIntutionIndex = -1, int accusationDialogIndex = -1, bool isAccusationStatement = false, bool isRemoveable = false, string buttonStr = "Remove") {
        GUI.BeginGroup(guiStatement.StatementGroupRect);
        GUI.Box(guiStatement.StatementBGRect, "");
        GUI.Label(guiStatement.StatementTitleRect, guiStatement.StatementTitle, labelStyle);

        if (GUI.Button(guiStatement.AddKeyWordBtn, "Add Keyword")) {
            AddKeyword(accusationIndex, choiceIndex, investigatorIntutionIndex, accusationDialogIndex, isAccusationStatement);
        }

        if (isRemoveable) {
            if( GUI.Button(guiStatement.RemoveStatement, buttonStr)) {
                DeleteStatement(accusationIndex, choiceIndex, investigatorIntutionIndex);
            }
        }

        // draw enabling keywords
        for (int j = 0; j < guiStatement.enablingKeywords.Count; j++) {
            GUI.Label(guiStatement.EnablingKeywordLabel[j], "Enabling Keyword: ", labelStyle);
            guiStatement.enablingKeywords[j] = GUI.TextField(guiStatement.EnablingKeywordInput[j], guiStatement.enablingKeywords[j]);
        }

        // draw keywords
        for (int j = 0; j < guiStatement.keywords.Count; j++) {
            GUI.Label(guiStatement.KeyWordsLabels[j], "Keyword: ", labelStyle);
            guiStatement.keywords[j] = GUI.TextField(guiStatement.KeyWordsInput[j], guiStatement.keywords[j]);

            if (GUI.Button(guiStatement.DeleteKeywordBtn[j], "X")) {
                DeleteKeyword(j, accusationIndex, choiceIndex, investigatorIntutionIndex, accusationDialogIndex, isAccusationStatement);
            }
        }

        GUI.Label(guiStatement.TextLabelRect, "Text: ", labelStyle);
        guiStatement.text = GUI.TextField(guiStatement.TextInputRect, guiStatement.text);

        GUI.Label(guiStatement.ShortTextLabelRect, "Short Text: ", labelStyle);
        guiStatement.shortText = GUI.TextField(guiStatement.ShortTextInputRect, guiStatement.shortText);

        GUI.EndGroup(); // end of starting statement
    }

    GUIStatement GenerateStatement(Statement pStatement, Rect offsetRect, Rect ParentGroup, string title = "Starting Statement") {
        GUIStatement guiStatement = new GUIStatement();

        guiStatement.StatementGroupRect = new Rect(20, offsetRect.y + offsetRect.height, ParentGroup.width - 40, 100);

        guiStatement.StatementTitleRect = new Rect(10, 0, guiStatement.StatementBGRect.width, 40);
        guiStatement.StatementTitle = title;

        guiStatement.AddKeyWordBtn = new Rect(guiStatement.StatementGroupRect.width - 155, 10, 150, 30);
        guiStatement.RemoveStatement = new Rect(guiStatement.StatementGroupRect.width - 310, 10, 150, 30);

        guiStatement.height = guiStatement.StatementTitleRect.y + guiStatement.StatementTitleRect.height;

        Rect prevRect = new Rect(0, 0, 0, guiStatement.StatementTitleRect.y + guiStatement.StatementTitleRect.height);
        // enabling keyword
        if(pStatement.enablingKeyword != null) {
            guiStatement.enablingKeywords.Add(pStatement.enablingKeyword.text);

            Rect enablingKeywordLabelRect = new Rect(10, prevRect.y + prevRect.height + 5, 180, 20);
            Rect enablingKeywordInputRect = new Rect(enablingKeywordLabelRect.x + enablingKeywordLabelRect.width, enablingKeywordLabelRect.y, ParentGroup.width - enablingKeywordLabelRect.x - enablingKeywordLabelRect.width - 50, 20);

            guiStatement.EnablingKeywordLabel.Add(enablingKeywordLabelRect);
            guiStatement.EnablingKeywordInput.Add(enablingKeywordInputRect);

            prevRect = enablingKeywordLabelRect;
        }
        
        // keywords
       foreach (Keyword keyword in pStatement.containedKeywords) {
            guiStatement.keywords.Add(keyword.text);
            Rect statementRectLabel = new Rect(10, prevRect.y + prevRect.height + 5, 100, 20);
            Rect statementRectInput = new Rect(statementRectLabel.x + statementRectLabel.width, prevRect.y + prevRect.height + 5, guiStatement.StatementGroupRect.width - (statementRectLabel.x + statementRectLabel.width) - 40, 20);
            Rect deleteBtn = new Rect(statementRectInput.x + statementRectInput.width + 10, statementRectInput.y, 20, 20);

            guiStatement.KeyWordsLabels.Add(statementRectLabel);
            guiStatement.KeyWordsInput.Add(statementRectInput);
            guiStatement.DeleteKeywordBtn.Add(deleteBtn);

            prevRect = statementRectLabel;

            guiStatement.height += statementRectLabel.height + 5;
        }

        guiStatement.TextLabelRect = new Rect(10, prevRect.y + prevRect.height + 5, 100, 20);
        guiStatement.TextInputRect = new Rect(guiStatement.TextLabelRect.x + guiStatement.TextLabelRect.width, guiStatement.TextLabelRect.y, guiStatement.StatementGroupRect.width - (guiStatement.TextLabelRect.x + guiStatement.TextLabelRect.width) - 10, 20);
        guiStatement.text = pStatement.displayText;

        guiStatement.ShortTextLabelRect = new Rect(10, guiStatement.TextLabelRect.y + guiStatement.TextLabelRect.height + 5, 100, 20);
        guiStatement.ShortTextInputRect = new Rect(guiStatement.ShortTextLabelRect.x + guiStatement.ShortTextLabelRect.width, guiStatement.ShortTextLabelRect.y, guiStatement.TextInputRect.width, 20);
        guiStatement.shortText = pStatement.shortenedText;

        guiStatement.height += (guiStatement.TextLabelRect.height + 5);
        guiStatement.height += (guiStatement.ShortTextLabelRect.height + 5);



        guiStatement.StatementGroupRect.height = guiStatement.height + 5;

        guiStatement.StatementBGRect = new Rect(0, 0, guiStatement.StatementGroupRect.width, guiStatement.StatementGroupRect.height);

        return guiStatement;
    }


    void SyncData() {
        SaveXMLFile(tmpFileAddress);

        xmlAccusations = AccusationParser.parse(tempXmlFile.text);
    }

    void SaveXMLFile(string pFileName) {
        //SyncData();
        List<string> fileData = new List<string>();

        fileData.Add("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        fileData.Add("<Accusations>");

        foreach (GUIAccusation guiAccusation in guiAccusationList) {
            fileData.Add("\t<Accusation timeState=\"" + guiAccusation.timeState+"\">");

            fileData.Add("\t\t<TransitionStatement>");
            foreach (string keyword in guiAccusation.TransitionStatement.keywords) {
                fileData.Add("\t\t\t<Keyword>" + keyword + "</Keyword>");
            }
            fileData.Add("\t\t\t<Text>" + guiAccusation.TransitionStatement.text + "</Text>");
            fileData.Add("\t\t\t<ShortenedText>" + guiAccusation.TransitionStatement.shortText + "</ShortenedText>");
            fileData.Add("\t\t</TransitionStatement>\n");

            // choices
            foreach (GUIAccusationChoice guiAccusationChoice in guiAccusation.AccusationChoice) {
                fileData.Add("\t\t<Choice personId=\""+guiAccusationChoice.personID+"\" defenderId=\""+guiAccusationChoice.defenderID+"\">");

                foreach(string keyword in guiAccusationChoice.enablingKeywords) {
                    fileData.Add("\t\t\t<EnablingKeyword>" + keyword + "</EnablingKeyword>");
                }

                foreach(GUIStatement guiStatement in guiAccusationChoice.investigatorIntution) {
                    fileData.Add("\t\t\t<InvestigatorIntuition>");
                    foreach (string keyword in guiStatement.enablingKeywords) {
                        fileData.Add("\t\t\t\t<EnablingKeyword>" + keyword + "</EnablingKeyword>");
                    }
                    foreach (string keyword in guiStatement.keywords) {
                        fileData.Add("\t\t\t\t<Keyword>" + keyword + "</Keyword>");
                    }
                    fileData.Add("\t\t\t\t<Text>" + guiStatement.text + "</Text>");
                    fileData.Add("\t\t\t\t<ShortenedText>" + guiStatement.shortText + "</ShortenedText>");
                    fileData.Add("\t\t\t</InvestigatorIntuition>");
                }

                foreach(GUIAccusationStatementPair guiAccusationStatementPair in guiAccusationChoice.accusationStatement) {
                    fileData.Add("\t\t\t<AccusationStatement>");
                    foreach(string keyword in guiAccusationStatementPair.question.keywords) {
                        fileData.Add("\t\t\t\t<Keyword>"+keyword+"</Keyword>");
                    }
                    fileData.Add("\t\t\t\t<Text>" + guiAccusationStatementPair.question.text + "</Text>");
                    fileData.Add("\t\t\t\t<ShortenedText>" + guiAccusationStatementPair.question.shortText + "</ShortenedText>");
                    fileData.Add("\t\t\t</AccusationStatement>");

                    fileData.Add("\t\t\t<ResultStatement>");
                    fileData.Add("\t\t\t\t<Text>" + guiAccusationStatementPair.answer.text + "</Text>");
                    fileData.Add("\t\t\t\t<ShortenedText>" + guiAccusationStatementPair.answer.shortText + "</ShortenedText>");
                    fileData.Add("\t\t\t</ResultStatement>");
                }
                
                fileData.Add("\t\t</Choice>");
            }
            
            fileData.Add("\t</Accusation>\n");
        }

        fileData.Add("</Accusations>");

        Debug.Log("Writing into temp file.");

        System.IO.File.WriteAllLines(pFileName, fileData.ToArray());

    }
}
