using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Configs", menuName = "Game Configuration", order = 1)]
public class Configs : ScriptableObject
{
    // Intervention Properties
    public Intervention intervention = Intervention.None;
    public enum Intervention { None, Game, Pause }
    public int interventionSceneIndex = 0;
    public SceneMngr.TransitionMode interventionTransition = SceneMngr.TransitionMode.Instant;

    // Cards properties
    public int cardSceneInterventionTurn = 60;
    public float cardSceneDealSpeed = 1.0f;
    public Object cardSceneOrderFile;
    public string cardSceneOrderDirectory
    {
        get { return cardSceneOrderFile.name; }
        private set { }
    }

    // Dice properties
    public float diceSceneMaxReactionTime = 1.5f;
    public int[] diceSceneStopSignalTurns;
    public Object diceSceneOrderFile;
    public string diceSceneOrderDirectory
    {
        get { return diceSceneOrderFile.name; }
        private set { }
    }

    // Freeze-out properties
    public float freezeSceneTime = 90.0f;
}
