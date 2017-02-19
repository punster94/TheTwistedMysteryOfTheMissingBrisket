using UnityEngine;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public AudioClip MainTheme;
    public AudioClip AlternateTheme;
    public AudioClip AccusationTheme;
    public float FadeSpeed = 1f;
    AudioSource audSource;


    public void PlayMainTheme()
    {
        if (!audSource.isPlaying)
        {
            InstantPlayAudio(MainTheme); 
        }
        else
        {
            StartCoroutine(DoFadeToPlayAudio(MainTheme));
        }
    }

    public void PlayAlternateTheme()
    {
        if (!audSource.isPlaying)
        {
            InstantPlayAudio(AlternateTheme);
        }
        else
        {
            StartCoroutine(DoFadeToPlayAudio(AlternateTheme));
        }
    }

    public void PlayAccusationTheme()
    {
        if (!audSource.isPlaying)
        {
            InstantPlayAudio(AccusationTheme);
        }
        else
        {
            StartCoroutine(DoFadeToPlayAudio(AccusationTheme));
        }
    }

    void Awake()
    {
        audSource = GetComponent<AudioSource>();
        PlayMainTheme();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayMainTheme();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayAlternateTheme();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayAccusationTheme();
        }
    }

    void InstantPlayAudio(AudioClip clip)
    {
        audSource.clip = clip;
        audSource.Play();
    }

    IEnumerator DoFadeToPlayAudio(AudioClip clip)
    {
        print("fade: ");
        StopCoroutine("FadeOutMusic");
        yield return StartCoroutine("FadeOutMusic");
        audSource.volume = 1;
        audSource.clip = clip;
        audSource.Play();
    }

    IEnumerator FadeOutMusic()
    {
        while (audSource.volume > 0)
        {            
            audSource.volume -= FadeSpeed * Time.deltaTime;
            yield return null;
        }
        audSource.volume = 0;
    }
}
