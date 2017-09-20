using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceGameUIScript : UITextScript
{
    public string winText = "GOOD";
    public string loseText = "BAD";

    public void PositiveAnimation()
    {
        Animate(winText, "Good");
    }

    public void NegativeAnimation()
    {
        Animate(loseText, "Bad");
    }
}
