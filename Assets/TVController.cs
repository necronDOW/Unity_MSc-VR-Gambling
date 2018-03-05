using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(AudioSource))]
public class TVController : MonoBehaviour
{
    [SerializeField] private VideoClip[] clips;
    [SerializeField] private bool loop;
    [SerializeField] private bool playOnAwake;
    [SerializeField] private bool mute = false;
    [SerializeField, Range(0, 1)] private float[] volumes;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    private VideoPlayer videoPlayerRef;
    private AudioSource audioSrcRef;
    private Renderer rendererRef;
    private int currentClip = 0;
    private float playbackSpeed = 1;

    private void OnValidate()
    {
        audioSrcRef = GetComponent<AudioSource>();
        videoPlayerRef = GetComponent<VideoPlayer>();

        if (clips.Length != volumes.Length) {
            float[] volumesTmp = volumes;
            volumes = new float[clips.Length];
            for (int i = 0; i < volumes.Length && i < volumesTmp.Length; i++) {
                volumes[i] = volumesTmp[i];
            }
        }

        videoPlayerRef.SetTargetAudioSource(0, audioSrcRef);
        videoPlayerRef.playOnAwake = playOnAwake;
        videoPlayerRef.playbackSpeed = playbackSpeed;
        videoPlayerRef.isLooping = loop;

        SetClip(currentClip);
    }

    private void Awake()
    {
        audioSrcRef = GetComponent<AudioSource>();
        videoPlayerRef = GetComponent<VideoPlayer>();
        rendererRef = GetComponent<Renderer>();

        if (playOnAwake) {
            rendererRef.material = onMaterial;
        } else {
            rendererRef.material = offMaterial;
        }
    }

    float switchDelay = 0.0f;
    float switchDelayTimer = 0.0f;
    private void Update()
    {
        switchDelayTimer -= Time.deltaTime;

        if (!videoPlayerRef.isPlaying && switchDelayTimer <= 0.0f) {
            PlayNextClip();

            switchDelay = (float)clips[currentClip].length / playbackSpeed;
            switchDelayTimer = switchDelay;
        }

        audioSrcRef.enabled = !mute;
    }

    public void Play()
    {
        if (!videoPlayerRef.isPlaying) {
            videoPlayerRef.Play();
            rendererRef.material = onMaterial;
        }
    }

    public void Pause()
    {
        if (videoPlayerRef.isPlaying) {
            videoPlayerRef.Pause();
        }
    }

    public void Stop()
    {
        if (videoPlayerRef.isPlaying) {
            videoPlayerRef.Stop();
            rendererRef.material = offMaterial;
        }
    }

    public void PlayNextClip()
    {
        currentClip = (currentClip + 1) % clips.Length;

        Stop();
        SetClip(currentClip);
        Play();
    }

    private void SetClip(int index)
    {
        if (index >= 0 && index < clips.Length) {
            if (clips[index] != null) {
                audioSrcRef.volume = volumes[index];
                videoPlayerRef.clip = clips[index];
            }
        }
    }
}
