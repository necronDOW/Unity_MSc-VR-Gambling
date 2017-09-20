using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceAudioManager : AudioManager
{
    public AudioClip stopSignalSound;
    public AudioClip selectSound;

    private void Awake()
    {
        ConfigureChannel(0, false, stopSignalSound);
        ConfigureChannel(1, false, selectSound);
    }

    public void PlaySelectSound()
    {
        PlaySoundEffect(1);
    }

    public void PlayStopSignalSound(float delay = 0.1f)
    {
        StartCoroutine(PlayAfterSeconds(0, delay));
    }
}
