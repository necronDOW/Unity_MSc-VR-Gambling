using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    public Renderer renderTarget;

    [HideInInspector]
    public bool showFace = false, animationDone = false, readyToReveal = false;

    [HideInInspector]
    public int value;

    protected DealerScript dealer;

    private Animator animator;
    private Transform target;
    private int pairIndex;
    private float animationTime = 0.4f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        dealer = HelperTools.FindLocalGameObjectWithTag("Dealer", gameObject.scene).GetComponent<DealerScript>();

        StartCoroutine(MoveOverTime());
    }

    private void Update()
    {
        if (!animationDone)
            Animate();
    }

    public void SetParameters(int value, Transform target, int pairIndex, float animationSpeed = 1.0f)
    {
        this.value = value;
        this.target = target;
        this.pairIndex = pairIndex;

        animationTime /= animationSpeed;

        if (!animator)
            animator = GetComponentInChildren<Animator>();
        animator.speed *= animationSpeed;
    }

    protected virtual void ShowFace() { }

    private void Animate()
    {
        if (readyToReveal && dealer.cardsInPlay[pairIndex].readyToReveal)
        {
            animator.SetTrigger("Reveal");

            if (showFace)
                ShowFace();
        }
    }

    private IEnumerator MoveOverTime()
    {
        float t = 0.0f;
        Vector3 currentPosition = transform.position;

        while (t < 1.0f)
        {
            if (target)
            {
                t += Time.deltaTime / animationTime;
                transform.position = Vector3.Lerp(currentPosition, target.position, t);
                yield return null;
            }
        }

        transform.position = target.position;
        readyToReveal = true;
    }
}