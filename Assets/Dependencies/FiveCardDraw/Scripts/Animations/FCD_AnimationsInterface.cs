using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCD_AnimationsInterface : MonoBehaviour
{
    [SerializeField] private FCD_AnimationTracker tracker;
    [SerializeField] private int trackedAnimationIndex = 0;

    public void SetAnimationPlaying_True()
    {
        if (tracker)
            tracker.SetAnimationPlaying(trackedAnimationIndex, true);
    }

    public void SetAnimationPlaying_False()
    {
        if (tracker)
            tracker.SetAnimationPlaying(trackedAnimationIndex, false);
    }
}
