using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogUIManager : MonoBehaviour
{
    public GameObject personBucket;
    public DialogSoundManager soundManager;

    public Image dialogBG;
    public Image playerPortrait;
    public Image personPortrait;
    public Text dialogText;

    public GameObject accusationSystem;
    public GameObject peopleInLineup;
    public GameObject peopleInCurrentLineup;
    public Text selectedPersonNameText;
    public int spaceBetweenPeople;

    public Image accusationBG;

    public GameObject portraits;

    public Color selectableColor;
    public Color inselectableColor;

    public int charactersForMediumBlah;
    public int charactersForLongBlah;

    private int lastIndexSelected;
    private ArrayList currentAccusables;
    
	void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}

    public void selectAccused(int index, AccusationChoice choice, ArrayList seenKeywords, Person person)
    {
        lastIndexSelected = index;

        Vector3 newPosition = peopleInCurrentLineup.transform.localPosition;
        newPosition.x = -index * spaceBetweenPeople;

        StartCoroutine(movePeopleInCurrentLineup(newPosition));
        changeAccusationText(choice, seenKeywords, person.personName);

        soundManager.playSelectSound();
    }

    IEnumerator movePeopleInCurrentLineup(Vector3 destinationPos) {
        Vector3 currentPos = peopleInCurrentLineup.transform.localPosition;
        float lerpParam = 0.0f;
        float timeElapsed = 0.0f;
        
        float angle = 0.0f;
        float angleVelocity = 2.0f * Mathf.PI;
        
        while (lerpParam <= 1.0f) {
            timeElapsed += Time.deltaTime;
            
            angle += angleVelocity * Time.deltaTime;
            if(angle > Mathf.PI / 2.0f) {
                lerpParam = 1.1f;
            }
            else {
                lerpParam = Mathf.Sin(angle);
            }
            Vector3 interimPos = Vector3.Lerp(currentPos, destinationPos, lerpParam);
            peopleInCurrentLineup.transform.localPosition = interimPos;

            yield return null;
        }
        yield return null;
    }

    private void changeAccusationText(AccusationChoice choice, ArrayList seenKeywords, string name)
    {
        selectedPersonNameText.text = name;
        dialogText.text = choice.furthestIntuition(seenKeywords).highlightedDisplayText();
    }

    public void displayAccusedList(Accusation accusation, ArrayList seenKeywords, Person person)
    {
        foreach (Transform child in peopleInCurrentLineup.transform)
        {
            Destroy(child.gameObject);
        }

        currentAccusables = accusation.filteredChoices(seenKeywords);

        for (int i = 0; i < currentAccusables.Count; i++)
        {
            Transform sourcePerson = peopleInLineup.transform.GetChild(((AccusationChoice)currentAccusables[i]).personId);

            Transform displayedPerson = GameObject.Instantiate(sourcePerson);
            displayedPerson.parent = peopleInCurrentLineup.transform;

            Vector3 newPosition = sourcePerson.localPosition;
            newPosition.x = i * spaceBetweenPeople;
            displayedPerson.localPosition = newPosition;

            displayedPerson.localScale = sourcePerson.localScale;
        }

        peopleInCurrentLineup.transform.localPosition = new Vector3();

        changeAccusationText((AccusationChoice)currentAccusables[0], seenKeywords, person.personName);
        playerTalking();
    }
    
    public void displayTimeUpForAccusation(Accusation accusation, Person transitionPerson)
    {
        personPortrait.sprite = transitionPerson.portrait;
        dialogText.text = accusation.transitionStatement.transitionStatementText();
        personTalking();
    }

    public void displayAccusationScreen()
    {
        removeDisplay();
        accusationSystem.SetActive(true);
    }

    public int hideAccusationScreen()
    {
        int accusedPersonID = lastIndexSelected + 1;

        removeDisplay();

        soundManager.playSelectSound();

        return accusedPersonID;
    }

    private Person selectPersonFromId(int personId)
    {
        Person resultingPerson = null;

        foreach(Person person in personBucket.GetComponentsInChildren<Person>())
        {
            if (person.personId == personId)
                resultingPerson = person;
        }

        return resultingPerson;
    }

    public void setPersonPotrait(int personID)
    {
        personPortrait.sprite = selectPersonFromId(personID).portrait;
    }

    public void displayStartingStatement(Dialog currentDialog)
    {
        setPersonPotrait(currentDialog.personId);
        
        dialogText.text = currentDialog.startingStatement.highlightedDisplayText();

        personTalking();
    }

    public void displayChoices(Dialog currentDialog, ArrayList seenKeywords, Question currentQuestion)
    {
        dialogText.text = "";

        ArrayList filtered = currentDialog.filteredQuestions(seenKeywords);

        foreach (Question question in filtered)
        {
            if (question == currentQuestion)
            {
                dialogText.text += question.question.colorizedShortenedText() + "\n";
            }
            else
            {
                dialogText.text += question.question.highlightedShortenedText() + "\n";
            }
        }

        playerTalking();

        soundManager.playSelectSound();
    }

    public void displayQuestion(Question question)
    {
        dialogText.text = question.question.highlightedDisplayText();
        playerTalking();

        playDialogSound();
    }

    public void displayAnswer(Question question)
    {
        dialogText.text = question.answer.highlightedDisplayText();
        personTalking();
    }

    public void removeDisplay()
    {
        setDialogBox(false);
        personPortrait.gameObject.SetActive(false);
        playerPortrait.gameObject.SetActive(false);
        accusationSystem.SetActive(false);
    }

    private void playerTalking()
    {
        setDialogBox(true);
        playerPortrait.gameObject.SetActive(true);
        personPortrait.gameObject.SetActive(false);
    }

    private void personTalking()
    {
        setDialogBox(true);
        playerPortrait.gameObject.SetActive(false);
        personPortrait.gameObject.SetActive(true);

        playDialogSound();
    }

    private void setDialogBox(bool active)
    {
        dialogBG.gameObject.SetActive(active);
        dialogText.gameObject.SetActive(active);
    }

    private void playDialogSound()
    {
        if (dialogText.text.Length < charactersForMediumBlah)
            soundManager.playShortDialogSound();
        else if (dialogText.text.Length < charactersForLongBlah)
            soundManager.playMediumDialogSound();
        else
            soundManager.playLongDialogSound();
    }
}
