using UnityEngine;

public class AudioInterface : MonoBehaviour
{
    public AudioSource targetSource;
    [Range(0, 1)] public float audioVolume = 1.0f;
    [Range(-3, 3)] public float audioPitch = 1.0f;
    public AudioClip audioClip;

    protected virtual void Awake()
    {
        if (audioClip && !targetSource) {
            targetSource = GetComponent<AudioSource>();

            if (!targetSource)
                targetSource = gameObject.AddComponent<AudioSource>();

            targetSource.loop = false;
        }
    }

    public virtual void PlayClip()
    {
        if (!targetSource.isPlaying)
        {
            targetSource.clip = audioClip;
            targetSource.volume = audioVolume;
            targetSource.pitch = audioPitch;
            targetSource.Play();
        }
    }
}
