using System.Collections;
using UnityEngine;

public class FCD_CardBehaviour : HL_CardBehaviour
{
    public bool ValueIsVisible { get; private set; }
    
    public Animator animator { get; private set; }
    public bool InitialFlip { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        ValueIsVisible = false;
        InitialFlip = true;
    }

    public override void FlipCard(int cardValue)
    {
        InitialFlip = false;
        
        if (ValueIsVisible) {
            TriggerAnimatorReset();
            ValueIsVisible = false;
        }

        this.CardValue = cardValue;
        animator.SetTrigger("Flip");
    }

    public void Rebind()
    {
        animator.Rebind();
        GetComponentInChildren<MaterialFadeScript>().ResetColor();
        
        PlayShimmer(false);
        ValueIsVisible = false;
    }

    //private IEnumerator FlipRoutine(int cardValue, float delay = 0.0f)
    //{
    //    if (delay > 0.0f)
    //        yield return new WaitForSeconds(delay);

    //    this.CardValue = cardValue;
    //    animator.SetTrigger("Flip");
    //}

    private float GetAnimationClipLength(Animator animator, string clipName)
    {
        AnimationClip[] ac = animator.runtimeAnimatorController.animationClips;

        for (int i = 0; i < ac.Length; i++) {
           if (ac[i].name == clipName)
                return ac[i].length;
        }

        return 0.0f;
    }

    public void CardFlipDone()
    {
        ValueIsVisible = true;

        if (this.CardValue % FCD_Deck.valueCount == 12) {
            PlayShimmer(true);
        }

        TimedDataLogger.Get().OverrideLastRecordedTime();
    }

    public void TriggerAnimatorReset()
    {
        animator.SetTrigger("Reset");
        PlayShimmer(false);
    }
}