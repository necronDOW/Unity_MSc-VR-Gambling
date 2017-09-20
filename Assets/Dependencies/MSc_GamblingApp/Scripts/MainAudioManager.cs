using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAudioManager : AudioManager
{
    public AudioClip paySound;
    public AudioClip winSound;
    public AudioClip bigWinSound;
    public AudioClip loseSound;

    private void Awake()
    {
        ConfigureChannel(0, true, winSound);
        ConfigureChannel(1, true, bigWinSound, 0.25f);
        ConfigureChannel(2, false, paySound);
        ConfigureChannel(3, false, loseSound);
    }

    public void PlayPaySound()
    {
        PlaySoundEffect(2);
    }

    public void PlayWinSound(int count)
    {
        StartCoroutine(AudioLooper(0, count));

        if (count > 9)
            StartCoroutine(AudioLooper(1, count, channels[0].clip.length * count));
    }

    public void PlayLoseSound()
    {
        PlaySoundEffect(3);
    }
}
