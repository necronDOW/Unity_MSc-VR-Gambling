using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_AnimatorInterface : MonoBehaviour
{
    [SerializeField] private FX_Animator targetFXAnimator;
    [SerializeField] int defaultAnimationIndex = 0;

    public void PlayAnimation()
    {
        PlayAnimation(defaultAnimationIndex);
    }

    public void PlayAnimation(int customAnimationIndex)
    {
        if (targetFXAnimator) {
            targetFXAnimator.Trigger(customAnimationIndex);
        }
    }

    public static IEnumerator PlayAnimatorInterfaceAfterSeconds(float seconds, FX_AnimatorInterface animatorInterface, int animationIndex = -1)
    {
        yield return new WaitForSeconds(seconds);

        if (animatorInterface) {
            if (animationIndex == -1) {
                animatorInterface.PlayAnimation();
            }
            else {
                animatorInterface.PlayAnimation(animationIndex);
            }
        }

        yield return null;
    }
}
