using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BtnAudio : MonoBehaviour
{
    public AudioSource output;
    public AudioClip clickSound;
    [Range(0, 1)] public float clickSoundVolume = 1.0f;
    public AudioClip hoverSound;
    [Range(0, 1)] public float hoverSoundVolume = 1.0f;

    private EventTrigger trigger;
    private Button uiButton;

    private void Awake()
    {
        uiButton = GetComponent<Button>();

        if (!output)
        {
            if (!GetComponent<AudioSource>())
                output = gameObject.AddComponent<AudioSource>();
            else output = GetComponent<AudioSource>();

            output.playOnAwake = false;
        }

        if (!GetComponent<EventTrigger>())
            trigger = gameObject.AddComponent<EventTrigger>();
        else trigger = GetComponent<EventTrigger>();

        AddEventListener(EventTriggerType.PointerClick, PlayClicked);
        AddEventListener(EventTriggerType.PointerEnter, PlayHover);
    }

    public void PlayClicked()
    {
        PlaySound(clickSound, clickSoundVolume);
    }

    private void PlayClicked(BaseEventData eventData)
    {
        if (uiButtonInteractable())
            PlaySound(clickSound, clickSoundVolume);
    }

    private void PlayHover(BaseEventData eventData)
    {
        if (uiButtonInteractable())
            PlaySound(hoverSound, hoverSoundVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip)
        {
            output.clip = clip;
            output.volume = volume;
            output.Play();
        }
    }

    private bool uiButtonInteractable()
    {
        return !uiButton || uiButton.interactable;
    }

    private void AddEventListener(EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}
