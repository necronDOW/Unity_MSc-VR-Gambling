using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreakUIScript : UITextScript
{
    public void UpdateUI(int streak)
    {
        if (streak == 0)
            Visible(false);
        else Animate(streak.ToString());
    }
}
