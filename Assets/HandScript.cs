using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    private const float animationFinishSpeed = 5.0f;

    [System.Serializable]
    public struct AnimationState
    {
        public AnimationClip clip;
        public float pauseAfterSeconds;
    }

    [SerializeField] private bool isLeftHand = false;
    [SerializeField] private AnimationState[] animationStates;

    private Animation animationComponent;

    private void Awake()
    {
        if (isLeftHand) {
            transform.localScale *= -1;
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x, 
                transform.eulerAngles.y - 180, 
                transform.eulerAngles.z + 180);
        }

        animationComponent = GetComponent<Animation>();
        if (animationComponent != null) {
            for (int i = 0; i < animationStates.Length; i++) {
                animationComponent.AddClip(animationStates[i].clip, animationStates[i].clip.name);
            }
        }
    }

    // TODO: Remove - This is test code.
    float timer = 0.0f;
    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 5.0f) {
            PlayAnimation(0);
            timer = 0.0f;
        }
    }

    public void PlayAnimation(int index)
    {
        if (animationComponent != null && index >= 0 && index < animationStates.Length) {
            StartCoroutine(PlaySequence(animationStates[index]));
        }
    }

    private IEnumerator PlaySequence(AnimationState animationState)
    {
        // Finish current animation quickly.
        SetAnimationSpeed(animationFinishSpeed);
        while (animationComponent.isPlaying) {
            yield return null;
        }

        // Play new animation.
        SetAnimationSpeed(1.0f);
        animationComponent.clip = animationState.clip;
        animationComponent.Play();

        // Start a pause sequence if necessary.
        StartCoroutine(PauseSequence(animationState));

        yield return null;
    }

    private IEnumerator PauseSequence(AnimationState animationState)
    {
        if (animationState.pauseAfterSeconds > 0.0f) {
            yield return new WaitForSeconds(animationState.pauseAfterSeconds);
            SetAnimationSpeed(0.0f);
        }

        yield return null;
    }

    private void SetAnimationSpeed(float value)
    {
        foreach (UnityEngine.AnimationState state in animationComponent) {
            state.speed = value;
        }
    }
}
