using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data", order = 1)]
public class PlayerData : ScriptableObject
{
    [HideInInspector] public float balance = 0.0f;
    [HideInInspector] public int maxDiceStreak = -1;
    [HideInInspector] public bool cashedOut = false;

    public void UpdateStreak(int newStreak)
    {
        maxDiceStreak = Mathf.Max(maxDiceStreak, newStreak);
    }

    public void Reset()
    {
        balance = 0.0f;
        maxDiceStreak = -1;
        cashedOut = false;
    }

    public void ResetAndSave()
    {
        Reset();
    }
}
