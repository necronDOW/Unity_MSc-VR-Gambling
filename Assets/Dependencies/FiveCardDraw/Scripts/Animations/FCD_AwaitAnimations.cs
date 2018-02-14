using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FCD_AwaitAnimations : MonoBehaviour
{
    [SerializeField] private FCD_AnimationTracker tracker;
    [SerializeField] private GameObject[] targetObjects;
    [SerializeField] private float doubleCheckTimer = 0.5f;
    [SerializeField] private bool doubleCheckOnEnable = false;
    [SerializeField] private bool doubleCheckOnDisable = false;
    private bool lastAnimationsDone;

    private void Awake()
    {
        if (tracker)
            lastAnimationsDone = !tracker.animationsDone;
    }

    public void Update()
    {
        if (tracker) {
            if (tracker.animationsDone != lastAnimationsDone)
                StartCoroutine(DoubleCheckProtocol());

            lastAnimationsDone = tracker.animationsDone;
        }
    }

    private IEnumerator DoubleCheckProtocol()
    {
        if ((tracker.animationsDone && doubleCheckOnEnable) || (!tracker.animationsDone && doubleCheckOnDisable))
            yield return new WaitForSeconds(doubleCheckTimer);

        EnableGameObjects();
        yield return null;
    }

    private void EnableGameObjects()
    {
        foreach (GameObject g in targetObjects)
            g.SetActive(tracker.animationsDone);
    }
}
