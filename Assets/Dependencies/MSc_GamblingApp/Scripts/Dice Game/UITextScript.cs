using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextScript : MonoBehaviour
{
    private Animator animator;
    private Text[] uiText;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        uiText = GetComponentsInChildren<Text>();
    }

    protected void Animate(string newText, string animatorTriggerID = "Animate")
    {
        Visible(true);

        for (int i = 0; i < uiText.Length; i++)
            uiText[i].text = newText;

        animator.SetTrigger(animatorTriggerID);
    }

    protected void Visible(bool value)
    {
        for (int i = 0; i < uiText.Length; i++)
            uiText[i].gameObject.SetActive(value);
    }
}
