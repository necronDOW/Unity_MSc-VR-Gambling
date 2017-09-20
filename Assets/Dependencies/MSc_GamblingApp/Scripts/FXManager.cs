using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour
{
    public FXScript[] positiveFX;
    public FXScript[] negativeFX;

    public void PlayPositiveFX()
    {
        for (int i = 0; i < positiveFX.Length; i++)
            positiveFX[i].Trigger();
    }

    public void PlayNegativeFX()
    {
        for (int i = 0; i < negativeFX.Length; i++)
            negativeFX[i].Trigger();
    }
}
