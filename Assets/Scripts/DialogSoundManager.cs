using UnityEngine;
using System.Collections;

public class DialogSoundManager : MonoBehaviour
{
    public AudioClip shortDialogSound;
    public AudioClip mediumDialogSound;
    public AudioClip longDialogSound;
    public AudioClip selectionSound;
    public AudioSource currentSound;

    void Start ()
    {
        
    }
    
    public void playShortDialogSound()
    {
        playSound(shortDialogSound);
    }

    public void playMediumDialogSound()
    {
        playSound(mediumDialogSound);
    }
    public void playLongDialogSound()
    {
        playSound(longDialogSound);
    }

    public void playSelectSound()
    {
        playSound(selectionSound);
    }

    private void playSound(AudioClip sound)
    {
        currentSound.Stop();
        currentSound.clip = sound;
        currentSound.Play();
    }
}
