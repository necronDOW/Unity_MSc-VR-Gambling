using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    public int value { get; private set; }
    public bool animationDone = false;
    
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetValue(int value)
    {
        this.value = value;
        animator.SetInteger("Value", value);
    }
}
