using UnityEngine;
using System.Collections;
using System.Xml;

public class DialogManager : MonoBehaviour
{
    public delegate void AddLogEntry(int personID, string shortenedText, string fullText);
    public static event AddLogEntry OnAddLogEntry;

    public GameManager gameManager;
    public DialogUIManager uiManager;
    public Player player;

    public TextAsset dialogFile;
    public TextAsset accusationFile;

    public int numberOfAccusables;

    private enum ConversationState { InvalidState, NotTalking, StartingStatement, Choices, Question, Answer, WaitingToAdvance, MaxState }
    private enum AccusationState { InvalidState, Transition, Selecting, Results, MaxState }

    private ConversationState conversationState;

    private ArrayList dialogues;
    private ArrayList accusations;

    private ArrayList seenKeywords;

    private Dialog currentDialog;
    private Question currentQuestion;
    private int currentQuestionIndex;
    private bool stateChanged;

    private Accusation currentAccusation;
    private AccusationState accusationState;
    private Person accused;
    private ArrayList currentAccused;
    private AccusationChoice chosenAccused;
    private int currentAccusationPair;

    private int selectedPerson;

    private bool filteredAccused;

	void Start ()
    {
        stateChanged = false;
        conversationState = ConversationState.NotTalking; 
        seenKeywords = new ArrayList();
        dialogues = DialogParser.parse(dialogFile.text);
        accusations = AccusationParser.parse(accusationFile.text);
        accusationState = AccusationState.InvalidState;
	}
	
	void Update ()
    {
        switch(gameManager.gamePhase)
        {
            case GamePhase.Questioning:
                questioningUpdate();
                break;
            case GamePhase.TimeUp:
                transitionUpdate();
                break;
            case GamePhase.Accusation:
                accusationUpdate();
                break;
        }
	}
    
    public bool keywordHasBeenSeen(string keyword)
    {
        return seenKeywords.Contains(Keyword.getKeyword(keyword));
    }

    private void transitionUpdate()
    {
        switch (accusationState)
        {
            case AccusationState.Transition:
                selectCurrentAccusation();
                displayAccusationTransitionStatement();
                conversationState = ConversationState.NotTalking;
                break;
        }
    }

    private void accusationUpdate()
    {
        switch (accusationState)
        {
            case AccusationState.Transition:
                accusationState = AccusationState.Selecting;
                uiManager.displayAccusationScreen();

                break;
            case AccusationState.Selecting:
                handleAccusationSelection();
                break;
            case AccusationState.Results:
                showAccusationDialog();
                break;
        }

    }

    private void handleAccusationSelection()
    {
        if (!filteredAccused)
        {
            filteredAccused = true;

            currentAccused = currentAccusation.filteredChoices(seenKeywords);
            numberOfAccusables = currentAccusation.sizeOfLastFilteredChoices();
            uiManager.displayAccusedList(currentAccusation, seenKeywords, gameManager.getPerson(((AccusationChoice)currentAccused[0]).personId));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            accusationState = AccusationState.Results;
            int accusedPersonID = uiManager.hideAccusationScreen();
            accused = gameManager.getPerson(accusedPersonID);
            AccusationChoice choice = currentAccusation.getAccusationChoice(accusedPersonID - 1);
            chosenAccused = choice;
            currentAccusationPair = 0;

            Keyword enabler = new Keyword("", 0);
            Question accusationDialog = (Question)choice.statementPairs[0];
            currentQuestion = accusationDialog;

            changeState(ConversationState.Question);
            uiManager.displayQuestion(currentQuestion);
            uiManager.setPersonPotrait(choice.defenderId);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedPerson = (selectedPerson + numberOfAccusables - 1) % numberOfAccusables;
            uiManager.selectAccused(selectedPerson, currentAccusation.getAccusationChoice(selectedPerson), seenKeywords, gameManager.getPerson(((AccusationChoice)currentAccused[selectedPerson]).personId));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedPerson = (selectedPerson + 1) % numberOfAccusables;
            uiManager.selectAccused(selectedPerson, currentAccusation.getAccusationChoice(selectedPerson), seenKeywords, gameManager.getPerson(((AccusationChoice)currentAccused[selectedPerson]).personId));
        }
    }
    
    private void showAccusationDialog()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (conversationState)
            {
                case ConversationState.Question:
                    changeState(ConversationState.Answer);
                    uiManager.displayQuestion(currentQuestion);
                    addKeywordsFromStatement(currentQuestion.question);
                    break;
                case ConversationState.Answer:
                    uiManager.displayAnswer(currentQuestion);
                    addKeywordsFromStatement(currentQuestion.answer);

                    if (currentAccusationPair < chosenAccused.statementPairs.Count - 1)
                    {
                        currentAccusationPair++;
                        currentQuestion = (Question)chosenAccused.statementPairs[currentAccusationPair];
                        changeState(ConversationState.Question);
                    }
                    else
                        changeState(ConversationState.WaitingToAdvance);
                    break;
                case ConversationState.WaitingToAdvance:
                    changeState(ConversationState.NotTalking);
                    StartCoroutine(gameManager.advanceTimeState(chosenAccused.personId));
                    break;
            }
        }
    }

    private void selectCurrentAccusation()
    {
        foreach (Accusation accusation in accusations)
        {
            if (accusation.timeState == gameManager.timeState)
            {
                currentAccusation = accusation;
                break;
            }
        }
    }

    private void displayAccusationTransitionStatement()
    {
        selectedPerson = 0;
        uiManager.displayTimeUpForAccusation(currentAccusation, gameManager.transitionPerson);
    }

    private void questioningUpdate()
    {
        if (stateChanged)
        {
            stateChanged = false;

            switch (conversationState)
            {
                case ConversationState.StartingStatement:
                    uiManager.displayStartingStatement(currentDialog);
                    addKeywordsFromStatement(currentDialog.startingStatement);
                    break;
                case ConversationState.Choices:
                    if (currentDialog.questions.Count == 0)
                        changeState(ConversationState.NotTalking);

                    uiManager.displayChoices(currentDialog, seenKeywords, currentQuestion);
                    break;
                case ConversationState.Question:
                    uiManager.displayQuestion(currentQuestion);
                    addKeywordsFromStatement(currentQuestion.question);
                    break;
                case ConversationState.Answer:
                    uiManager.displayAnswer(currentQuestion);
                    addKeywordsFromStatement(currentQuestion.answer);

                    if (OnAddLogEntry != null)
                        OnAddLogEntry(currentDialog.personId, currentQuestion.answer.highlightedShortenedText(), currentQuestion.answer.highlightedDisplayText());
                    break;
                default:
                    uiManager.removeDisplay();
                    player.doneTalking();
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            switch (conversationState)
            {
                case ConversationState.StartingStatement:
                    changeState(ConversationState.Choices);
                    currentQuestionIndex = 0;
                    currentQuestion = (Question)currentDialog.filteredQuestions(seenKeywords)[currentQuestionIndex];
                    break;
                case ConversationState.Choices:
                    currentQuestion = (Question)currentDialog.questions[currentQuestionIndex];
                    changeState(ConversationState.Question);
                    break;
                case ConversationState.Question:
                    changeState(ConversationState.Answer);
                    break;
                case ConversationState.Answer:
                    changeState(ConversationState.NotTalking);
                    break;
                default:
                    break;
            }
        }

        if (conversationState == ConversationState.Choices)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                ArrayList filteredQuestions = currentDialog.filteredQuestions(seenKeywords);
                currentQuestionIndex = currentDialog.getFilteredQuestionIndex(currentQuestion, seenKeywords);
                currentQuestionIndex = (currentQuestionIndex + 1) % filteredQuestions.Count;
                currentQuestionIndex = currentDialog.getQuestionIndex((Question)filteredQuestions[currentQuestionIndex]);
                currentQuestion = (Question)currentDialog.questions[currentQuestionIndex];
                uiManager.displayChoices(currentDialog, seenKeywords, currentQuestion);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                ArrayList filteredQuestions = currentDialog.filteredQuestions(seenKeywords);
                currentQuestionIndex = currentDialog.getFilteredQuestionIndex(currentQuestion, seenKeywords);
                int filteredCount = filteredQuestions.Count;
                currentQuestionIndex = (currentQuestionIndex + filteredCount - 1) % filteredCount;
                currentQuestionIndex = currentDialog.getQuestionIndex((Question)filteredQuestions[currentQuestionIndex]);
                currentQuestion = (Question)currentDialog.questions[currentQuestionIndex];
                uiManager.displayChoices(currentDialog, seenKeywords, currentQuestion);
            }
        }
    }

    public void startTalkingToPerson(Person person)
    {
        gameManager.addPersonTalkedTo(person);

        if (gameManager.gamePhase == GamePhase.TimeUp)
        {
            accused = person;
            filteredAccused = false;
            accusationState = AccusationState.Transition;
        }

        foreach(Dialog dialog in dialogues)
        {
            if (dialog.personId == person.personId && dialog.timeState == gameManager.timeState)
            {
                currentDialog = dialog;
                changeState(ConversationState.StartingStatement);
                return;
            }
        }
    }

    public void addKeywordsFromStatement(Statement statement)
    {
        foreach (Keyword keyword in statement.containedKeywords)
        {
            if (!seenKeywords.Contains(keyword))
                seenKeywords.Add(keyword);
        }
    }

    private void changeState(ConversationState state)
    {
        conversationState = state;
        stateChanged = true;
    }
}
