using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpawnerScript : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject dicePrefab;
    public Configs config;
    [HideInInspector] public DiceAudioManager audioManager;
    public DiceGameUIScript gameUI;
    public StreakUIScript streakUI;
    public LoggerWrapper logger;
    public PlayerData playerData;

    private DiceScript[] dice;
    private int[] diceOrder;
    private int currentDiceIndex = 0;
    private bool unloaded = false;
    private int streak = 0;
    private FXManager fxManager;
    private int logFileIndex = -1;

    private void Awake()
    {
        dice = new DiceScript[spawnPoints.Length];
        audioManager = HelperTools.FindLocalGameObjectWithTag("Scene", gameObject.scene).GetComponent<DiceAudioManager>();
        fxManager = HelperTools.FindLocalGameObjectWithTag("FX", gameObject.scene).GetComponent<FXManager>();

        LoadOrder();
    }

    private void Start()
    {
        RespawnAll();

        if (logger)
            logFileIndex = logger.CreateLogFile("dicetask", "resp(ms)", "click(x)", "click(y)", "win");

        playerData.UpdateStreak(streak);
    }

    float reactionTimer = 0.0f;
    public void Update()
    {
        if (!unloaded)
        {
            reactionTimer += Time.deltaTime;

            if (reactionTimer >= config.diceSceneMaxReactionTime)
            {
                if (IsStopSignalTurn())
                    AddToStreak();
                else ResetStreak();

                RespawnAll();

                reactionTimer = 0.0f;
            }
        }
    }

    private void OnEnable()
    {
        if (logger)
            logger.RecordResponseTime(true);
    }

    private void OnDisable()
    {
        if (logger)
            logger.RecordResponseTime(false);
    }

    public void Evaluate(int clickedIndex)
    {
        if (AnimationsDone())
        {
            bool win = (dice[clickedIndex].value == 2 || dice[clickedIndex].value == 5) && !IsStopSignalTurn();

            if (logger)
            {
                logger.LogResponseTime(logFileIndex);
                logger.LogClickPoint(logFileIndex);
                logger.LogBool(logFileIndex, win, true);
            }

            // If value matches diamond face (2 or 5).
            if (win)
                AddToStreak();
            else ResetStreak();

            RespawnAll();
        }
    }
    
    private void RespawnAll()
    {
        reactionTimer = 0.0f;

        for (int i = 0; i < dice.Length; i++)
            Spawn(i);

        // 0.45f represents dice animation time, this value must be changed if animations are adjusted!
        if (IsStopSignalTurn())
            audioManager.PlayStopSignalSound(0.45f + 0.25f);
    }
    
    private void Spawn(int index)
    {
        if (currentDiceIndex < diceOrder.Length)
        {
            if (dice[index])
                Destroy(dice[index].gameObject);

            GameObject die = Instantiate(dicePrefab, spawnPoints[index].position, dicePrefab.transform.rotation);
            dice[index] = die.GetComponent<DiceScript>();

            dice[index].SetValue(diceOrder[currentDiceIndex++]);
        }
        else if (!unloaded)
        {
            unloaded = true;
            StartCoroutine(QuitAfterSeconds(0.5f));
        }
    }

    private bool AnimationsDone()
    {
        for (int i = 0; i < dice.Length; i++)
        {
            if (!dice[i] || !dice[i].animationDone)
                return false;
        }

        return true;
    }

    private void LoadOrder()
    {
        string[] rawOrder = Helper.ReadFile(config.diceSceneOrderDirectory, 2);

        if (rawOrder != null)
        {
            diceOrder = new int[rawOrder.Length];

            for (int i = 0; i < diceOrder.Length; i++)
                int.TryParse(rawOrder[i], out diceOrder[i]);
        }
    }

    private void AddToStreak()
    {
        streakUI.UpdateUI(++streak);
        gameUI.PositiveAnimation();
        fxManager.PlayPositiveFX();
        playerData.UpdateStreak(streak);
    }

    private void ResetStreak()
    {
        streak = 0;
        gameUI.NegativeAnimation();
        streakUI.UpdateUI(streak);
        fxManager.PlayNegativeFX();
    }

    private IEnumerator QuitAfterSeconds(float timeInSeconds)
    {
        yield return new WaitForSeconds(timeInSeconds);

        for (int i = 0; i < dice.Length; i++)
            Destroy(dice[i].gameObject);

        HelperTools.FindLocalGameObjectWithTag("Scene Manager", gameObject.scene).GetComponent<SceneMngr>().SwitchScene(0, SceneMngr.TransitionMode.Lerp);
    }

    private bool IsStopSignalTurn()
    {
        if (config.diceSceneStopSignalTurns.Length > 0)
        {
            int stopSignalTrigger = (currentDiceIndex / 2);

            for (int i = 0; i < config.diceSceneStopSignalTurns.Length; i++)
            {
                if (stopSignalTrigger == config.diceSceneStopSignalTurns[i])
                    return true;
            }
        }

        return false;
    }
}