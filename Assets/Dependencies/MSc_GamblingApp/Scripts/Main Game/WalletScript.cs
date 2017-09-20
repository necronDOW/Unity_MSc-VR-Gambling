using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletScript : MonoBehaviour
{
    public PlayerData playerData;
    public ChipEmitterScript chipEmitter;

    private Text display;
    private Text changeDisplay;
    private Animator changeAnimator;

    private void Awake()
    {
        display = GetComponent<Text>();
        display.text = "Wallet: " + string.Format("{0:£0.00}", playerData.balance);

        changeDisplay = transform.Find("WalletChangeTxt").GetComponent<Text>();
        changeAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        playerData.Reset();
        Add(40.0f);
    }

    private void OnDisable()
    {
        changeDisplay.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public void Add(float value)
    {
        ModifyBalance(value);
        changeAnimator.SetTrigger("Add");

        if (chipEmitter && chipEmitter.isActiveAndEnabled)
            chipEmitter.Emit(value);
    }

    public void Deduct(float value)
    {
        if (playerData.balance >= value)
        {
            ModifyBalance(-value);
            changeAnimator.SetTrigger("Deduct");

            if (chipEmitter && chipEmitter.isActiveAndEnabled)
                chipEmitter.Destroy(value);
        }
    }

    public bool Ready()
    {
        return (changeDisplay.color.a == 0.0f);
    }

    public bool CanDeduct(float amount)
    {
        return (playerData.balance - amount >= 0.0f);
    }

    private void ModifyBalance(float value)
    {
        playerData.balance += value;
        display.text = "Wallet: " + string.Format("{0:£0.00}", playerData.balance);

        changeDisplay.text = string.Format("{0:£0.00}", value);
    }
}