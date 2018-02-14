using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FCD_WalletScript : MonoBehaviour
{
    private Text targetText;
    private float _wallet;
    public float wallet
    {
        get { return _wallet; }
        set {
            float diff = value - _wallet;

            if (diff <= 0.0f || !potScript)
                _wallet += diff;
            else potScript.AddToPot(diff);
        }
    }

    public DialogueBox dbReference;
    public HL_PotScript potScript;
    public float startingBalance = 50.0f;

    private void Awake()
    {
        targetText = GetComponent<Text>();
        _wallet = startingBalance;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (targetText)
            targetText.text = string.Format("£{0:f2}", _wallet);
    }

    public void EmptyPot(bool merge)
    {
        if (potScript)
            potScript.EmptyPot(merge ? this : null);
    }

    public void DoublePot()
    {
        if (potScript)
            potScript.AddToPot(potScript.potTotal);
    }

    public void AddDirect(float amount)
    {
        _wallet += amount;
        UpdateUI();
    }

    public void CashOut()
    {
        if (dbReference) {
            DialogueOptions options = new DialogueOptions(string.Format("Balance: £{0:f2}", _wallet), "Do you wish to cash out?", 350, 150);
            options.AddButton("Yes", CashOutConfirmed);
            options.AddButton("Cancel", DialogueOptions.ButtonOptions.StandardAction.Cancel);

            dbReference.Show(options);
        }
        else CashOutConfirmed();
    }

    public void CashOutConfirmed()
    {
        Debug.Log("cashed out.");
    }
}
