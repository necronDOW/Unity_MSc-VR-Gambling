using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCardLossOutcome : StateMachineBehaviour
{
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MaterialFadeScript mfs = GetMFS(animator.GetComponent<FCD_CardBehaviour>());
        mfs.BeginFadeOut(0.25f, 0.5f);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MaterialFadeScript mfs = GetMFS(animator.GetComponent<FCD_CardBehaviour>());
        mfs.ResetColor();
    }

    public static MaterialFadeScript GetMFS(FCD_CardBehaviour cardBehaviour)
    {
        if (cardBehaviour) {
            MaterialFadeScript mfs = cardBehaviour.GetComponentInChildren<MaterialFadeScript>();
            return mfs;
        }

        return null;
    }
}
