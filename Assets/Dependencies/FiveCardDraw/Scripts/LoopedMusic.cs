using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopedMusic : MonoBehaviour
{
    public AudioClip introClip;
    public AudioClip loopClip;
    
    private float introLength, introPosition;
    private AudioSource audioSrc;

    private bool isIntro {
        get { return introPosition < introLength; }
    }

    private void Awake()
    {
        if (introClip != null)
        {
            introLength = introClip.length;
            audioSrc = GetComponent<AudioSource>();
            ChangeAudioSrcClip(introClip);
        }
    }

    private void Update()
    {
        if (isIntro) {
            introPosition += Time.deltaTime;
            if (introPosition >= introLength) {
                ChangeAudioSrcClip(loopClip, true);
            }
        }
    }

    private void ChangeAudioSrcClip(AudioClip newClip, bool loop = false)
    {
        if (audioSrc && audioSrc.enabled && newClip != null) {
            audioSrc.clip = newClip;
            audioSrc.Play();
            audioSrc.loop = loop;
        }
    }
}
