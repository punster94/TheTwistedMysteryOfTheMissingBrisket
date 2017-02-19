using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public GenericBlackFader fader;    

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReplayGame()
    {
        StartCoroutine(GoToLevel(GameManager.mainSceneIndex));
    }

    public void BackToMenu()
    {
        StartCoroutine(GoToLevel(GameManager.menuIndex));
    }

    IEnumerator GoToLevel(int sceneIndex)
    {
        yield return StartCoroutine(fader.FadeIn());
        SceneManager.LoadScene(sceneIndex);
    }
}