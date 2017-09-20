using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    protected AudioSource[] channels = new AudioSource[5];

    protected void ConfigureChannel(int index, bool loop, AudioClip clip, float volume = 1.0f, bool playNow = false)
    {
        if (index < channels.Length)
        {
            channels[index] = gameObject.AddComponent<AudioSource>();
            channels[index].loop = loop;
            channels[index].volume = volume;
            channels[index].clip = clip;
            channels[index].playOnAwake = playNow;

            if (playNow)
                channels[index].Play();
        }
    }

    protected void PlaySoundEffect(int channelIndex = 1)
    {
        if (channelIndex < channels.Length)
        {
            if (channels[channelIndex].isPlaying)
                channels[channelIndex].Stop();
            
            channels[channelIndex].Play();
        }
    }

    protected IEnumerator AudioLooper(int channelIndex, int count, float length = -1.0f)
    {
        if (length < 0.0f)
            length = channels[channelIndex].clip.length * count;
        
        PlaySoundEffect(channelIndex);
        yield return new WaitForSeconds(length);
        channels[channelIndex].Stop();
    }

    protected IEnumerator PlayAfterSeconds(int channelIndex, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        PlaySoundEffect(channelIndex);
    }
}