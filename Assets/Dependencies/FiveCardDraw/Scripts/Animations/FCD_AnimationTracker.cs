using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCD_AnimationTracker : MonoBehaviour
{
    [SerializeField] private int animationsCount = 1;
    [SerializeField] private bool[] animationsPlaying;
    private float doubleCheckTimer = 0.1f;
    private bool trackingOverride = false;
    public bool animationsDone {
        get {
            if (trackingOverride)
                return true;

            for (int i = 0; i < animationsCount; i++) {
                if (animationsPlaying[i])
                    return false;
            }

            return true;
        }
    }
    
    private void Awake()
    {
        animationsPlaying = new bool[animationsCount];
    }

    public void SetTrackingOverride(bool value)
    {
        trackingOverride = value;
    }

    public void SetAnimationPlaying(int index, bool value)
    {
        if (index >= 0 && index < animationsCount)
            animationsPlaying[index] = value;
        else Debug.Log("The requested index of " + index + " is not a valid tracked animation.");
    }
}
