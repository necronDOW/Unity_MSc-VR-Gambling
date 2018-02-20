using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : SteamVR_TrackedController
{
    //private const float animationFinishSpeed = 5.0f;
    private const float defaultPlaySpeed = 4.0f;

    [System.Serializable]
    public struct AnimationPausePair
    {
        public AnimationClip clip;
        public float pauseAfterTime;
    }

    [SerializeField] private GameObject customModel;
    [SerializeField] private bool isLeftHand = false;
    [SerializeField] private AnimationPausePair[] animationPausePairs;
    [SerializeField] private int triggerAnimationIndex = -1;
    [SerializeField] private int grippedAnimationIndex = -1;
    [SerializeField] private int triggerAndGrippedAnimationIndex = -1;

    private Animation animationComponent;

    private void Awake()
    {
        if (customModel != null) {
            customModel = Instantiate(customModel, transform);

            if (isLeftHand) {
                customModel.transform.localScale *= -1;
                customModel.transform.rotation = Quaternion.Euler(
                    customModel.transform.eulerAngles.x,
                    customModel.transform.eulerAngles.y - 180,
                    customModel.transform.eulerAngles.z + 180);
            }
        }

        animationComponent = customModel.GetComponent<Animation>();
        if (animationComponent != null) {
            for (int i = 0; i < animationPausePairs.Length; i++) {
                animationComponent.AddClip(animationPausePairs[i].clip, animationPausePairs[i].clip.name);
            }

            if (animationPausePairs.Length >= 1 && animationPausePairs[0].clip != null) {
                PlayAnimation(0, 0.1f);
            }

            TriggerClicked      += HandleTriggerPress;
            Gripped             += HandleGripPress;

            TriggerUnclicked    += HandleTriggerRelease;
            Ungripped           += HandleGripRelease;
        }
        else {
            Debug.LogWarning("No Animation Component found on " + gameObject.name + " or children. Animations will not play.");
        }
    }

    private void OnValidate()
    {
        if (triggerAnimationIndex >= animationPausePairs.Length) {
            triggerAnimationIndex = -1;
        }

        if (grippedAnimationIndex >= animationPausePairs.Length) {
            grippedAnimationIndex = -1;
        }
    }

    #region Hooks
    private void HandleTriggerPress(object sender, ClickedEventArgs e)
    {
        if (!gripped) PlayAnimation(triggerAnimationIndex);
        else PlayAnimation(triggerAndGrippedAnimationIndex);
    }
    
    private void HandleGripPress(object sender, ClickedEventArgs e)
    {
        if (!triggerPressed) PlayAnimation(grippedAnimationIndex);
        else PlayAnimation(triggerAndGrippedAnimationIndex);
    }

    private void HandleTriggerRelease(object sender, ClickedEventArgs e)
    {
        if (!gripped) PlayAnimation(0);
        else PlayAnimation(grippedAnimationIndex);
    }

    private void HandleGripRelease(object sender, ClickedEventArgs e)
    {
        if (!triggerPressed) PlayAnimation(0);
        else PlayAnimation(triggerAnimationIndex);
    }
    #endregion

    public void PlayAnimation(int index, float delay = 0.0f)
    {
        if (animationComponent != null && index >= 0 && index < animationPausePairs.Length && animationPausePairs[index].clip != null) {
            StartCoroutine(PlaySequence(animationPausePairs[index], delay));
        }
    }

    private IEnumerator PlaySequence(AnimationPausePair animationPausePair, float delay = 0.0f)
    {
        if (delay > 0.0f) {
            yield return new WaitForSeconds(delay);
        }

        //// Finish current animation quickly.
        //SetAnimationSpeed(animationFinishSpeed);
        //while (animationComponent.isPlaying) {
        //    yield return null;
        //}

        // Play new animation.
        SetAnimationSpeed(defaultPlaySpeed);
        //animationComponent.clip = animationPausePair.clip;
        //animationComponent.Play();
        animationComponent.CrossFade(animationPausePair.clip.name);
        animationComponent.clip = animationPausePair.clip;

        // Start a pause sequence if necessary.
        StartCoroutine(PauseSequence(animationPausePair));

        yield return null;
    }

    private IEnumerator PauseSequence(AnimationPausePair animationPausePair)
    {
        while (GetAnimationTime(animationPausePair.clip.name) < animationPausePair.pauseAfterTime) {
            yield return null;
        }

        SetAnimationSpeed(0.0f);
        yield return null;
    }

    private void SetAnimationSpeed(float value)
    {
        foreach (AnimationState state in animationComponent) {
            state.speed = value;
        }
    }

    private float GetAnimationTime(string name)
    {
        foreach (AnimationState state in animationComponent) {
            if (state.name == name)
                return state.time;
        }

        return 1.0f;
    }
}
