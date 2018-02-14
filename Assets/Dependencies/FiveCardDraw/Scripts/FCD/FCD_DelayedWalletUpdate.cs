using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCD_DelayedWalletUpdate : MonoBehaviour
{
    public int requiredReady = 5;
    public int ready = 0;
    private FCD_WalletScript targetWallet;

    private void Awake()
    {
        targetWallet = GetComponent<FCD_WalletScript>();
    }

    public void IncrementReady()
    {
        ready++;
        if (ready >= requiredReady) {
            targetWallet.UpdateUI();
            ready = 0;
        }
    }
}
