using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum TimeState { InvalidTime, Time1, Time2, Time3, MaxTime }

public enum GamePhase { InvalidPhase, Questioning, TimeUp, Accusation, GameComplete, MaxPhase }

public class GameManager : MonoBehaviour
{
    public const int menuIndex = 0;
    public const int mainSceneIndex = 1;
    public const int goodEndIndex = 2;
    public const int badEndIndex = 3;

    public static GameManager Instance;

    public TimeState timeState;
    public GamePhase gamePhase;
    public GameObject personBucket;

    public Player player;

    public Person transitionPerson;

    public int[] dialogsPerTimeState;

    public int idOfActualBandit;

    public GenericBlackFader fader;

    private ArrayList peopleTalkedTo;
    private Person[] personsInScene;

    void Awake ()
    {
        Instance = this;

        timeState = TimeState.Time1;
        gamePhase = GamePhase.Questioning;

        peopleTalkedTo = new ArrayList();

        PopulatePersonsInScene();
        relocatePersonsInScene();
    }
	
	void Update ()
    {
	    switch(gamePhase)
        {
            case GamePhase.Questioning:
                questioningUpdate();
                break;
            case GamePhase.TimeUp:
                timeUpUpdate();
                break;
            case GamePhase.Accusation:
                accusationUpdate();
                break;
            case GamePhase.GameComplete:
                gameCompleteUpdate();
                break;
        }
	}

    private void questioningUpdate()
    {

    }

    private void timeUpUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            peopleTalkedTo = new ArrayList();
            gamePhase = GamePhase.Accusation;
        }
    }

    private void accusationUpdate()
    {

    }

    private void gameCompleteUpdate()
    {

    }

    public IEnumerator advanceTimeState(int chosen = 0)
    {
        switch (timeState)
        {
            case TimeState.Time1:
                timeState = TimeState.Time2;
                break;
            case TimeState.Time2:
                timeState = TimeState.Time3;
                break;
            case TimeState.Time3:
                moveToEndScreen(chosen);
                break;
        }

        yield return StartCoroutine(fader.FadeIn());
        relocatePersonsInScene();
        gamePhase = GamePhase.Questioning;
        StartCoroutine(fader.FadeOut());
    }

    private void moveToEndScreen(int chosen)
    {
        if (chosen == idOfActualBandit)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(3);
        }
    }

    public void addPersonTalkedTo(Person person)
    {
        if (!peopleTalkedTo.Contains(person))
        {
            if (peopleTalkedTo.Count == dialogsPerTimeState[currentTimeStateIndex()])
            {
                gamePhase = GamePhase.TimeUp;
                transitionPerson = person;
            }
            else
            {
                peopleTalkedTo.Add(person);
            }
        }
    }

    private int currentTimeStateIndex()
    {
        int index;

        switch (timeState)
        {
            case TimeState.Time2:
                index = 1;
                break;
            case TimeState.Time3:
                index = 2;
                break;
            default:
                index = 0;
                break;
        }

        return index;
    }

    private void PopulatePersonsInScene()
    {
        personsInScene = personBucket.transform.GetComponentsInChildren<Person>();
    }

    private void relocatePersonsInScene()
    {
        int index = currentTimeStateIndex();

        foreach (Person person in personsInScene)
        {
            person.gameObject.transform.localPosition = person.positionsAtEachTime[index];
        }

        player.gameObject.transform.localPosition = player.positionsAtEachTime[index];
}

    public Person getPerson(int personID)
    {
        if (personsInScene.Length > personID)
            return personsInScene[personID];

        return null;
    }
}
